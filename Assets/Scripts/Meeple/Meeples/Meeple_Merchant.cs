using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meeple_Merchant : NpcMeeple, ITrader, ISelectable
{
    public const int MIN_TOKENS_TO_SELL = 2;
    public const int MAX_TOKENS_TO_SELL = 5;
    public const float SELL_VALUE_MODIFIER = 0.8f;
    public const float BUY_VALUE_MODIFIER = 1.2f;

    private List<ITradable> TradeInventory;

    protected override void OnInit()
    {
        TradeInventory = new List<ITradable>();

        // Add some random tokens to the inventory
        int numTokens = Random.Range(MIN_TOKENS_TO_SELL, MAX_TOKENS_TO_SELL + 1);
        for(int i = 0; i < numTokens; i++)
        {
            TradeInventory.Add(TokenGenerator.GenerateRandomToken());
        }
    }

    public TradingSession GetNewTradingSession()
    {
        List<ITradable> sellOptions = new List<ITradable>();
        sellOptions.AddRange(Game.Instance.TokenPouch.Tokens);
        return new TradingSession(this, SELL_VALUE_MODIFIER, BUY_VALUE_MODIFIER, sellOptions, new List<ITradable>(TradeInventory));
    }

    // ITradable
    public List<ITradable> GetTradeInventory() => TradeInventory;

    // ISelectable
    public GameObject SelectionWindow => GameUI.Instance.SelectionPanel.Content_Merchant;
    public void OnSelect()
    {
        SelectionWindow.GetComponentInChildren<UI_TradeInventoryList>().Init(TradeInventory, BUY_VALUE_MODIFIER);
    }
}
