using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resources refer to anything that does not have inherent behaviour or logic attached and is solely defined by its amount.
/// <br/> Therefore resources can always be referred to by just a ResourceDef and amount and never have to be explicitly instantiated.
/// <br/> Resources are used and interacted with by specific game mechanics and rules that add or substract from a resource.
/// </summary>
public class ResourceDef : Def
{
    public ResourceType Type { get; init; }

    public string LabelPlural { get; init; }
    public string LabelPluralCap => LabelPlural.CapitalizeFirst();

    private Sprite _Sprite;
    public override Sprite Sprite
    {
        get
        {
            if (_Sprite == null) _Sprite = ResourceManager.LoadSprite($"Sprites/ResourceIcons/{DefName}");
            return _Sprite;
        }
    }
}
