using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public ItemDef Def { get; private set; }

    public void Init(ItemDef def)
    {
        Def = def;
        Sprite = ResourceManager.LoadSprite($"Sprites/Items/{Def.DefName}");
        OnInit();
    }

    protected virtual void OnInit() { }

    public virtual Dictionary<ResourceDef, int> GetDrawPhaseResources()
    {
        return new Dictionary<ResourceDef, int>();
    }

    public Sprite Sprite { get; private set; }
    public virtual string Label => Def.Label;
    public virtual string LabelCap => Label.CapitalizeFirst();
    public virtual string Descrption => Def.Description;
}
