using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An action prompt is a game state where the player has to make a specific choice or confirm provided information.
/// <br/>Action prompts can come in the form of a UI element or choosing a tile on the board etc.
/// </summary>
public abstract class ActionPrompt
{
    public void Show()
    {
        OnShow();
    }
    public void Close()
    {
        OnClose();
    }

    public abstract void OnShow();
    public virtual void OnClose() { }
}
