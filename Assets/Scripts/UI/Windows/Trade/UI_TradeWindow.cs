using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_TradeWindow : UI_Window
{
    public TradingSession TradingSession { get; private set; }

    [Header("Elements")]
    public TextMeshProUGUI TitleText;
    public UI_TradeInventoryList PlayerInventory;
    public UI_TradeInventoryList PlayerSelling;
    public UI_TradeInventoryList TraderInventory;
    public UI_TradeInventoryList TraderBuying;
    public TextMeshProUGUI SummaryText;
    public Button ConfirmButton;
    public Button AbortButton;

    protected override void Init()
    {
        ConfirmButton.onClick.AddListener(Confirm);
        AbortButton.onClick.AddListener(Abort);
    }

    public void Show(TradingSession tradingSession)
    {
        gameObject.SetActive(true);

        TradingSession = tradingSession;

        TitleText.text = TradingSession.TraderName;

        PlayerInventory.Init(TradingSession.SellOptions, TradingSession.SellValueModifier, StageToSell);
        TraderInventory.Init(TradingSession.BuyOptions, TradingSession.BuyValueModifier, UnstageToSell);
        PlayerSelling.Init(new(), TradingSession.SellValueModifier, StageToBuy);
        TraderBuying.Init(new(), TradingSession.BuyValueModifier, UnstageToBuy);
    }

    private void StageToSell(ITradable tradable)
    {
        PlayerInventory.RemoveEntry(tradable);
        PlayerSelling.AddEntry(tradable);
        TradingSession.StageToSell(tradable);
        UpdateSummaryText();
    }
    private void UnstageToSell(ITradable tradable)
    {
        PlayerSelling.RemoveEntry(tradable);
        PlayerInventory.AddEntry(tradable);
        TradingSession.UnstageToSell(tradable);
        UpdateSummaryText();
    }
    private void StageToBuy(ITradable tradable)
    {
        TraderInventory.RemoveEntry(tradable);
        TraderBuying.AddEntry(tradable);
        TradingSession.StageToBuy(tradable);
        UpdateSummaryText();
    }
    private void UnstageToBuy(ITradable tradable)
    {
        TraderBuying.RemoveEntry(tradable);
        TraderInventory.AddEntry(tradable);
        TradingSession.UnstageToBuy(tradable);
        UpdateSummaryText();
    }

    private void UpdateSummaryText()
    {
        if (TradingSession.FinalTradeValue >= 0) SummaryText.text = $"You will receive <b>{TradingSession.FinalTradeValue} Gold</b>.";
        else SummaryText.text = $"You will pay <b>{-TradingSession.FinalTradeValue} Gold</b>.";
    }

    private void Confirm()
    {
        TradingSession.Apply();
        Close();
    }

    private void Abort()
    {
        Close();
    }

    private void Close()
    {
        // Hide
        gameObject.SetActive(false);

        // Continue
        Game.Instance.CompleteCurrentActionPrompt();
    }
}
