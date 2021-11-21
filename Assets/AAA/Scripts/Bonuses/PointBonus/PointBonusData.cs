using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PointBonusData
{
    public PointBonusItem PointBonus;
    public Vector3 Position;
    public Quaternion Rotation;

    public PointBonusData(PointBonusItem pointBonus, Vector3 position, Quaternion rotation)
    {
        PointBonus = pointBonus;
        Position = position;
        Rotation = rotation;
    }
}
