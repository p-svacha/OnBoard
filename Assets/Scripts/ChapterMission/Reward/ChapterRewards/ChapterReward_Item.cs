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

    public override void Apply()
    {
        Game.Instance.AddItem(Item);
    }

    // IDraftable
    public override string DraftDisplay_Text => Item.LabelCap;
    public override Sprite DraftDisplay_Sprite => Item.Sprite;
}
