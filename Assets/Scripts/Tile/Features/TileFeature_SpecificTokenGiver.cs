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

    public void InitToken(TokenShapeDef shape, TokenColorDef color, TokenSizeDef size)
    {
        AwardedToken = TokenGenerator.GenerateToken(shape, color, size);
    }

    public override void SetRandomParameters()
    {
        AwardedToken = TokenGenerator.GenerateToken(TokenShapeDefOf.Pebble, TokenColorDefOf.White, TokenSizeDefOf.Small);
    }

    public override void InitVisuals()
    {
        float degreeStep = 360f / NUM_VISUAL_TOKENS;
        for(int i = 0; i < NUM_VISUAL_TOKENS; i++)
        {
            float deg = i * degreeStep;
            float rad = deg * Mathf.Deg2Rad;
            float x = Mathf.Sin(rad) * Tile.TILE_SIZE;
            float y = Mathf.Cos(rad) * Tile.TILE_SIZE;

            Token visualToken = TokenGenerator.GenerateTokenCopy(AwardedToken, randomModel: true);
            visualToken.Show();
            visualToken.Freeze();
            visualToken.transform.SetParent(transform);
            visualToken.transform.localPosition = new Vector3(x * visualToken.transform.localScale.x, 0f, y * visualToken.transform.localScale.z);
            HelperFunctions.ApplyRandomRotation(visualToken.gameObject);
        }
    }

    public override void OnLand()
    {
        Game.Instance.AddTokenToPouch(AwardedToken.Shape, AwardedToken.Color, AwardedToken.Size, silent: false);
    }
}
