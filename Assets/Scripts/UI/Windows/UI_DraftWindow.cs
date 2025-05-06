using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UI_DraftWindow : UI_Window
{
    [Header("Elements")]
    public TextMeshProUGUI Title;
    public UI_Draft Draft;
    public Button ContinueButton;

    protected override void Init()
    {
        ContinueButton.onClick.AddListener(Continue);
    }

    public void Show(string title, string subtitle, List<IDraftable> options, bool isDraft)
    {
        gameObject.SetActive(true);
        Title.text = title;
        Draft.Init(subtitle, options, isDraft);
    }

    private void Continue()
    {
        // Validate
        if (Draft.IsDraft && Draft.SelectedOption == null) return; // Must choose an option

        // Apply options
        if(Draft.IsDraft)
        {
            (Draft.SelectedOption).ApplySelection();
        }
        else
        {
            foreach(IDraftable draftable in Draft.Options)
            {
                draftable.ApplySelection();
            }
        }

        // Hide
        gameObject.SetActive(false);
        

        // Continue
        Game.Instance.CompleteCurrentActionPrompt();
    }
}
