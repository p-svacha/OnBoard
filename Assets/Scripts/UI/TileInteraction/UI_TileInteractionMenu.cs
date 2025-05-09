using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TileInteractionMenu : MonoBehaviour
{
    [Header("Elements")]
    public GameObject Container;

    [Header("Prefabs")]
    public UI_TileInteractionOption OptionPrefab;

    public void Refresh()
    {
        HelperFunctions.DestroyAllChildredImmediately(Container);

        foreach(TileInteraction interaction in Game.Instance.GetAllPossibleTileInteractions())
        {
            UI_TileInteractionOption optionDisplay = GameObject.Instantiate(OptionPrefab, Container.transform);
            optionDisplay.Init(interaction);
        }

        Show();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
