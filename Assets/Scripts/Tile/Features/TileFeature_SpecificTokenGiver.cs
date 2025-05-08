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

    public void InitToken(TokenShapeDef shape, List<TokenSurface> surfaces, TokenSizeDef size)
    {
        AwardedToken = TokenGenerator.GenerateToken(shape, surfaces, size);
    }

    public override void SetRandomParameters()
    {
        List<TokenSurface> tokenSurfaces = new List<TokenSurface>()
        {
            new TokenSurface(TokenColorDefOf.White)
        };
        AwardedToken = TokenGenerator.GenerateToken(TokenShapeDefOf.Pebble, tokenSurfaces, TokenSizeDefOf.Small);
    }

    protected override void OnInitVisuals()
    {
        float degreeStep = 360f / NUM_VISUAL_TOKENS;
        for(int i = 0; i < NUM_VISUAL_TOKENS; i++)
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
}
