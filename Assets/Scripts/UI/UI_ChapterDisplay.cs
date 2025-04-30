using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_ChapterDisplay : MonoBehaviour
{
    public TextMeshProUGUI ChapterText;
    public TextMeshProUGUI GoalText;

    public void Refresh()
    {
        ChapterText.text = $"Chapter {Game.Instance.Chapter}";
        GoalText.text = Game.Instance.CurrentChapterMission.Description;
    }
}
