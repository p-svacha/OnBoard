using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class containing all info about a trading session.
/// </summary>
public class TradingSession
{
    public string TraderName { get; private set; }

    /// <summary>
    /// The the market value modifier applies to sold tradables resulting in the gold received.
    /// </summary>
    public float SellValueModifier { get; private set; }

    /// <summary>
    /// The the market value modifier applies to bought tradables resulting in the gold payed.
    /// </summary>
    public float BuyValueModifier { get; private set; }

    /// <summary>
    /// All tradables in the player inventory that can be sold in this session.
    /// </summary>
    public List<ITradable> SellOptions { get; private set; }

    /// <summary>
    /// All tradables the player buy in this session.
    /// </summary>
    public List<ITradable> BuyOptions { get; private set; }

    /// <summary>
    /// All tradables the player has currently staged to sell.
    /// </summary>
    public List<ITradable> ToSell { get; private set; }

    /// <summary>
    /// All tradables the player has currently staged to buy.
    /// </summary>
    public List<ITradable> ToBuy { get; private set; }

    /// <summary>
    /// The amount of gold that is transferred if the trade is completed in its current state.
    /// <br/>Positive value means the player receives gold. Negative means the player pays gold.
    /// </summary>
    public int FinalTradeValue { get; private set; }

    public TradingSession(string traderName, float sellValueModifier, float buyValueModifier, List<ITradable> sellOptions, List<ITradable> buyOptions)
    {
        TraderName = traderName;
        SellValueModifier = sellValueModifier;
        BuyValueModifier = buyValueModifier;
        SellOptions = sellOptions;
        BuyOptions = buyOptions;
    }

    public void StageToSell(ITradable tradable)
    {
        ToSell.Add(tradable);
        UpdateFinalTradeValue();
    }
    public void UnstageToSell(ITradable tradable)
    {
        ToSell.Remove(tradable);
        UpdateFinalTradeValue();
    }
    public void StageToBuy(ITradable tradable)
    {
        ToBuy.Add(tradable);
        UpdateFinalTradeValue();
    }
    public void UnstageToBuy(ITradable tradable)
    {
        ToBuy.Remove(tradable);
        UpdateFinalTradeValue();
    }

    private void UpdateFinalTradeValue()
    {
        FinalTradeValue = 0;
        foreach (ITradable sold in ToSell) FinalTradeValue -= Mathf.RoundToInt(sold.GetMarketValue() * SellValueModifier);
        foreach (ITradable bought in ToBuy) FinalTradeValue += Mathf.RoundToInt(bought.GetMarketValue() * BuyValueModifier);
    }

    /// <summary>
    /// Executes the trade in its current state.
    /// </summary>
    public void Apply()
    {

    }
}
