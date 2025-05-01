using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenShapeDef : Def
{
    /// <summary>
    /// How many different possible results can appear when a token with this shape is drawn.
    /// </summary>
    public int NumSurfaces { get; init; }

    /// <summary>
    /// If the shape has multiple surfaces, this list contains the normals that define which surface is currently rolled on the 3D token.
    /// </summary>
    public List<Vector3> SurfaceLocalNormals { get; init; } = null;

    public override bool Validate()
    {
        base.Validate();
        if (NumSurfaces > 1 && NumSurfaces != SurfaceLocalNormals.Count) throw new System.Exception($"There must be exactly {NumSurfaces} normals defined in the TokenShapeDef {DefName}.");
        return true;
    }
}
