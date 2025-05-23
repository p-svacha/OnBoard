using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class containing all info about a trading session.
/// </summary>
public class TradingSession
{
    /// <summary>
    /// The trader the player is trading with in this session.
    /// </summary>
    public ITrader Trader { get; private set; }

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

    public TradingSession(ITrader trader, float sellValueModifier, float buyValueModifier, List<ITradable> sellOptions, List<ITradable> buyOptions)
    {
        Trader = trader;
        SellValueModifier = sellValueModifier;
        BuyValueModifier = buyValueModifier;
        SellOptions = sellOptions;
        BuyOptions = buyOptions;

        ToSell = new List<ITradable>();
        ToBuy = new List<ITradable>();
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
        foreach (ITradable sold in ToSell) FinalTradeValue += Mathf.RoundToInt(sold.GetMarketValue() * SellValueModifier);
        foreach (ITradable bought in ToBuy) FinalTradeValue -= Mathf.RoundToInt(bought.GetMarketValue() * BuyValueModifier);
    }

    public bool CanApply()
    {
        UpdateFinalTradeValue();
        if (FinalTradeValue < 0 && Game.Instance.Resources[ResourceDefOf.Gold] < Mathf.Abs(FinalTradeValue)) return false;
        return true;
    }

    /// <summary>
    /// Executes the trade in its current state.
    /// </summary>
    public void Apply()
    {
        UpdateFinalTradeValue();

        // Sold stuff
        foreach (ITradable soldTradable in ToSell)
        {
            // Remove sold stuff from player inventory
            if (soldTradable is Token token) Game.Instance.RemoveTokenFromPouch(token, destroyToken: false);
            else throw new System.Exception($"Type {soldTradable.GetType()} not handled for selling the tradable of that type.");

            // Add sold stuff to trader inventory
            Trader.GetTradeInventory().Add(soldTradable);
        }

        // Bought stuff
        foreach (ITradable boughtTradable in ToBuy)
        {
            // Add bought stuff to player inventory
            if (boughtTradable is Token token) Game.Instance.AddTokenToPouch(token);
            else throw new System.Exception($"Type {boughtTradable.GetType()} not handled for buying the tradable of that type.");

            // Remove bought stuff from trader inventory
            Trader.GetTradeInventory().Remove(boughtTradable);
        }

        // Payment
        if (FinalTradeValue < 0) Game.Instance.RemoveResource(ResourceDefOf.Gold, -FinalTradeValue);
        if (FinalTradeValue > 0) Game.Instance.AddResource(ResourceDefOf.Gold, FinalTradeValue);
    }
}
