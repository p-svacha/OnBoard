using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;

    [Header("Elements")]
    public UI_TurnDraw TurnDraw;
    public UI_TokenPouchButton TokenPouchButton;

    private void Awake()
    {
        Instance = this;
    }
}
