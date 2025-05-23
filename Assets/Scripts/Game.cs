using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance;
    

    #region Static Rules

    private const int NEW_QUEST_INTERVAL = 6;
    private const int FIRST_NEW_QUEST_TURN = 3;

    #endregion

    /// <summary>
    /// List of all meeples the player controls.
    /// </summary>
    public Meeple PlayerMeeple;

    /// <summary>
    /// Amount of tokens drawn in a turn.
    /// </summary>
    public int DrawAmount;

    /// <summary>
    /// The pouch containing all player tokens.
    /// </summary>
    public TokenPouch TokenPouch;

    /// <summary>
    /// The collecion of all board segments that make up the board.
    /// </summary>
    public Board Board;

    /// <summary>
    /// The set of tokens and surfaces that are active this turn.
    /// </summary>
    public Spread CurrentSpread { get; private set; }

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
    /// The amount of preparation phase resources the player has remaining this preparation phase.
    /// </summary>
    public Dictionary<ResourceDef, int> RemainingPreparationPhaseResources { get; private set; }

    /// <summary>
    /// The amount of preparation phase resources the player had available in total this preparation phase.
    /// </summary>
    public Dictionary<ResourceDef, int> TotalPreparationPhaseResources { get; private set; }

    /// <summary>
    /// The amount of action phase resources the player has remaining this action phase.
    /// </summary>
    public Dictionary<ResourceDef, int> RemainingActionPhaseResources { get; private set; }

    /// <summary>
    /// The amount of action phase resources the player had available in total this action phase.
    /// </summary>
    public Dictionary<ResourceDef, int> TotalActionPhaseResources { get; private set; }

    /// <summary>
    /// The rulebook tracks the current state of all active rules and contains all logic regarding rules.
    /// </summary>
    public Rulebook Rulebook { get; private set; }

    /// <summary>
    /// List of all meeples on the board not controlled by the player.
    /// </summary>
    public List<NpcMeeple> NpcMeeples { get; private set; }

    // Visual
    private List<Tile> CurrentlyHighlightedMovementTargets;

    #region Initialization

    private void Awake()
    {
        Instance = this;
        TokenPouch = GameObject.Find("TokenPouch").GetComponent<TokenPouch>();
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
        TotalPreparationPhaseResources = new Dictionary<ResourceDef, int>();
        RemainingPreparationPhaseResources = new Dictionary<ResourceDef, int>();
        TotalActionPhaseResources = new Dictionary<ResourceDef, int>();
        RemainingActionPhaseResources = new Dictionary<ResourceDef, int>();

        InitializeRulebook();
        CreateInitialBoard();
        AddStartingMeeple();
        AddStartingContent();
        InitializeFirstChapter();
        CameraHandler.Instance.SetPosition(PlayerMeeple.transform.position);

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
        GameObject meeplePrefab = ResourceManager.LoadPrefab("Prefabs/Meeples/Meeple");
        GameObject meepleObject = GameObject.Instantiate(meeplePrefab);
        PlayerMeeple = meepleObject.GetComponent<Meeple>();
        TeleportMeeple(PlayerMeeple, Board.StartTile);

        MaxHealth = 6;
        Health = 6;
        Items = new List<Item>();
        GameUI.Instance.HealthDisplay.Refresh();

        NpcMeeples = new List<NpcMeeple>();
        AddNpcMeeple(MeepleDefOf.Merchant, Board.GetRandomTile());
    }

    private void AddStartingContent()
    {
        // Tokens
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
        GameState = GameState.PreTurn;

        Turn++;
        Debug.Log($"Starting Turn {Turn}.");

        // Check if time for new quest
        if ((Turn - FIRST_NEW_QUEST_TURN) % NEW_QUEST_INTERVAL == 0)
        {
            ActiveQuests.Add(QuestGenerator.GenerateQuest());
        }

        // UI
        GameUI.Instance.TurnSpreadPanel.ShowPreTurnText();
        GameUI.Instance.TurnPhaseResources.Refresh();
        GameUI.Instance.QuestPanel.Refresh();
        GameUI.Instance.ItemPanel.Refresh();

        GameUI.Instance.GameLoopButton.RefreshTextAndTooltip();
        GameUI.Instance.GameLoopButton.Enable();
    }

    public void DrawInitialTokens()
    {
        GameState = GameState.PreparationPhase;

        // Draw initial tokens
        CurrentSpread = new Spread();
        TokenPhysicsManager.ThrowInitialTokens(this);

        // Get amount of redraws
        TotalPreparationPhaseResources.Clear();
        foreach (Item item in Items) TotalPreparationPhaseResources.IncrementMultiple(item.GetDrawPhaseResources());
        RemainingPreparationPhaseResources = new Dictionary<ResourceDef, int>(TotalPreparationPhaseResources);

        // Disable "Confirm Draw" button until all tokens are resting
        GameUI.Instance.GameLoopButton.Disable();

        // UI
        GameUI.Instance.TurnSpreadPanel.RefreshCurrentSpread();
        GameUI.Instance.GameLoopButton.RefreshTextAndTooltip();
        GameUI.Instance.TurnPhaseResources.Refresh();
    }

    /// <summary>
    /// Ends the preparation phase where the player can change their spread, activiating the tokens and starting the action phase.
    /// </summary>
    public void LockInSpread()
    {
        GameState = GameState.ActionPhase;

        // Rules
        Rulebook.OnLockInSpread(CurrentSpread);

        // Chapter
        CurrentChapterGoal.OnLockInSpread(CurrentSpread);

        // Set resources for moving phase
        TotalActionPhaseResources.Clear();
        TotalActionPhaseResources.IncrementMultiple(CurrentSpread.GetMovingPhaseResources()); 
        RemainingActionPhaseResources = new Dictionary<ResourceDef, int>(TotalActionPhaseResources);

        // Add collectable resources
        foreach(var res in CurrentSpread.GetCollectableResources())
        {
            AddResource(res.Key, res.Value);
        }

        // Prepare possible actions for player
        ShowNextActionPrompt();

        // UI
        GameUI.Instance.TurnSpreadPanel.LockIn();
        GameUI.Instance.GameLoopButton.RefreshTextAndTooltip();
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

        // Game loop button
        if (CanEndActionPhase()) GameUI.Instance.GameLoopButton.Enable();
        else GameUI.Instance.GameLoopButton.Disable();
    }

    /// <summary>
    /// Returns if the player can end the turn.
    /// </summary>
    private bool CanEndActionPhase()
    {
        if (RemainingMovementPoints > 0) return false;
        return true;
    }

    /// <summary>
    /// Gets called when a movement of the player meeple is done and it lands on a tile.
    /// </summary>
    public void OnMovementDone(MovementOption move)
    {
        // Set new position
        move.Meeple.SetTile(move.TargetTile);

        // Effects for player meeples
        if (move.IsPlayerMovement)
        {
            // Exectute OnLandEffect of arrived tile
            move.TargetTile.OnLand();

            // Show action prompts
            ShowNextActionPrompt();
        }
    }

    public void EndActionPhase()
    {
        GameState = GameState.PostTurn;

        // Disable game loop button
        GameUI.Instance.GameLoopButton.RefreshTextAndTooltip();
        GameUI.Instance.GameLoopButton.Disable();

        // Collect all tokens off the board
        StartCoroutine(TokenPhysicsManager.CollectTokens(this));

        // NPC meeples
        foreach (NpcMeeple meeple in NpcMeeples) meeple.OnEndTurn();

        // Quests
        foreach (Quest quest in ActiveQuests) quest.OnTurnPassed();
        GameUI.Instance.QuestPanel.Refresh();

        // Rulebook
        Rulebook.OnTurnPassed();

        // UI
        GameUI.Instance.TileInteractionMenu.Hide();
        GameUI.Instance.TurnPhaseResources.Refresh();

        // Go through post turn action prompts
        ShowNextActionPrompt();
    }



    #endregion

    #region Game Actions

    public Token AddTokenToPouch(TokenShapeDef shape, List<TokenSurface> surfaces, TokenSizeDef size, TokenAffinityDef affinity = null)
    {
        Token newToken = TokenGenerator.GenerateToken(shape, surfaces, size, affinity);
        AddTokenToPouch(newToken);
        return newToken;
    }
    public void AddTokenToPouch(Token token)
    {
        TokenPouch.AddToken(token);
        token.IsInPouch = true;
    }
    public void RemoveTokenFromPouch(Token token)
    {
        TokenPouch.RemoveToken(token);
        token.IsInPouch = false;
        GameObject.Destroy(token);
    }
    public void UpgradeTokenSize(Token token)
    {
        if (token.Size == TokenSizeDefOf.Small) token.SetSize(TokenSizeDefOf.Medium);
        else if (token.Size == TokenSizeDefOf.Medium) token.SetSize(TokenSizeDefOf.Big);
        else if (token.Size == TokenSizeDefOf.Big) token.SetSize(TokenSizeDefOf.Large);
    }
    public void SetTokenSurfacePattern(TokenSurface surface, TokenSurfacePatternDef pattern)
    {
        surface.SetPattern(pattern);
    }
    public void SetTokenAffinity(Token token, TokenAffinityDef affinity)
    {
        token.SetAffinity(affinity);
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

    public void AddNpcMeeple(MeepleDef def, Tile tile)
    {
        NpcMeeple meeple = MeepleGenerator.GenerateNpcMeeple(def);
        TeleportMeeple(meeple, tile);
        NpcMeeples.Add(meeple);
    }

    public void TeleportMeeple(Meeple meeple, Tile tile)
    {
        meeple.transform.position = tile.transform.position;
        meeple.SetTile(tile);
    }

    public void ExecuteMovement(MovementOption movement)
    {
        // Resources
        if (movement.IsPlayerMovement) RemainingActionPhaseResources[ResourceDefOf.MovementPoint] -= movement.Length;

        // Start moving
        MeepleMovementAnimator.AnimateMove(movement, onComplete: OnMovementDone);

        // FX
        if (movement.IsPlayerMovement) UnhighlightAllMovementOptionTargets();

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
        string subtitle = quest.Reward.RewardDraftTitle;
        List<IDraftable> options = quest.Reward.GetRewardOptions();
        GameUI.Instance.DraftWindow.Show(title, subtitle, options, isDraft: quest.Reward.IsDraft, callback: chosenOptions => OnQuestRewardChosen(quest, chosenOptions));
    }

    private void OnQuestRewardChosen(Quest quest, List<IDraftable> chosenOptions)
    {
        foreach(IDraftable rewardOption in chosenOptions)
        {
            quest.Reward.ApplyReward(rewardOption);
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

        Token newDrawnToken = CurrentSpread.RedrawToken(thrownToken.Original);

        if (newDrawnToken != null)
        {
            StartCoroutine(TokenPhysicsManager.LiftAndImplode(thrownToken));
            TokenPhysicsManager.ThrowToken(newDrawnToken);

            RemainingPreparationPhaseResources[ResourceDefOf.Redraw]--;

            // Disable game loop button until all tokens are resting
            GameUI.Instance.GameLoopButton.Disable();

            // UI
            GameUI.Instance.TurnPhaseResources.Refresh();
            GameUI.Instance.TurnSpreadPanel.RefreshCurrentSpread();
        }
    }

    public void DiscardToken(Token thrownToken)
    {
        // Discard
        CurrentSpread.DiscardToken(thrownToken.Original);
        StartCoroutine(TokenPhysicsManager.LiftAndImplode(thrownToken));

        // UI
        GameUI.Instance.TurnPhaseResources.Refresh();
        GameUI.Instance.TurnSpreadPanel.RefreshCurrentSpread();
    }

    public void SetRolledTokenSurface(Token thrownToken, int index)
    {
        Token original = thrownToken.Original;
        CurrentSpread.SetRolledSurface(original, original.Surfaces[index]);

        // Check if we can enable "Confirm Draw" Button
        if(CurrentSpread.AreAllTokensResting()) GameUI.Instance.GameLoopButton.Enable();

        // UI
        GameUI.Instance.TurnSpreadPanel.RefreshCurrentSpread();
    }

    /// <summary>
    /// Reduces that many half hearts from the health.
    /// </summary>
    public void TakeDamage(int amount, List<DamageTag> tags = null)
    {
        if (tags == null) tags = new List<DamageTag>();

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
        if (GameState == GameState.ActionPhase) PrepareMovingPhasePlayerActions();
        if (GameState == GameState.PostTurn) StartTurn();
    }

    #endregion

    #region Movement

    private void UpdateCurrentMovementOptions()
    {
        MovementOptions.Clear();
        MovementOptions.AddRange(GetMeepleMovementOptions(PlayerMeeple, RemainingMovementPoints));
    }

    public List<MovementOption> GetMeepleMovementOptions(Meeple meeple, int numTiles, bool allowStops = true)
    {
        bool isPlayerMovement = (meeple == PlayerMeeple);
        List<MovementOption> options = new List<MovementOption>();

        Tile startTile = meeple.Tile;
        List<List<Tile>> paths = Pathfinder.GetPaths(startTile, prevPosition: null, numTiles, allowStops);
        foreach(List<Tile> path in paths)
        {
            Tile target = path.Last();
            path.Remove(target);
            options.Add(new MovementOption(meeple, startTile, new List<Tile>(path), target, isPlayerMovement));
        }

        return options;
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
        if (GameState == GameState.PreparationPhase) tokensToDisplay = new List<Token>(CurrentSpread.PouchTokens);
        else tokensToDisplay = new List<Token>(TokenPouch.Tokens);

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
        list.AddRange(PlayerMeeple.Tile.GetInteractions());
        return list;
    }

    public int GetDraftOptionsAmount()
    {
        return 3;
    }

    public int RemainingRedraws => RemainingPreparationPhaseResources.TryGetValue(ResourceDefOf.Redraw, out int value) ? value : 0;
    public int RemainingMovementPoints => RemainingActionPhaseResources.TryGetValue(ResourceDefOf.MovementPoint, out int value) ? value : 0;

    #endregion
}
