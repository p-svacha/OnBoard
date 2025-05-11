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
        if (Game.Instance.GameState == GameState.PreparationPhase)
        {
            List<ResourceDef> resourcesToShow = Game.Instance.TotalPreparationPhaseResources.Keys.ToList();
            foreach(ResourceDef resource in resourcesToShow)
            {
                resources.Add(resource, Game.Instance.RemainingPreparationPhaseResources[resource]);
            }
        }
        else if (Game.Instance.GameState == GameState.ActionPhase)
        {
            List<ResourceDef> resourcesToShow = Game.Instance.TotalActionPhaseResources.Keys.ToList();
            foreach (ResourceDef resource in resourcesToShow)
            {
                resources.Add(resource, Game.Instance.RemainingActionPhaseResources[resource]);
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
        if (Game.Instance.GameState != GameState.PreparationPhase && Game.Instance.GameState != GameState.ActionPhase) return true;

        if (Game.Instance.GameState == GameState.PreparationPhase && Game.Instance.TotalPreparationPhaseResources.Count == 0) return true;
        if (Game.Instance.GameState == GameState.ActionPhase && Game.Instance.TotalActionPhaseResources.Count == 0) return true;

        return false;
    }
}
