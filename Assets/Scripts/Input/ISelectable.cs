using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface that should be attached to every GameObject that can be selected to be shown in the selection panel.
/// </summary>
public interface ISelectable
{
    public string Label { get; }
    public GameObject SelectionWindow { get; }
    public void OnSelect();
}
