using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UI_Window_ChapterComplete : UI_Window
{
    [Header("Elements")]
    public TextMeshProUGUI Title;
    public UI_Draft RewardDraft;
    public Button ContinueButton;

    protected override void Init()
    {
        ContinueButton.onClick.AddListener(Continue);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        int completedChapter = Game.Instance.Chapter;
        Title.text = $"chapter {completedChapter} complete!";

        List<ChapterReward> rewardOptions = ChapterRewardGenerator.GenerateRewards(completedChapter, amount: 3);
        RewardDraft.Init("Choose a reward", rewardOptions.Select(r => (IDraftable)r).ToList());
    }

    private void Continue()
    {
        if (RewardDraft.SelectedOption == null) return; // Must choose an option

        gameObject.SetActive(false);
        ((ChapterReward)RewardDraft.SelectedOption).Apply();
        Game.Instance.CompleteCurrentActionPrompt();
    }
}
