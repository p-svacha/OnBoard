using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_TurnDraw : MonoBehaviour
{
    [Header("Elements")]
    public TextMeshProUGUI Title;
    public GameObject TokenSectionsContainer;
    public GameObject SummaryContainer;

    [Header("Prefabs")]
    public GameObject TokenSectionPrefab;
    public UI_TokenDisplay TokenDisplayPrefab;
    public TextMeshProUGUI SummaryLinePrefab;

    public void ShowWaitingText()
    {
        Title.text = "Press Space to Draw";
        HelperFunctions.DestroyAllChildredImmediately(TokenSectionsContainer);
        HelperFunctions.DestroyAllChildredImmediately(SummaryContainer);
    }

    public void ShowTurnDraw(Game game)
    {
        Title.text = $"Turn {game.Turn} Draw";
        HelperFunctions.DestroyAllChildredImmediately(TokenSectionsContainer);
        HelperFunctions.DestroyAllChildredImmediately(SummaryContainer);

        // Sort by color
        Dictionary<TokenColorDef, List<Token>> tokensByColor = new Dictionary<TokenColorDef, List<Token>>();
        foreach(Token token in game.CurrentDrawResult.DrawnTokens) tokensByColor.AddToValueList(token.Color, token);

        // Display
        foreach(var colorSection in tokensByColor)
        {
            GameObject section = GameObject.Instantiate(TokenSectionPrefab, TokenSectionsContainer.transform);
            HelperFunctions.DestroyAllChildredImmediately(section);
            foreach(Token p in colorSection.Value)
            {
                UI_TokenDisplay tokenDisplay = GameObject.Instantiate(TokenDisplayPrefab, section.transform);
                tokenDisplay.Init(p);
            }
        }

        // Resource summary
        foreach(var res in game.CurrentDrawResult.Resources)
        {
            TextMeshProUGUI summaryLine = GameObject.Instantiate(SummaryLinePrefab, SummaryContainer.transform);
            string label = res.Value == 1 ? res.Key.LabelCap : res.Key.LabelPluralCap;
            summaryLine.text = $"{res.Value} {label}";
        }
    }
}
