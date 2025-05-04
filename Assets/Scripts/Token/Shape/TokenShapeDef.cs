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

    /// <summary>
    /// This list contains the index of each material on the shape prefab that represents the color of a surface.
    /// </summary>
    public List<int> SurfaceMaterialIndices { get; init; } = null;

    public override bool Validate()
    {
        base.Validate();
        if (NumSurfaces > 1 && NumSurfaces != SurfaceLocalNormals.Count) throw new System.Exception($"There must be exactly {NumSurfaces} normals defined in the TokenShapeDef {DefName}. But there were {SurfaceLocalNormals.Count}.");
        if (NumSurfaces != SurfaceMaterialIndices.Count) throw new System.Exception($"There must be exactly {NumSurfaces} surface material indicies defined in the TokenShapeDef {DefName}. But there were {SurfaceMaterialIndices.Count}.");
        return true;
    }
}
