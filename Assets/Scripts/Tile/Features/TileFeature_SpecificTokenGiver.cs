using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFeature_SpecificTokenGiver : TileFeature
{
    private const int NUM_VISUAL_TOKENS = 12;

    /// <summary>
    /// A token with these properties will be awarded to the player when landed here.
    /// </summary>
    public Token AwardedToken { get; private set; }

    public void SetToken(TokenShapeDef shape, List<TokenSurface> surfaces, TokenSizeDef size, TokenAffinityDef affinity = null)
    {
        AwardedToken = TokenGenerator.GenerateToken(shape, surfaces, size, affinity);
        RefreshVisuals();
    }

    /// <summary>
    /// Sets a random surface of the awarded token to the provided color.
    /// </summary>
    public void SetAnyColorTo(TokenColorDef color)
    {
        AwardedToken.Surfaces.RandomElement().SetColor(color);
        RefreshVisuals();
    }

    public override void SetRandomParameters()
    {
        Dictionary<TokenShapeDef, float> shapeProbabilities = new Dictionary<TokenShapeDef, float>()
        {
            { TokenShapeDefOf.Pebble, 1f },
            { TokenShapeDefOf.Coin, 0.2f },
        };

        Dictionary<TokenColorDef, float> colorProbabilities = new Dictionary<TokenColorDef, float>()
        {
            { TokenColorDefOf.White, 1f },
            { TokenColorDefOf.Gold, 0.3f },
            { TokenColorDefOf.Black, 0.2f },
        };

        Dictionary<TokenSizeDef, float> sizeProbabilities = new Dictionary<TokenSizeDef, float>()
        {
            { TokenSizeDefOf.Small, 1f },
            { TokenSizeDefOf.Medium, 0.2f },
            { TokenSizeDefOf.Big, 0.05f },
        };

        float affinityProbability = 0.05f;

        TokenShapeDef shape = shapeProbabilities.GetWeightedRandomElement();
        List<TokenSurface> tokenSurfaces = new List<TokenSurface>();
        for (int i = 0; i < shape.NumSurfaces; i++) tokenSurfaces.Add(new TokenSurface(colorProbabilities.GetWeightedRandomElement()));
        TokenSizeDef size = sizeProbabilities.GetWeightedRandomElement();
        TokenAffinityDef affinity = (Random.value < affinityProbability) ? DefDatabase<TokenAffinityDef>.AllDefs.RandomElement() : null;

        AwardedToken = TokenGenerator.GenerateToken(shape, tokenSurfaces, size, affinity);
    }

    protected override void OnInitVisuals()
    {
        float degreeStep = 360f / NUM_VISUAL_TOKENS;
        for (int i = 0; i < NUM_VISUAL_TOKENS; i++)
        {
            float deg = i * degreeStep;
            float rad = deg * Mathf.Deg2Rad;
            float x = Mathf.Sin(rad) * Tile.TILE_RADIUS;
            float y = Mathf.Cos(rad) * Tile.TILE_RADIUS;

            Token visualToken = TokenGenerator.GenerateTokenCopy(AwardedToken, randomModel: true, isStatic: true, hidden: false);
            visualToken.transform.SetParent(transform);
            visualToken.transform.localPosition = new Vector3(x, Tile.TILE_HEIGHT, y);
            HelperFunctions.ApplyRandomRotation(visualToken.gameObject);
        }
    }

    public override void OnLand()
    {
        Game.Instance.QueueActionPrompt(new ActionPrompt_ReceiveToken(AwardedToken));
    }

    public override string Description => $"When landing here, receive a <b>{AwardedToken.Label}</b>.";
}
