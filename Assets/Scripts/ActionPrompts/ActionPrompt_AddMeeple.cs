using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPrompt_AddMeeple : ActionPrompt
{
    private MeepleDef MeepleDef;
    private Tile Tile;

    public ActionPrompt_AddMeeple(MeepleDef def, Tile tile)
    {
        MeepleDef = def;
        Tile = tile;
    }

    public override void OnShow()
    {
        Game.Instance.DoAddNpcMeeple(MeepleDef, Tile, playAnimation: true);
    }
}
