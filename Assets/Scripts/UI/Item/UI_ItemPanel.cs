using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ItemPanel : MonoBehaviour
{
    [Header("Elements")]
    public GameObject Container;

    [Header("Prefabs")]
    public UI_ItemDisplay ItemPrefab;

    public void Refresh()
    {
        if(Game.Instance.Items.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        HelperFunctions.DestroyAllChildredImmediately(Container, skipElements: 1);

        foreach(Item item in Game.Instance.Items)
        {
            UI_ItemDisplay display = GameObject.Instantiate(ItemPrefab, Container.transform);
            display.Init(item);
        }
    }
}
