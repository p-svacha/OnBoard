using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_Window : MonoBehaviour
{
    private void Awake()
    {
        Init();
    }

    protected virtual void Init() { }
}
