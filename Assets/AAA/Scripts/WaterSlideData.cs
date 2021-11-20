using System;
using UnityEngine;

[Serializable]
public struct WaterSlideData
{
    public Vector3[] LocalPoints;
    public Vector3[] LocalNormals;
    public Vector3[] LocalTangents;
    public int Density;
    public float Depth;
    public float Width;
    
    public WaterSlideData(Vector3[] localPoints, Vector3[] localNormals, Vector3[] localTangents, int density, float depth, float width)
    {
        LocalPoints = localPoints;
        LocalNormals = localNormals;
        LocalTangents = localTangents;
        Density = density;
        Depth = depth;
        Width = width;
    }
}