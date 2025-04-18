using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawResult
{
    /// <summary>
    /// The list of tokens that got drawn.
    /// </summary>
    public List<Token> DrawnTokens;

    /// <summary>
    /// The amount of every resource that gets awarded from this draw.
    /// </summary>
    public Dictionary<ResourceDef, int> Resources;

    public DrawResult(List<Token> drawnTokens)
    {
        DrawnTokens = drawnTokens;
        Resources = new Dictionary<ResourceDef, int>();
        UpdateResult();
    }

    /// <summary>
    /// Updates all result values (like MovementPoints, Gold, etc.) from the currently drawn tokens.
    /// </summary>
    private void UpdateResult()
    {
        // Reset
        Resources.Clear();

        foreach (Token token in DrawnTokens)
        {
            // Add resource from color and size
            if(token.Color.Resource != null)
            {
                int amount = token.Color.ResourceBaseAmount * token.Size.EffectMultiplier;
                Resources.Increment(token.Color.Resource, amount);
            }
        }
    }
}
