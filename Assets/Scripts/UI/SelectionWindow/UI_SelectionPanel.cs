using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SelectionPanel : MonoBehaviour
{
    private ISelectable SelectedObject;

    [Header("Elements")]
    public TextMeshProUGUI TitleText;
    public GameObject Content_Merchant;

    public void ApplySelection(ISelectable selectable)
    {
        if (selectable == SelectedObject || selectable == null)
        {
            Deselect();
            return;
        }

        // Select new object
        gameObject.SetActive(true);
        SelectedObject = selectable;
        TitleText.text = SelectedObject.Label;
        SelectedObject.OnSelect();
    }

    public void Deselect()
    {
        SelectedObject = null;
        gameObject.SetActive(false);
    }
}
