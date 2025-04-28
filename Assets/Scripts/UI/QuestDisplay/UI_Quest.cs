using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Quest : MonoBehaviour
{
    private Objective Quest;

    [Header("Elements")]
    public TextMeshProUGUI GoalText;
    public GameObject Progress;
    public GameObject Reward;
    public GameObject Deadline;
    public TextMeshProUGUI ProgressValueText;
    public TextMeshProUGUI RewardValueText;
    public TextMeshProUGUI DeadlineValueText;

    public Button ExpandCollapseButton;
    public GameObject ExpandedIcon;
    public GameObject CollapsedIcon;

    public void Init(Objective quest)
    {
        Quest = quest;
        ExpandCollapseButton.onClick.AddListener(Button_OnClick);
        Refresh();
    }

    private void Button_OnClick()
    {
        Quest.IsUiCollapsed = !Quest.IsUiCollapsed;
        Refresh();
    }

    private void Refresh()
    {
        GoalText.text = Quest.Goal.Description;
        RewardValueText.text = Quest.Reward.Label;

        if (Quest.IsUiCollapsed)
        {
            ExpandedIcon.SetActive(false);
            CollapsedIcon.SetActive(true);
            Progress.SetActive(false);
            Reward.SetActive(false);
            Deadline.SetActive(false);
        }
        else
        {
            ExpandedIcon.SetActive(true);
            CollapsedIcon.SetActive(false);
            Progress.SetActive(true);
            Reward.SetActive(true);
            Deadline.SetActive(Quest.DeadlineTurn > 0);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}
