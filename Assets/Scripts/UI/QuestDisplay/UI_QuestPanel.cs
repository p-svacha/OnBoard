using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_QuestPanel : MonoBehaviour
{
    [Header("Elements")]
    public GameObject QuestsContainer;

    [Header("Prefabs")]
    public UI_Quest QuestPrefab;
    public GameObject DividerLinePrefab;

    public void Refresh()
    {
        HelperFunctions.DestroyAllChildredImmediately(QuestsContainer, skipElements: 1);

        if(Game.Instance.ActiveQuests.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        int counter = 0;
        foreach (Objective quest in Game.Instance.ActiveQuests)
        {
            UI_Quest questDisplay = GameObject.Instantiate(QuestPrefab, QuestsContainer.transform);
            questDisplay.Init(quest);
            if (counter != Game.Instance.ActiveQuests.Count - 1) GameObject.Instantiate(DividerLinePrefab, QuestsContainer.transform);
            counter++;
        }
    }
}
