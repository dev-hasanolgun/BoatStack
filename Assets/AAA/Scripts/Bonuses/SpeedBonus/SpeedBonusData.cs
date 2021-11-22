using System;
using UnityEngine;

[Serializable]
public class SpeedBonusData
{
    public SpeedBonusItem SpeedBonus;
    public Vector3 Position;
    public Quaternion Rotation;

    public SpeedBonusData(SpeedBonusItem speedBonus, Vector3 position, Quaternion rotation)
    {
        SpeedBonus = speedBonus;
        Position = position;
        Rotation = rotation;
    }
}
