using System;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

[Serializable]
public class Level
{
    public WaterSlideData SlideData;
    public BezierPath BezierPath;
    public List<Vector3> ObstacleList = new List<Vector3>();
    public int LevelID;
    public string LevelName;

    private float _cellDiameter;

    public Level(WaterSlideData slideData, BezierPath bezierPath, int levelID, string levelName)
    {
        SlideData = slideData;
        BezierPath = bezierPath;
        LevelID = levelID;
        LevelName = levelName;
    }
}