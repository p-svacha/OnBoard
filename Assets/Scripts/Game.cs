using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance;
    public GameObject TokenPouchObject;

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
    public TurnDraw CurrentDraw { get; private set; }

    /// <summary>
    /// A list of all current meeple movement options.
    /// </summary>
    public List<MovementOption> MovementOptions { get; private set; }

    /// <summary>
    /// The current chapter of the game.
    /// </summary>
    public int Chapter { get; private set; }

    /// <summary>
    /// The current goal that has to be completed to complete the current chapter.
    /// </summary>
    public ChapterGoal CurrentChapterGoal { get; private set; }

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
    public bool InActionPrompt => CurrentActionPrompt != null;

    /// <summary>
    /// The current health the player has remaining in half hearts.
    /// </summary>
    public int Health { get; private set; }

    /// <summary>
    /// The maximum health the player can have in half hearts.
    /// </summary>
    public int MaxHealth { get; private set; }

    /// <summary>
    /// All quests that are currently active.
    /// </summary>
    public List<Quest> ActiveQuests { get; private set; }

    /// <summary>
    /// All items in the possession of the player.
    /// </summary>
    public List<Item> Items { get; private set; }

    /// <summary>
    /// The amount of each collectible resource the player currently has.
    /// </summary>
    public Dictionary<ResourceDef, int> Resources;

    /// <summary>
    /// The amount of drawing phase resources the player has remaining this drawing phase.
    /// </summary>
    public Dictionary<ResourceDef, int> RemainingDrawPhaseResources { get; private set; }

    /// <summary>
    /// The amount of drawing phase resources the player had available in total this drawing phase.
    /// </summary>
    public Dictionary<ResourceDef, int> TotalDrawPhaseResources { get; private set; }

    /// <summary>
    /// The amount of moving phase resources the player has remaining this moving phase.
    /// </summary>
    public Dictionary<ResourceDef, int> RemainingMovingPhaseResources { get; private set; }

    /// <summary>
    /// The amount of moving phase resources the player had available in total this moving phase.
    /// </summary>
    public Dictionary<ResourceDef, int> TotalMovingPhaseResources { get; private set; }

    /// <summary>
    /// The rulebook tracks the current state of all active rules and contains all logic regarding rules.
    /// </summary>
    public Rulebook Rulebook { get; private set; }

    // Visual
    private List<Tile> CurrentlyHighlightedMovementTargets;

    #region Initialization

    private void Awake()
    {
        Instance = this;
        TokenPouchObject = GameObject.Find("TokenPouch");
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
        TokenPhysicsManager.Initialize();

        MovementOptions = new List<MovementOption>();
        TotalDrawPhaseResources = new Dictionary<ResourceDef, int>();
        RemainingDrawPhaseResources = new Dictionary<ResourceDef, int>();
        TotalMovingPhaseResources = new Dictionary<ResourceDef, int>();
        RemainingMovingPhaseResources = new Dictionary<ResourceDef, int>();

        InitializeRulebook();
        CreateInitialBoard();
        AddStartingMeeple();
        AddStartingContent();
        InitializeFirstChapter();
        CameraHandler.Instance.SetPosition(Meeples[0].transform.position);

        StartTurn();
    }

    private void CreateInitialBoard()
    {
        Board = BoardGenerator.GenerateBoard(this);
        BoardGenerator.GenerateStartSegment();
        Board.Init(Board.Tiles.First());
    }

    private void AddStartingMeeple()
    {
        AddPlayerMeeple(Board.StartTile);
        MaxHealth = 6;
        Health = 6;
        Items = new List<Item>();
        GameUI.Instance.HealthDisplay.Refresh();
    }

    private void AddStartingContent()
    {
        // Tokens
        TokenPouch = new List<Token>();
        AddTokenToPouch(TokenShapeDefOf.Pebble, new() { new(TokenColorDefOf.White) }, TokenSizeDefOf.Small);
        AddTokenToPouch(TokenShapeDefOf.Pebble, new() { new(TokenColorDefOf.White) }, TokenSizeDefOf.Small);
        AddTokenToPouch(TokenShapeDefOf.Pebble, new() { new(TokenColorDefOf.White) }, TokenSizeDefOf.Small);
        AddTokenToPouch(TokenShapeDefOf.Pebble, new() { new(TokenColorDefOf.White) }, TokenSizeDefOf.Small);
        AddTokenToPouch(TokenShapeDefOf.Pebble, new() { new(TokenColorDefOf.Black) }, TokenSizeDefOf.Small);
        AddTokenToPouch(TokenShapeDefOf.Pebble, new() { new(TokenColorDefOf.Black) }, TokenSizeDefOf.Small);
        AddTokenToPouch(TokenShapeDefOf.Coin, new() { new(TokenColorDefOf.White), new(TokenColorDefOf.Black) }, TokenSizeDefOf.Small);
        DrawAmount = 4;

        // Items
        AddItem(ItemGenerator.GenerateRandomItem());

        // Resources
        Resources = new Dictionary<ResourceDef, int>();
        AddResource(ResourceDefOf.Gold, 5);
    }

    private void InitializeFirstChapter()
    {
        Chapter = 1;
        Turn = 0;
        ActiveQuests = new List<Quest>();
        SetNextChapterGoal();
    }

    private void InitializeRulebook()
    {
        Rulebook = new Rulebook();
    }

    #endregion

    #region Game Loop

    void Update()
    {
        WorldManager.UpdateHoveredObjects();

        InputHandler.HandleInputs();
    }

    public void StartTurn()
    {
        Turn++;
        Debug.Log($"Starting Turn {Turn}.");

        // Check if time for new quest
        if ((Turn - FIRST_NEW_QUEST_TURN) % NEW_QUEST_INTERVAL == 0)
        {
            ActiveQuests.Add(QuestGenerator.GenerateQuest());
        }

        // UI
        GameUI.Instance.TurnDraw.ShowWaitingText();
        GameUI.Instance.GameLoopButton.SetText("Draw");
        GameUI.Instance.TurnPhaseResources.Refresh();
        GameUI.Instance.QuestPanel.Refresh();
        GameUI.Instance.ItemPanel.Refresh();
        GameUI.Instance.GameLoopButton.Enable();

        GameState = GameState.PreDraw;
    }

    public void DrawInitialTokens()
    {
        // Draw initial tokens
        CurrentDraw = new TurnDraw();
        GameUI.Instance.TurnDraw.Refresh();

        TokenPhysicsManager.ThrowInitialTokens(this);
        GameState = GameState.DrawingPhase;
        GameUI.Instance.GameLoopButton.SetText("Confirm Draw");

        // Get amount of redraws
        TotalDrawPhaseResources.Clear();
        foreach (Item item in Items) TotalDrawPhaseResources.IncrementMultiple(item.GetDrawPhaseResources());
        RemainingDrawPhaseResources = new Dictionary<ResourceDef, int>(TotalDrawPhaseResources);

        // Disable "Confirm Draw" button until all tokens are resting
        GameUI.Instance.GameLoopButton.Disable();

        // UI
        GameUI.Instance.TurnPhaseResources.Refresh();
    }

    /// <summary>
    /// Ends the phase where the player can change what he has drawn and starts the movement phase.
    /// </summary>
    public void ConfirmDraw()
    {
        GameState = GameState.MovingPhase;

        // Set resources for moving phase
        TotalMovingPhaseResources.Clear();
        TotalMovingPhaseResources.IncrementMultiple(CurrentDraw.GetMovingPhaseResources()); // Add resources from tokens
        RemainingMovingPhaseResources = new Dictionary<ResourceDef, int>(TotalMovingPhaseResources);

        // Prepare possible actions for player
        PrepareMovingPhasePlayerActions();

        // UI
        GameUI.Instance.GameLoopButton.SetText("End\nTurn");
        GameUI.Instance.TurnPhaseResources.Refresh();
    }

    /// <summary>
    /// Gets executed during the moving phase when the player is free again to perform their next action (i.e. move a meeple, interacting with a tile, etc.).
    /// </summary>
    private void PrepareMovingPhasePlayerActions()
    {
        // Find all movement options
        UpdateCurrentMovementOptions();
        HighlightAllMovementOptionTargets();

        // Identify interactions the player meeples can perform on the tiles they are on
        GameUI.Instance.TileInteractionMenu.Refresh();

        // Game loop button (can only end turn when all moveme
        if (CanEndTurn()) GameUI.Instance.GameLoopButton.Enable();
        else GameUI.Instance.GameLoopButton.Disable();
    }

    /// <summary>
    /// Returns if the player can end the turn.
    /// </summary>
    private bool CanEndTurn()
    {
        if (RemainingMovementPoints > 0) return false;
        return true;
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

    public void EndTurn()
    {
        GameState = GameState.PostTurn;

        // Disable game loop button
        GameUI.Instance.GameLoopButton.Disable();
        GameUI.Instance.TurnPhaseResources.Refresh();

        // Collect all tokens off the board
        StartCoroutine(TokenPhysicsManager.CollectTokens(this));

        // Quests
        foreach (Quest quest in ActiveQuests) quest.OnTurnPassed();
        GameUI.Instance.QuestPanel.Refresh();

        // Rulebook
        Rulebook.OnTurnPassed();

        // Tile interactions
        GameUI.Instance.TileInteractionMenu.Hide();

        // Go through post turn action prompts
        ShowNextActionPrompt();
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

    public Token AddTokenToPouch(TokenShapeDef shape, List<TokenSurface> surfaces, TokenSizeDef size)
    {
        Token newToken = TokenGenerator.GenerateToken(shape, surfaces, size);
        AddTokenToPouch(newToken);
        return newToken;
    }
    public void AddTokenToPouch(Token token)
    {
        TokenPouch.Add(token);
        token.IsInPouch = true;
        token.transform.SetParent(TokenPouchObject.transform);  
    }
    public void RemoveTokenFromPouch(Token token)
    {
        TokenPouch.Remove(token);
        token.IsInPouch = false;
        GameObject.Destroy(token);
    }
    public void UpgradeTokenSize(Token token)
    {
        if (token.Size == TokenSizeDefOf.Small) token.SetSize(TokenSizeDefOf.Medium);
        else if (token.Size == TokenSizeDefOf.Medium) token.SetSize(TokenSizeDefOf.Big);
        else if (token.Size == TokenSizeDefOf.Big) token.SetSize(TokenSizeDefOf.Large);
    }

    public void AddItem(Item item)
    {
        Items.Add(item);
        GameUI.Instance.ItemPanel.Refresh();
    }

    public void AddResource(ResourceDef resource, int amount)
    {
        if (resource.Type != ResourceType.Collectable) throw new System.Exception($"Can't add resource {resource.LabelCap} since it is not a collectable resource.");
        Resources.Increment(resource, amount);
        GameUI.Instance.ResourcesPanel.Refresh();
    }

    public void RemoveResource(ResourceDef resource, int amount)
    {
        if (resource.Type != ResourceType.Collectable) throw new System.Exception($"Can't remove resource {resource.LabelCap} since it is not a collectable resource.");
        Resources.Decrement(resource, amount);
        GameUI.Instance.ResourcesPanel.Refresh();
    }

    public void TeleportMeeple(Meeple meeple, Tile tile)
    {
        meeple.SetPosition(tile);
    }

    public void ExecuteMovement(MovementOption movement)
    {
        // Resources
        RemainingMovingPhaseResources[ResourceDefOf.MovementPoint] -= movement.Length;

        // Start moving
        MeepleMovementAnimator.AnimateMove(movement, onComplete: OnMovementDone);

        // FX
        UnhighlightAllMovementOptionTargets();

        // UI
        GameUI.Instance.TurnPhaseResources.Refresh();
        GameUI.Instance.TileInteractionMenu.Hide();
    }

    public void QueueCompleteChapter()
    {
        QueueActionPrompt(new ActionPrompt_ChapterComplete());
    }

    /// <summary>
    /// Only call from action prompt.
    /// </summary>
    public void DoCompleteChapter()
    {
        // Queue draft window
        string title = $"Chapter {Chapter} complete !";
        string subtitle = "Choose a reward";
        List<ChapterReward> rewardOptions = ChapterRewardGenerator.GenerateRewards(Chapter, amount: 3);
        List<IDraftable> draftOptions = rewardOptions.Select(r => (IDraftable)r).ToList();
        GameUI.Instance.DraftWindow.Show(title, subtitle, draftOptions, isDraft: true, callback: OnChapterRewardChosen);

        // Queue board expansion
        QueueActionPrompt(new ActionPrompt_BoardRegionAdded());

        // Increase chapter
        Chapter++;

        // Remove old goal related stuff
        CurrentChapterGoal.OnCompleted();

        // Set new goal
        SetNextChapterGoal();
    }

    private void OnChapterRewardChosen(List<IDraftable> chosenOptions)
    {
        foreach (ChapterReward reward in chosenOptions.Select(o => (ChapterReward)o))
        {
            reward.ApplyReward();
        }
    }

    private void SetNextChapterGoal()
    {
        CurrentChapterGoal = ChapterMissionGenerator.GenerateChapterObectiveGoal(Chapter);
        CurrentChapterGoal.OnStarted();

        GameUI.Instance.ChapterDisplay.Refresh();
    }

    public void CompleteQuest(Quest quest)
    {
        Game.Instance.QueueActionPrompt(new ActionPrompt_QuestComplete(quest));
        Game.Instance.QueueActionPrompt(new ActionPrompt_RemoveQuest(quest));
    }

    public void FailQuest(Quest quest)
    {
        Game.Instance.QueueActionPrompt(new ActionPrompt_QuestFailed(quest));
        Game.Instance.QueueActionPrompt(new ActionPrompt_RemoveQuest(quest));
    }

    /// <summary>
    /// Only call from action prompt.
    /// </summary>
    public void DoCompleteQuest(Quest quest)
    {
        // Show draft window
        string title = $"Quest complete !";
        string subtitle = "Enjoy your reward";
        List<IDraftable> options = new List<IDraftable>() { quest.Reward };
        GameUI.Instance.DraftWindow.Show(title, subtitle, options, isDraft: false, callback: OnQuestRewardChosen);
    }

    private void OnQuestRewardChosen(List<IDraftable> chosenOptions)
    {
        foreach(QuestReward reward in chosenOptions.Select(o => (QuestReward)o))
        {
            reward.ApplyReward();
        }
    }

    /// <summary>
    /// Only call from action prompt.
    /// </summary>
    public void DoFailQuest(Quest quest)
    {
        // Show draft window
        string title = $"Quest failed !";
        if(quest.HasPenalty)
        {
            string subtitle = "A penalty has been applied";
            List<IDraftable> options = new List<IDraftable>() { quest.Penalty };
            GameUI.Instance.DraftWindow.Show(title, subtitle, options, isDraft: false, OnQuestPenaltyChosen);
        }
        else
        {
            string subtitle = "At least you tried...";
            GameUI.Instance.DraftWindow.Show(title, subtitle, null, isDraft: false);
        }
    }

    private void OnQuestPenaltyChosen(List<IDraftable> chosenOptions)
    {
        foreach (QuestPenalty penalty in chosenOptions.Select(o => (QuestPenalty)o))
        {
            penalty.ApplyPenalty();
        }
    }

    /// <summary>
    /// Only call from action prompt.
    /// </summary>
    public void DoRemoveQuest(Quest quest)
    {
        // Remove quest
        quest.OnRemoved();
        ActiveQuests.Remove(quest);

        // UI
        GameUI.Instance.QuestPanel.Refresh();

        CompleteCurrentActionPrompt();
    }

    public void RedrawToken(Token thrownToken)
    {
        if (RemainingRedraws == 0) return;

        Token newDrawnToken = CurrentDraw.RedrawToken(thrownToken.Original);

        if (newDrawnToken != null)
        {
            StartCoroutine(TokenPhysicsManager.LiftAndImplode(thrownToken));
            TokenPhysicsManager.ThrowToken(newDrawnToken);

            RemainingDrawPhaseResources[ResourceDefOf.Redraw]--;

            // Disable "Confirm Draw" button until all tokens are resting
            GameUI.Instance.GameLoopButton.Disable();

            // UI
            GameUI.Instance.TurnPhaseResources.Refresh();
            GameUI.Instance.TurnDraw.Refresh();
        }
    }

    public void SetRolledTokenSurface(Token thrownToken, int index)
    {
        CurrentDraw.SetRolledSurface(thrownToken.Original, thrownToken.Surfaces[index]);

        // Check if we can enable "Confirm Draw" Button
        if(CurrentDraw.AreAllTokensResting()) GameUI.Instance.GameLoopButton.Enable();

        // UI
        GameUI.Instance.TurnDraw.Refresh();
    }

    /// <summary>
    /// Reduces that many half hearts from the health.
    /// </summary>
    public void TakeDamage(int amount, List<DamageTag> tags)
    {
        // Rule modifiers
        foreach(DamageTag tag in tags)
        {
            foreach (Rule r in Rulebook.ActiveRules)
            {
                if(r.GetDamageModifiers().TryGetValue(tag, out int modifier))
                {
                    amount += modifier;
                }
            }
        }

        Health -= amount;
        GameUI.Instance.HealthDisplay.Refresh();
    }

    #endregion

    #region Action Prompts

    private Queue<ActionPrompt> ActionPromptQueue = new Queue<ActionPrompt>();

    public void QueueActionPrompt(ActionPrompt prompt)
    {
        ActionPromptQueue.Enqueue(prompt);
    }

    public void ShowNextActionPrompt()
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

        // Show next prompt
        ShowNextActionPrompt();
    }

    /// <summary>
    /// Gets called when all action prompts are completed after a meeple landed on a tile.
    /// </summary>
    private void OnActionPromptsDone()
    {
        if (GameState == GameState.MovingPhase) PrepareMovingPhasePlayerActions();
        if (GameState == GameState.PostTurn) StartTurn();
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
        List<List<Tile>> paths = GetPaths(startTile, prevPosition: null, RemainingMovementPoints);
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
        UnhighlightAllMovementOptionTargets();
        CurrentlyHighlightedMovementTargets = MovementOptions.Select(o => o.TargetTile).ToList();
        foreach (Tile target in CurrentlyHighlightedMovementTargets)
        {
            target.HighlightAsMovementOption();
        }
    }

    private void UnhighlightAllMovementOptionTargets()
    {
        if (CurrentlyHighlightedMovementTargets == null || CurrentlyHighlightedMovementTargets.Count == 0) return;

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

        // Select tokens to display
        List<Token> tokensToDisplay = new List<Token>();
        if (GameState == GameState.DrawingPhase) tokensToDisplay = new List<Token>(CurrentDraw.PouchTokens);
        else tokensToDisplay = new List<Token>(TokenPouch);

        // pre‐generate non‐overlapping 2D offsets
        var offsets2D = new List<Vector2>();
        foreach (Token token in tokensToDisplay)
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

        for (int i = 0; i < tokensToDisplay.Count; i++)
        {
            Token orig = tokensToDisplay[i];

            // Make a copy so we don't need to move the original token
            Token token = TokenGenerator.GenerateTokenCopy(orig); 

            // make sure it's visible
            token.Show();

            // position in world using our 2D offset on the camera‐plane
            Vector2 c2 = offsets2D[i];
            Vector3 offset = new Vector3(c2.x, 0.5f, c2.y);
            token.transform.position = center + offset;

            // optionally face the camera (if your token mesh isn't round)
            token.transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));

            // Save token in display list
            PouchDisplayTokens.Add(token);
        }

        // Move camera
        CameraHandler.Instance.Camera.transform.position = new Vector3(0f, 100f + TokenDisplayDistance, 0f);
        CameraHandler.Instance.Camera.transform.rotation = Quaternion.Euler(90, 0, 0);

        // Freeze camera
        CameraHandler.Instance.Freeze();

        // Hide tile interactions
        GameUI.Instance.TileInteractionMenu.Hide();
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

        // Unhide tile interactions
        GameUI.Instance.TileInteractionMenu.Show();
    }

    #endregion

    #region Getters

    public List<TileInteraction> GetAllPossibleTileInteractions()
    {
        List<TileInteraction> list = new List<TileInteraction>();

        foreach (Meeple meeple in Game.Instance.Meeples)
        {
            Tile tile = meeple.Tile;
            list.AddRange(tile.GetInteractions());
        }

        return list;
    }

    public int GetDraftOptionsAmount()
    {
        return 3;
    }

    public int RemainingRedraws => RemainingDrawPhaseResources[ResourceDefOf.Redraw];
    public int RemainingMovementPoints => RemainingMovingPhaseResources[ResourceDefOf.MovementPoint];

    #endregion
}
