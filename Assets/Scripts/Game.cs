using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance;

    #region Static Rules

    private const int NEW_QUEST_INTERVAL = 3;
    private const int FIRST_NEW_QUEST_TURN = 3;

    private const int NEW_RULE_INTERVAL = 20;
    private const int FIRST_NEW_RULE_TURN = 15;

    #endregion

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
    public Board Board;

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
    /// The current major goal that has to be completed to reach the next chapter.
    /// </summary>
    public ObjectiveGoal CurrentChapterMission { get; private set; }

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

    /// <summary>
    /// The action prompt that is currently shown to the player.
    /// </summary>
    public ActionPrompt CurrentActionPrompt { get; private set; }

    /// <summary>
    /// Returns if an action prompt is currently active.
    /// </summary>
    public bool IsShowingActionPrompt => CurrentActionPrompt != null;

    public List<Objective> ActiveQuests { get; private set; }

    // Visual
    private List<Tile> CurrentlyHighlightedMovementTargets;

    #region Initialization

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // References
        Board = Board.Instance;

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
        InitializeFirstChapter();
        CameraHandler.Instance.SetPosition(Meeples[0].transform.position);

        StartTurn();
    }

    private void CreateInitialBoard()
    {
        Board = BoardSegmentGenerator.GenerateBoard(this);
        BoardSegmentGenerator.GenerateStartSegment(this, Board, minTiles: 18, maxTiles: 22);
        Board.Init(Board.Tiles.First());
    }

    private void AddStartingMeeple()
    {
        AddPlayerMeeple(Board.StartTile);
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

    private void InitializeFirstChapter()
    {
        Chapter = 1;
        Turn = 0;
        ActiveQuests = new List<Objective>();
        CurrentChapterMission = ChapterMissionGenerator.GenerateChapterObectiveGoal(Chapter);
        GameUI.Instance.ChapterDisplay.UpdateDisplay();
    }

    #endregion

    #region Game Loop

    void Update()
    {
        WorldManager.UpdateHoveredObjects();

        if (Input.GetKeyDown(KeyCode.Space) && GameState == GameState.PreDraw && !IsTokenPouchOpen) DrawInitialTokens();

        if (GameState == GameState.MovingPhase && !IsShowingActionPrompt)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (WorldManager.HoveredBoardTile != null)
                {
                    MovementOption movement = MovementOptions.FirstOrDefault(o => o.TargetTile == WorldManager.HoveredBoardTile);
                    if (movement != null)
                    {
                        ExecuteMovement(movement);
                        UnhighlightAllMovementOptionTargets();
                    }
                }
            }
        }
    }

    public void StartTurn()
    {
        Turn++;
        GameUI.Instance.TurnDraw.ShowWaitingText();
        GameUI.Instance.PostItButton.SetText("Draw");

        // Check if time for new quest
        if ((Turn - FIRST_NEW_QUEST_TURN) % NEW_QUEST_INTERVAL == 0)
        {
            ActiveQuests.Add(QuestGenerator.GenerateQuest());
        }
        GameUI.Instance.QuestPanel.Refresh();

        GameState = GameState.PreDraw;
    }

    public void DrawInitialTokens()
    {
        Debug.Log($"Starting Turn {Turn}.");
        CurrentDrawResult = new DrawResult(DrawTokens());
        TokenPhysicsManager.ThrowTokens(this);
        GameUI.Instance.TurnDraw.ShowTurnDraw(this);
        GameState = GameState.DrawingPhase;
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

        GameState = GameState.MovingPhase;
    }

    /// <summary>
    /// Gets called when a movement of the player meeple is done and it lands on a tile.
    /// </summary>
    public void OnMovementDone(MovementOption move)
    {
        // Exectute OnLandEffect of arrived tile
        move.TargetTile.OnLand();

        // Show action prompts
        ShowNextActionPrompt();
    }

    /// <summary>
    /// Gets called when all action prompts are completed after a meeple landed on a tile.
    /// </summary>
    private void OnActionPromptsDone()
    {
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

        // Check if major goal is complete
        if(CurrentChapterMission.IsComplete)
        {
            // Remove old goal related stuff
            CurrentChapterMission.OnRemoved();

            // Set new goal
            Chapter++;
            CurrentChapterMission = ChapterMissionGenerator.GenerateChapterObectiveGoal(Chapter);
            GameUI.Instance.ChapterDisplay.UpdateDisplay();
        }

        // Wait for player to start next turn
        StartTurn();
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
        for (int i = 0; i < drawAmount; i++)
        {
            Token token = remainingTokens.RandomElement();
            drawnTokens.Add(token);
            remainingTokens.Remove(token);
        }
        return drawnTokens;
    }

    #endregion

    #region Game Actions

    public Meeple AddPlayerMeeple(Tile tile)
    {
        GameObject meeplePrefab = ResourceManager.LoadPrefab("Prefabs/Meeple");
        GameObject meepleObject = GameObject.Instantiate(meeplePrefab);
        Meeple newMeeple = meepleObject.GetComponent<Meeple>();
        newMeeple.Init(this);
        Meeples.Add(newMeeple);

        TeleportMeeple(newMeeple, tile);

        return newMeeple;
    }

    public Token AddTokenToPouch(TokenShapeDef shape, TokenColorDef color, TokenSizeDef size)
    {
        Token newToken = TokenGenerator.GenerateToken(shape, color, size);
        AddTokenToPouch(newToken);
        return newToken;
    }
    public void AddTokenToPouch(Token token)
    {
        TokenPouch.Add(token);
    }

    public void RemoveTokenFromPouch(Token token)
    {
        TokenPouch.Remove(token);
        GameObject.Destroy(token);
    }

    public void TeleportMeeple(Meeple meeple, Tile tile)
    {
        meeple.SetPosition(tile);
    }

    public void ExecuteMovement(MovementOption movement)
    {
        MeepleMovementAnimator.AnimateMove(movement, onComplete: OnMovementDone);
    }

    public void SetMajorGoalAsComplete()
    {
        CurrentChapterMission.SetAsComplete();
    }

    #endregion

    #region Action Prompts

    private Queue<ActionPrompt> ActionPromptQueue = new Queue<ActionPrompt>();

    public void QueueActionPrompt(ActionPrompt prompt)
    {
        ActionPromptQueue.Enqueue(prompt);
    }

    private void ShowNextActionPrompt()
    {
        if (ActionPromptQueue.Count == 0)
        {
            OnActionPromptsDone();
            return;
        }

        CurrentActionPrompt = ActionPromptQueue.Dequeue();
        CurrentActionPrompt.Show();
    }

    public void CompleteCurrentActionPrompt()
    {
        // Close prompt
        CurrentActionPrompt.Close();
        CurrentActionPrompt = null;

        // Update movements
        UpdateCurrentMovementOptions();

        // Show next prompt
        ShowNextActionPrompt();
    }
    
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

        Tile startTile = meeple.Tile;
        List<List<Tile>> paths = GetPaths(startTile, prevPosition: null, remainingMovementPoints: CurrentDrawResult.Resources[ResourceDefOf.MovementPoint]);
        foreach(List<Tile> path in paths)
        {
            Tile target = path.Last();
            path.Remove(target);
            options.Add(new MovementOption(meeple, startTile, new List<Tile>(path), target));
        }

        return options;
    }

    /// <summary>
    /// Returns all possible movement paths starting from `position`.  
    /// - Any path that uses up exactly `remainingMovementPoints` steps, **or**
    /// - Any shorter path that ends on a tile where CanMeepleStopHere() is true  
    /// never immediately back‑tracks to `prevPosition`.
    /// </summary>
    private List<List<Tile>> GetPaths(Tile position, Tile prevPosition, int remainingMovementPoints)
    {
        var paths = new List<List<Tile>>();

        // No movement points → no valid paths
        if (remainingMovementPoints <= 0)
            return paths;

        foreach (Tile next in position.ConnectedTiles)
        {
            // don't immediately reverse
            if (prevPosition != null && next == prevPosition)
                continue;

            // 1) full‑length paths (use your last movement point here)
            if (remainingMovementPoints == 1)
            {
                paths.Add(new List<Tile> { next });
            }
            else
            {
                // 2) early‑stop paths: you can stop here any time if allowed
                if (next.CanMeepleStopHere())
                {
                    paths.Add(new List<Tile> { next });
                }

                // 3) longer paths: spend one point and recurse
                var subPaths = GetPaths(next, position, remainingMovementPoints - 1);
                foreach (var sub in subPaths)
                {
                    var path = new List<Tile> { next };
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
        foreach (Tile target in CurrentlyHighlightedMovementTargets)
        {
            target.HighlightAsMovementOption();
        }
    }

    private void UnhighlightAllMovementOptionTargets()
    {
        foreach (Tile target in CurrentlyHighlightedMovementTargets)
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

    #region Getters

    public int GetDraftDrawAmount()
    {
        return 3;
    }

    #endregion
}
