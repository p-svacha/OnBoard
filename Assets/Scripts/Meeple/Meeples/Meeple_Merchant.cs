using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meeple_Merchant : NpcMeeple
{
    public TradingSession GetNewTradingSession()
    {
        return new TradingSession("Merchant", 0.8f, 1.2f, new List<ITradable>(), new List<ITradable>());
    }
}
