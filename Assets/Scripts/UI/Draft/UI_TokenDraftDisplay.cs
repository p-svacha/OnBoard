using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_TokenDraftDisplay : MonoBehaviour
{
    [Header("Elements")]
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Subtitle;
    public GameObject OptionsContainer;
    public Button ConfirmButton;

    [Header("Prefabs")]
    public UI_TokenDraftOption OptionPrefab;

    private TokenDraftType DraftType;
    private List<Token> TokenOptions;
    private Token SelectedToken;
    private Dictionary<Token, UI_TokenDraftOption> OptionDisplays;

    private enum TokenDraftType
    {
        Discard,
        // Upgrade
    }

    public void Initialize()
    {
        gameObject.SetActive(false);
        ConfirmButton.onClick.AddListener(Confirm);
    }

    public void ShowDraftToDiscard()
    {
        gameObject.SetActive(true);

        // Init
        DraftType = TokenDraftType.Discard;
        TokenOptions = new List<Token>();
        SelectedToken = null;
        OptionDisplays = new Dictionary<Token, UI_TokenDraftOption>();

        // Draw random tokens to draft from
        List<Token> candidates = new List<Token>(Game.Instance.TokenPouch);
        int drawAmount = Game.Instance.GetDraftDrawAmount();
        for(int i = 0; i < drawAmount; i++)
        {
            Token chosenToken = candidates.RandomElement();
            candidates.Remove(chosenToken);
            TokenOptions.Add(chosenToken);
        }

        // Display
        HelperFunctions.DestroyAllChildredImmediately(OptionsContainer);
        foreach(Token token in TokenOptions)
        {
            UI_TokenDraftOption option = GameObject.Instantiate(OptionPrefab, OptionsContainer.transform);
            option.Init(this, token);
            OptionDisplays.Add(token, option);
        }
    }

    public void SetSelectedToken(Token token)
    {
        if (SelectedToken != null) OptionDisplays[SelectedToken].SetSelected(false);

        if (token == SelectedToken) SelectedToken = null;
        else SelectedToken = token;

        if (SelectedToken != null) OptionDisplays[SelectedToken].SetSelected(true);
    }


    public void Confirm()
    {
        if (SelectedToken == null) return;

        switch(DraftType)
        {
            case TokenDraftType.Discard:
                Game.Instance.RemoveTokenFromPouch(SelectedToken);
                break;
        }
        
        gameObject.SetActive(false);
        Game.Instance.CompleteCurrentActionPrompt();
    }
}
