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

    /// <summary>
    /// The function that gets executed when closing / confirming this window. The IDraftables passed represent the chosen options.
    /// </summary>
    private System.Action<List<IDraftable>> Callback;

    protected override void Init()
    {
        ContinueButton.onClick.AddListener(Continue);
    }

    public void Show(string title, string subtitle, List<IDraftable> options, bool isDraft, System.Action<List<IDraftable>> callback = null)
    {
        Callback = callback;

        gameObject.SetActive(true);
        Title.text = title;
        Draft.Init(subtitle, options, isDraft);
    }

    private void Continue()
    {
        // Validate
        if (Draft.IsDraft && Draft.SelectedOption == null) return; // Must choose an option

        // Apply options
        List<IDraftable> chosenOptions = new List<IDraftable>();
        if (Draft.IsDraft)
        {
            chosenOptions = new List<IDraftable>() { Draft.SelectedOption };
        }
        else
        {
            if (Draft.Options != null)
            {
                foreach (IDraftable draftable in Draft.Options)
                {
                    chosenOptions.Add(draftable);
                }
            }
        }

        // Hide
        gameObject.SetActive(false);

        // Callback
        Callback?.Invoke(chosenOptions);

        // Continue
        Game.Instance.CompleteCurrentActionPrompt();
    }
}
