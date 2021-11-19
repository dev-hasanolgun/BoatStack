using System;
using UnityEngine;

[Serializable]
public struct WaterSlideData
{
    public Vector3[] LocalPoints;
    public Vector3[] LocalTangents;
    public float Density;
    public float Depth;
    public float Width;
    
    public WaterSlideData(Vector3[] localPoints, Vector3[] localTangents, float density, float depth, float width)
    {
        LocalPoints = localPoints;
        LocalTangents = localTangents;
        Density = density;
        Depth = depth;
        Width = width;
    }
}