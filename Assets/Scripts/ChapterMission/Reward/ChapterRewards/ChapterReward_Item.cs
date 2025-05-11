using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterReward_Item : ChapterReward
{
    public Item Item;

    protected override void OnInit()
    {
        Item = ItemGenerator.GenerateRandomItem();
    }

    public override void ApplyReward()
    {
        Game.Instance.AddItem(Item);
    }

    // IDraftable
    public override string DraftDisplay_Title => Item.LabelCap;
    public override Sprite DraftDisplay_Sprite => Item.Sprite;
}
