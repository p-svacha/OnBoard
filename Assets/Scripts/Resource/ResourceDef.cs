using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDef : Def
{
    public string LabelPlural { get; init; }
    public string LabelPluralCap => LabelPlural.CapitalizeFirst();
}
