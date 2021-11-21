using System;
using UnityEngine;

[Serializable]
public class ObstacleData
{
    public Obstacle Obstacle;
    public Vector3 Position;
    public Quaternion Rotation;

    public ObstacleData(Obstacle obstacle, Vector3 position, Quaternion rotation)
    {
        Obstacle = obstacle;
        Position = position;
        Rotation = rotation;
    }
}