using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance;

    /// <summary>
    /// List of all meeples the player controls.
    /// </summary>
    public List<Meeple> Meeples;

    /// <summary>
    /// Amount of tokens drawn in a turn.
    /// </summary>
    public int DrawAmount;

    /// <summary>
    /// The contents of the token pouch.
    /// </summary>
    public List<Token> TokenPouch;

    /// <summary>
    /// The collecion of all board segments that make up the board.
    /// </summary>
    public List<BoardSegment> Board;

    /// <summary>
    /// The result of the draw of this turn.
    /// </summary>
    public DrawResult CurrentDrawResult { get; private set; }

    /// <summary>
    /// A list of all current meeple movement options.
    /// </summary>
    public List<MovementOption> MovementOptions { get; private set; }

    /// <summary>
    /// Flag if all movement has been completed this turn.
    /// </summary>
    public bool IsMovementDone { get; private set; }

    /// <summary>
    /// The current chapter of the game.
    /// </summary>
    public int Chapter { get; private set; }

    /// <summary>
    /// The current turn number.
    /// </summary>
    public int Turn { get; private set; }

    /// <summary>
    /// The current state of the game.
    /// </summary>
    public GameState GameState { get; private set; }

    /// <summary>
    /// Returns if the player is currently looking at the contents of the token pouch.
    /// </summary>
    public bool IsTokenPouchOpen { get; private set; }

    // Visual
    private List<BoardTile> CurrentlyHighlightedMovementTargets;

    #region Initialization

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Load defs
        DefDatabaseRegistry.AddAllDefs();
        DefDatabaseRegistry.ResolveAllReferences();
        DefDatabaseRegistry.OnLoadingDone();

        StartGame();
    }

    public void StartGame()
    {
        GameState = GameState.Initializing;

        WorldManager.Initialize();

        MovementOptions = new List<MovementOption>();

        CreateInitialBoard();
        AddStartingMeeple();
        AddStartingTokensToPouch();
        CameraHandler.Instance.SetPosition(Meeples[0].transform.position);

        Turn = 0;
        StartPreTurn();
    }

    private void CreateInitialBoard()
    {
        Board = new List<BoardSegment>();
        BoardSegment initialSegment = BoardSegmentGenerator.GenerateSegment(this, new Vector2Int(0, 0), numTiles: 10);
        AddBoardSegment(initialSegment);
    }

    private void AddStartingMeeple()
    {
        AddPlayerMeeple(Board[0].Tiles.Last());
    }

    private void AddStartingTokensToPouch()
    {
        TokenPouch = new List<Token>();
        AddTokenToPouch(TokenShapeDefOf.Pebble, TokenColorDefOf.White, TokenSizeDefOf.Small);
        AddTokenToPouch(TokenShapeDefOf.Pebble, TokenColorDefOf.White, TokenSizeDefOf.Small);
        AddTokenToPouch(TokenShapeDefOf.Pebble, TokenColorDefOf.White, TokenSizeDefOf.Small);
        AddTokenToPouch(TokenShapeDefOf.Pebble, TokenColorDefOf.White, TokenSizeDefOf.Small);
        AddTokenToPouch(TokenShapeDefOf.Pebble, TokenColorDefOf.Black, TokenSizeDefOf.Small);
        AddTokenToPouch(TokenShapeDefOf.Pebble, TokenColorDefOf.Black, TokenSizeDefOf.Small);
        DrawAmount = 4;
    }

    #endregion

    #region Game Loop

    void Update()
    {
        WorldManager.UpdateHoveredObjects();

        if (Input.GetKeyDown(KeyCode.Space) && GameState == GameState.WaitingForDraw && !IsTokenPouchOpen) StartTurn();

        if (GameState == GameState.Movement)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (WorldManager.HoveredBoardTile != null)
                {
                    MovementOption movement = MovementOptions.FirstOrDefault(o => o.TargetTile == WorldManager.HoveredBoardTile);
                    if(movement != null)
                    {
                        ExecuteMovement(movement);
                        UnhighlightAllMovementOptionTargets();
                    }
                }
            }
        }
    }

    public void StartPreTurn()
    {
        GameUI.Instance.TurnDraw.ShowWaitingText();
        GameUI.Instance.PostItButton.SetText("Draw");

        GameState = GameState.WaitingForDraw;
    }

    public void StartTurn()
    {
        Turn++;
        Debug.Log($"Starting Turn {Turn}.");
        CurrentDrawResult = new DrawResult(DrawTokens());
        TokenPhysicsManager.ThrowTokens(this);
        GameUI.Instance.TurnDraw.ShowTurnDraw(this);
        GameState = GameState.DrawInteraction;
        GameUI.Instance.PostItButton.SetText("Confirm Draw");
    }

    /// <summary>
    /// Ends the phase where the player can change what he has drawn and starts the movement phase.
    /// </summary>
    public void ConfirmDraw()
    {
        // Find all initial movement options
        UpdateCurrentMovementOptions();
        HighlightAllMovementOptionTargets();
        GameUI.Instance.PostItButton.SetText("End\nTurn");

        // Post it button
        IsMovementDone = (MovementOptions.Count == 0);
        if (IsMovementDone) GameUI.Instance.PostItButton.Enable();
        else GameUI.Instance.PostItButton.Disable();

        GameState = GameState.Movement;
    }

    public void OnMovementDone(MovementOption move)
    {
        // Exectute OnLandEffect of arrived tile
        move.TargetTile.OnLand();

        // Update movements
        UpdateCurrentMovementOptions();

        // Enable end turn button
        IsMovementDone = true;
        if (IsMovementDone) GameUI.Instance.PostItButton.Enable();
        else GameUI.Instance.PostItButton.Disable();
    }

    public void EndTurn()
    {
        GameState = GameState.PostTurn;

        // Collect all tokens off the board
        StartCoroutine(TokenPhysicsManager.CollectTokens(this));

        // Wait for player to start next turn
        StartPreTurn();
    }

    /// <summary>
    /// Each turn starts by drawing tokens out of your pouch.
    /// </summary>
    private List<Token> DrawTokens()
    {
        int drawAmount = DrawAmount;
        int pouchSize = TokenPouch.Count();
        if (drawAmount > pouchSize) drawAmount = pouchSize;
        List<Token> remainingTokens = new List<Token>(TokenPouch);
        List<Token> drawnTokens = new List<Token>();
        for (int i = 0; i < DrawAmount; i++)
        {
            Token token = remainingTokens.RandomElement();
            drawnTokens.Add(token);
            remainingTokens.Remove(token);
        }
        return drawnTokens;
    }

    #region Game Actions

    public Meeple AddPlayerMeeple(BoardTile tile)
    {
        GameObject meeplePrefab = ResourceManager.LoadPrefab("Prefabs/Meeple");
        GameObject meepleObject = GameObject.Instantiate(meeplePrefab);
        Meeple newMeeple = meepleObject.GetComponent<Meeple>();
        newMeeple.Init(this);
        Meeples.Add(newMeeple);

        TeleportMeeple(newMeeple, tile);

        return newMeeple;
    }

    public void AddBoardSegment(BoardSegment segment)
    {
        Board.Add(segment);
    }

    public void AddTokenToPouch(TokenShapeDef shape, TokenColorDef color, TokenSizeDef size, bool silent = true)
    {
        Token newToken = TokenGenerator.GenerateToken(shape, color, size);
        TokenPouch.Add(newToken);

        if (!silent)
        {
            GameUI.Instance.ItemReceivedDisplay.ShowTokenReceived(newToken);
        }
    }

    public void TeleportMeeple(Meeple meeple, BoardTile tile)
    {
        meeple.SetPosition(tile);
    }

    public void ExecuteMovement(MovementOption movement)
    {
        MeepleMovementAnimator.AnimateMove(movement, onComplete: OnMovementDone);
    }

    #endregion

    #endregion

    #region Movement

    private void UpdateCurrentMovementOptions()
    {
        MovementOptions.Clear();

        foreach(Meeple meeple in Meeples)
        {
            MovementOptions.AddRange(GetMeepleMovementOptions(meeple));
        }
    }

    private List<MovementOption> GetMeepleMovementOptions(Meeple meeple)
    {
        List<MovementOption> options = new List<MovementOption>();

        BoardTile startTile = meeple.Tile;
        List<List<BoardTile>> paths = GetPaths(startTile, prevPosition: null, remainingMovementPoints: CurrentDrawResult.Resources[ResourceDefOf.MovementPoint]);
        foreach(List<BoardTile> path in paths)
        {
            BoardTile target = path.Last();
            path.Remove(target);
            options.Add(new MovementOption(meeple, startTile, new List<BoardTile>(path), target));
        }

        return options;
    }

    private void RemoveMovementOptionsFor(Meeple meeple)
    {
        MovementOptions = MovementOptions.Where(o => o.Meeple != meeple).ToList();
    }

    /// <summary>
    /// Returns all possible paths of exactly remainingMovementPoints steps starting from position, never immediately back‑racking to prevPosition. 
    /// Each path is a list of the tiles visited in order(excluding the start tile).
    /// </summary>
    private List<List<BoardTile>> GetPaths(BoardTile position, BoardTile prevPosition, int remainingMovementPoints)
    {
        var paths = new List<List<BoardTile>>();

        // No movement points --> no valid paths
        if (remainingMovementPoints <= 0)
            return paths;

        // Try every connected tile, except the one we just came from
        foreach (BoardTile next in position.ConnectedTiles)
        {
            if (prevPosition != null && next == prevPosition)
                continue;

            // If this is our last step, yield a single‑step path [next]
            if (remainingMovementPoints == 1)
            {
                paths.Add(new List<BoardTile> { next });
            }
            else
            {
                // Otherwise, recurse one step deeper
                var subPaths = GetPaths(next, position, remainingMovementPoints - 1);
                foreach (var sub in subPaths)
                {
                    // Prepend our current move onto each sub‐path
                    var path = new List<BoardTile> { next };
                    path.AddRange(sub);
                    paths.Add(path);
                }
            }
        }

        return paths;
    }

    private void HighlightAllMovementOptionTargets()
    {
        CurrentlyHighlightedMovementTargets = MovementOptions.Select(o => o.TargetTile).ToList();
        foreach (BoardTile target in CurrentlyHighlightedMovementTargets)
        {
            target.HighlightAsMovementOption();
        }
    }

    private void UnhighlightAllMovementOptionTargets()
    {
        foreach (BoardTile target in CurrentlyHighlightedMovementTargets)
        {
            target.UnhighlightAsMovementOption();
        }
        CurrentlyHighlightedMovementTargets.Clear();
    }

    #endregion

    #region Pouch Contents Display

    // how far in front of the camera the pouch contents appear
    private const float TokenDisplayDistance = 4f;
    // how wide the random spread is (in world units)
    private const float pouchSpreadRadius = 1.5f;
    // minimum distance between any two tokens
    private const float pouchMinSeparation = 0.5f;
    private List<Token> PouchDisplayTokens = new List<Token>();


    public void OpenTokenPouch()
    {
        // show your leather‑cloth background
        IsTokenPouchOpen = true;

        Vector3 center = new Vector3(0, 100f, 0);

        // pre‐generate non‐overlapping 2D offsets
        var offsets2D = new List<Vector2>();
        foreach (Token token in TokenPouch)
        {
            Vector2 candidate;
            int attempts = 0;
            do
            {
                candidate = Random.insideUnitCircle * pouchSpreadRadius;
                attempts++;
            }
            // retry up to 10 times to find a spot that’s at least pouchMinSeparation away
            while (attempts < 10 && offsets2D.Any(o => Vector2.Distance(o, candidate) < pouchMinSeparation));
            offsets2D.Add(candidate);
        }

        for (int i = 0; i < TokenPouch.Count; i++)
        {
            Token orig = TokenPouch[i];

            // Make a copy so we don't need to move the original token
            Token token = TokenGenerator.GenerateTokenCopy(orig); 

            // make sure it's visible
            token.Show();

            // position in world using our 2D offset on the camera‐plane
            Vector2 c2 = offsets2D[i];
            Vector3 offset = new Vector3(c2.x, 0.3f, c2.y);
            token.transform.position = center + offset;

            // optionally face the camera (if your token mesh isn't round)
            token.transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));

            // Save token in display list
            PouchDisplayTokens.Add(token);

            // Move camera
            CameraHandler.Instance.Camera.transform.position = new Vector3(0f, 100f + TokenDisplayDistance, 0f);
            CameraHandler.Instance.Camera.transform.rotation = Quaternion.Euler(90, 0, 0);

            // Freeze camera
            CameraHandler.Instance.Freeze();
        }
    }

    public void CloseTokenPouch()
    {
        IsTokenPouchOpen = false;

        // hide everything again so it doesn't clutter the next turn
        foreach (var token in PouchDisplayTokens)
        {
            GameObject.Destroy(token.gameObject);
        }
        PouchDisplayTokens.Clear();

        // Unfreeze camera
        CameraHandler.Instance.Unfreeze();
        CameraHandler.Instance.UpdatePosition();

        HelperFunctions.UnfocusNonInputUiElements();
    }

    #endregion
}
