using System;
using UnityEngine;

[Serializable]
public class ExtraBoatData
{
    public ExtraBoatItem ExtraBoat;
    public Vector3 Position;
    public Quaternion Rotation;

    public ExtraBoatData(ExtraBoatItem extraBoat, Vector3 position, Quaternion rotation)
    {
        ExtraBoat = extraBoat;
        Position = position;
        Rotation = rotation;
    }
}
