using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class UI_TurnPhaseResources : MonoBehaviour
{
    [Header("Elements")]
    public GameObject RowContainer;

    [Header("Prefabs")]
    public GameObject RowPrefab;
    public UI_ResourceDisplay TurnResourcePrefab;

    public void Refresh()
    {
        if (ShouldHide())
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);

        // Identify which resources to show
        Dictionary<ResourceDef, int> resources = new Dictionary<ResourceDef, int>();
        if (Game.Instance.GameState == GameState.DrawingPhase)
        {
            List<ResourceDef> resourcesToShow = Game.Instance.TotalDrawPhaseResources.Keys.ToList();
            foreach(ResourceDef resource in resourcesToShow)
            {
                resources.Add(resource, Game.Instance.RemainingDrawPhaseResources[resource]);
            }
        }
        else if (Game.Instance.GameState == GameState.MovingPhase)
        {
            List<ResourceDef> resourcesToShow = Game.Instance.TotalMovingPhaseResources.Keys.ToList();
            foreach (ResourceDef resource in resourcesToShow)
            {
                resources.Add(resource, Game.Instance.RemainingMovingPhaseResources[resource]);
            }
        }

        // Clear display
        HelperFunctions.DestroyAllChildredImmediately(RowContainer);

        // Display resources
        int counter = 0;
        GameObject currentRow = null;
        foreach(var kvp in resources)
        {
            ResourceDef resource = kvp.Key;
            int amount = kvp.Value;

            if(counter % 2 == 0) currentRow = GameObject.Instantiate(RowPrefab, RowContainer.transform);

            UI_ResourceDisplay resDisplay = GameObject.Instantiate(TurnResourcePrefab, currentRow.transform);
            resDisplay.Init(resource, amount);

            counter++;
        }
    }

    private bool ShouldHide()
    {
        if (Game.Instance.GameState != GameState.DrawingPhase && Game.Instance.GameState != GameState.MovingPhase) return true;

        if (Game.Instance.GameState == GameState.DrawingPhase && Game.Instance.TotalDrawPhaseResources.Count == 0) return true;
        if (Game.Instance.GameState == GameState.MovingPhase && Game.Instance.TotalMovingPhaseResources.Count == 0) return true;

        return false;
    }
}
