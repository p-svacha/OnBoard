using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;

    [Header("Elements")]
    public UI_TurnDraw TurnDraw;

    private void Awake()
    {
        Instance = this;
    }
}
