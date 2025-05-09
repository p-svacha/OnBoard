using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Quest : MonoBehaviour
{
    private Quest Quest;

    [Header("Elements")]
    public TextMeshProUGUI GoalText;
    public GameObject Progress;
    public GameObject Reward;
    public GameObject Deadline;
    public GameObject Penalty;
    public TextMeshProUGUI ProgressValueText;
    public TextMeshProUGUI RewardValueText;
    public TextMeshProUGUI DeadlineValueText;
    public TextMeshProUGUI PenaltyText;

    public Button ExpandCollapseButton;
    public GameObject ExpandedIcon;
    public GameObject CollapsedIcon;

    public void Init(Quest quest)
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
        RewardValueText.text = Quest.Reward.LabelCap;
        int numTurnsUntilDeadline = Quest.DeadlineTurn - Game.Instance.Turn;
        if(numTurnsUntilDeadline == 1) DeadlineValueText.text = $"This Turn!";
        else DeadlineValueText.text = $"{Quest.DeadlineTurn - Game.Instance.Turn} Turns";
        PenaltyText.text = Quest.HasPenalty ? Quest.Penalty.LabelCap : "";

        if (Quest.IsUiCollapsed)
        {
            ExpandedIcon.SetActive(false);
            CollapsedIcon.SetActive(true);

            Progress.SetActive(false);
            Reward.SetActive(false);
            Deadline.SetActive(false);
            Penalty.SetActive(false);
        }
        else
        {
            ExpandedIcon.SetActive(true);
            CollapsedIcon.SetActive(false);

            Progress.SetActive(Quest.HasProgress);
            Reward.SetActive(true);
            Deadline.SetActive(Quest.HasDeadline);
            Penalty.SetActive(Quest.HasPenalty);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}
