using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;

    [Header("Elements")]
    public UI_TurnDraw TurnDraw;
    public UI_TokenPouchButton TokenPouchButton;
    public UI_PostItButton PostItButton;

    private void Awake()
    {
        Instance = this;
    }
}
