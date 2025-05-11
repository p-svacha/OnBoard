using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_TurnSpread : MonoBehaviour
{
    [Header("Elements")]
    public TextMeshProUGUI TurnText;
    public TextMeshProUGUI SpreadText;
    public GameObject TokenSectionsContainer;
    public GameObject SummaryContainer;

    [Header("Prefabs")]
    public GameObject TokenSectionPrefab;
    public UI_TokenDisplay TokenDisplayPrefab;
    public TextMeshProUGUI SummaryLinePrefab;

    public void ShowPreTurnText()
    {
        TurnText.text = $"Turn {Game.Instance.Turn}";
        SpreadText.gameObject.SetActive(false);
        TokenSectionsContainer.gameObject.SetActive(false);
        SummaryContainer.gameObject.SetActive(false);
        HelperFunctions.DestroyAllChildredImmediately(TokenSectionsContainer);
        HelperFunctions.DestroyAllChildredImmediately(SummaryContainer);
    }

    public void RefreshCurrentSpread()
    {
        SpreadText.gameObject.SetActive(true);
        TokenSectionsContainer.gameObject.SetActive(true);
        SummaryContainer.gameObject.SetActive(true);

        SpreadText.text = "Preliminary Spread";
        HelperFunctions.DestroyAllChildredImmediately(TokenSectionsContainer);
        HelperFunctions.DestroyAllChildredImmediately(SummaryContainer);

        // Sort by color
        Dictionary<TokenColorDef, List<KeyValuePair<Token, TokenSurface>>> tokensByColor = new();
        foreach (var t in Game.Instance.CurrentSpread.TableTokens)
        {
            TokenSurface surface = t.Value;
            if (surface == null) continue; // Surface not yet determined because token is still in movement

            tokensByColor.AddToValueList(surface.Color, t);
        }

        // Display
        foreach(var colorSection in tokensByColor)
        {
            GameObject section = GameObject.Instantiate(TokenSectionPrefab, TokenSectionsContainer.transform);
            HelperFunctions.DestroyAllChildredImmediately(section);
            foreach(var t in colorSection.Value)
            {
                Token token = t.Key;
                TokenSurface surface = t.Value;
                UI_TokenDisplay tokenDisplay = GameObject.Instantiate(TokenDisplayPrefab, section.transform);
                tokenDisplay.Init(token, surface);
            }
        }

        // Resource summary
        foreach(var res in Game.Instance.CurrentSpread.Resources)
        {
            TextMeshProUGUI summaryLine = GameObject.Instantiate(SummaryLinePrefab, SummaryContainer.transform);
            string label = res.Value == 1 ? res.Key.LabelCap : res.Key.LabelPluralCap;
            summaryLine.text = $"{res.Value} {label}";
        }
    }

    public void LockIn()
    {
        SpreadText.text = "Locked in spread";
    }
}
