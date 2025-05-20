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

        PlayerInventory.Init(TradingSession.SellOptions);
        TraderInventory.Init(TradingSession.BuyOptions);
        PlayerSelling.Clear();
        TraderBuying.Clear();
    }

    private void Confirm()
    {


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
