using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    /// <summary>
    /// List of all meeples the player controls.
    /// </summary>
    public List<Meeple> Meeples;

    /// <summary>
    /// Amount of tokens drawn in a turn.
    /// </summary>
    public int DrawAmount;

    /// <summary>
    /// The contents of the token bag.
    /// </summary>
    public List<Token> TokenBag;

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
    /// The current turn number.
    /// </summary>
    public int Turn;

    /// <summary>
    /// The current state of the game.
    /// </summary>
    public GameState GameState;

    // Visual
    private List<BoardTile> CurrentlyHighlightedMovementTargets;

    #region Initialization

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
        AddStartingTokensToBag();
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

    private void AddStartingTokensToBag()
    {
        TokenBag = new List<Token>();
        AddTokenToBag(TokenShapeDefOf.Pebble, TokenColorDefOf.White, TokenSizeDefOf.Small);
        AddTokenToBag(TokenShapeDefOf.Pebble, TokenColorDefOf.White, TokenSizeDefOf.Small);
        AddTokenToBag(TokenShapeDefOf.Pebble, TokenColorDefOf.White, TokenSizeDefOf.Small);
        AddTokenToBag(TokenShapeDefOf.Pebble, TokenColorDefOf.White, TokenSizeDefOf.Small);
        AddTokenToBag(TokenShapeDefOf.Pebble, TokenColorDefOf.Black, TokenSizeDefOf.Small);
        AddTokenToBag(TokenShapeDefOf.Pebble, TokenColorDefOf.Black, TokenSizeDefOf.Small);
        DrawAmount = 4;
    }

    #endregion

    #region Game Loop

    void Update()
    {
        WorldManager.UpdateHoveredObjects();

        if (Input.GetKeyDown(KeyCode.Space) && GameState == GameState.WaitingForDraw) StartTurn();

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

        // Skip draw interaction if the player has no means to change the draw
        bool playerCanChangeDraw = false;
        if (!playerCanChangeDraw)
        {
            ConfirmDraw();
        }
    }

    /// <summary>
    /// Ends the phase where the player can change what he has drawn and starts the movement phase.
    /// </summary>
    public void ConfirmDraw()
    {
        // Find all initial movement options
        UpdateCurrentMovementOptions();
        HighlightAllMovementOptionTargets();

        GameState = GameState.Movement;
    }

    public void EndMovement()
    {
        // Currently ending movement ends a turn - this will get replaced so the player can manually end the turn, so they can also use items etc.
        EndTurn();
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
    /// Each turn starts by drawing tokens out of your bag.
    /// </summary>
    private List<Token> DrawTokens()
    {
        int drawAmount = DrawAmount;
        int bagSize = TokenBag.Count();
        if (drawAmount > bagSize) drawAmount = bagSize;
        List<Token> remainingTokens = new List<Token>(TokenBag);
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

    public void AddTokenToBag(TokenShapeDef shape, TokenColorDef color, TokenSizeDef size)
    {
        Token newToken = TokenGenerator.GenerateToken(shape, color, size);
        TokenBag.Add(newToken);
    }

    public void TeleportMeeple(Meeple meeple, BoardTile tile)
    {
        meeple.SetPosition(tile);
    }

    public void ExecuteMovement(MovementOption movement)
    {
        MeepleMovementAnimator.AnimateMove(movement, onComplete: EndMovement);
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
}
