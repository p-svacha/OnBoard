using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPrompt_Trade : ActionPrompt
{
    private TradingSession TradingSession;

    public ActionPrompt_Trade(TradingSession tradingSession)
    {
        TradingSession = tradingSession;
    }

    public override void OnShow()
    {
        GameUI.Instance.TradeWindow.Show(TradingSession);
    }
}
