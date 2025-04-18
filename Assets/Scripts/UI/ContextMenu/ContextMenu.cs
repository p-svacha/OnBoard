using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContextMenu : MonoBehaviour
{
    private const int MOUSE_OFFSET = 5; // px
    private const int SCREEN_EDGE_OFFSET = 5; // px
    private bool GotInitializedThisFrame;

    [Header("Elements")]
    public GameObject Container;

    [Header("Prefabs")]
    public GameObject TitlePrefab;
    public UI_ContextMenuOption OptionPrefab;

    [Header("Dynamic Values")]
    public float Width;
    public float Height;

    // Singleton
    public static ContextMenu Instance;
    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show(List<ContextMenuOption> options, string title = "")
    {
        GotInitializedThisFrame = true;

        // Remove all previous option entries
        HelperFunctions.DestroyAllChildredImmediately(Container);

        // Add title
        if(title != "")
        {
            GameObject titleObj = Instantiate(TitlePrefab, Container.transform);
            titleObj.GetComponentInChildren<TextMeshProUGUI>().text = title;
        }

        // Add new option entries
        foreach(ContextMenuOption option in options)
        {
            UI_ContextMenuOption optionButton = Instantiate(OptionPrefab, Container.transform);
            optionButton.Init(this, option);
        }

        // Reposition
        Reposition();

        // Show
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0) && !GotInitializedThisFrame && !HelperFunctions.IsPointerOverUIWithComponent<UI_ContextMenuOption>())
        {
            Hide();
        }

        GotInitializedThisFrame = false;
    }

    private void Reposition()
    {
        RectTransform rect = GetComponent<RectTransform>();
        Vector3 position = Input.mousePosition + new Vector3(MOUSE_OFFSET, MOUSE_OFFSET, 0);

        // Fit on screen
        Width = rect.rect.width;
        Height = rect.rect.height;

        // If tooltip would go off the right edge, nudge left
        if (position.x + Width > Screen.width - SCREEN_EDGE_OFFSET)
            position.x = Screen.width - Width - SCREEN_EDGE_OFFSET;

        // If it would go off the bottom, nudge up
        if (position.y - Height < SCREEN_EDGE_OFFSET)
            position.y = Height + SCREEN_EDGE_OFFSET;

        transform.position = position;
    }
}
