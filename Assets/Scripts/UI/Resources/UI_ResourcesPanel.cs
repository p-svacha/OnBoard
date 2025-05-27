using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_ResourcesPanel : MonoBehaviour
{
    [Header("Elements")]
    public GameObject Container;

    [Header("Prefab")]
    public UI_ResourceDisplay ResourcePrefab;

    public void Refresh()
    {
        HelperFunctions.DestroyAllChildredImmediately(Container, skipElements: 1);
        foreach(var res in Game.Instance.Resources.Where(r => r.Key.Type == ResourceType.Collectable))
        {
            UI_ResourceDisplay resourceDisplay = GameObject.Instantiate(ResourcePrefab, Container.transform);
            resourceDisplay.Init(res.Key, res.Value);
        }
    }
}
