using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileFeature_Altar : TileFeature
{
    public TokenShapeDef Shape { get; private set; }
    public TokenColorDef Color { get; private set; }
    public TokenSizeDef Size { get; private set; }

    protected override void OnInitVisuals()
    {
        GameObject altarPrefab = ResourceManager.LoadPrefab("Prefabs/TileFeatures/Altar");
        GameObject altar = GameObject.Instantiate(altarPrefab, transform);
        altar.transform.rotation = Quaternion.Euler(0f, Tile.ForwardAngle, 0f);

        float offsetAngle = Tile.ForwardAngle + 90;
        float offsetDistance = Tile.TILE_RADIUS * 1.6f;
        float x = Mathf.Sin(Mathf.Deg2Rad * offsetAngle) * offsetDistance;
        float y = Mathf.Cos(Mathf.Deg2Rad * offsetAngle) * offsetDistance;
        transform.localPosition = new Vector3(x, 0f, y);
    }

    public void SetRequiredTokenInfo(TokenShapeDef shape, TokenColorDef color, TokenSizeDef size)
    {
        Shape = shape;
        Color = color;
        Size = size;
    }

    public override List<TileInteraction> GetInteractions()
    {
        return new List<TileInteraction>()
        {
            new TileInteraction(DeliverToken, CanDeliverToken, Tile, this, "Deliver Token", "Deliver the token to complete the chapter.")
        };
    }

    private void DeliverToken()
    {
        Debug.Log("Deliver");
        List<Token> eligibleTokens = GetEligibleTokens();
        if (eligibleTokens.Count() == 1)
        {
            DeliverToken(eligibleTokens[0]);
        }
        else if (eligibleTokens.Count() > 1)
        {
            Game.Instance.QueueActionPrompt(new ActionPrompt_DraftToken("", "Choose which token to offer", GetEligibleTokens(), OnChosenWhichTokenToDeliver));
        }
    }

    private void OnChosenWhichTokenToDeliver(List<IDraftable> chosenTokens)
    {
        Token chosenToken = (Token)chosenTokens[0];
        DeliverToken(chosenToken);
    }

    private void DeliverToken(Token token)
    {
        Game.Instance.RemoveTokenFromPouch(token);
        Game.Instance.QueueCompleteChapter();
    }

    private string CanDeliverToken()
    {
        if (GetEligibleTokens().Count() == 0) return "No token matches the requirements.";
        return "";
    }

    private List<Token> GetEligibleTokens()
    {
        return Game.Instance.TokenPouch.Where(t => t.Shape == Shape && t.Size == Size && t.Surfaces.Any(s => s.Color == Color)).ToList();
    }
}
