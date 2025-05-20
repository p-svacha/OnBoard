using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meeple_Merchant : NpcMeeple
{
    public TradingSession GetNewTradingSession()
    {
        List<ITradable> sellOptions = new List<ITradable>();
        sellOptions.AddRange(Game.Instance.TokenPouch);
        return new TradingSession("Merchant", 0.8f, 1.2f, sellOptions, new List<ITradable>());
    }
}
