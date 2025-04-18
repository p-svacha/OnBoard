using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTile_TokenTile : BoardTile
{
    private const int NUM_VISUAL_TOKENS = 12;

    /// <summary>
    /// A token with these properties will be awarded to the player when landed here.
    /// </summary>
    public Token AwardedToken { get; private set; }

    protected override void OnInit()
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
            float x = Mathf.Sin(rad) * TILE_SIZE;
            float y = Mathf.Cos(rad) * TILE_SIZE;

            Token visualToken = TokenGenerator.GenerateToken(AwardedToken.Shape, AwardedToken.Color, AwardedToken.Size);
            visualToken.Show();
            visualToken.Freeze();
            visualToken.transform.SetParent(transform);
            visualToken.transform.localPosition = new Vector3(x * visualToken.transform.localScale.x, 0f, y * visualToken.transform.localScale.z);
        }
    }
}
