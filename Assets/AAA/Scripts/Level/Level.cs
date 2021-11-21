using System;
using System.Collections.Generic;
using PathCreation;

[Serializable]
public class Level
{
    public WaterSlideData SlideData;
    public BezierPath BezierPath;
    public List<ObstacleData> ObstacleDataList = new List<ObstacleData>();
    public List<ExtraBoatData> ExtraBoatDataList = new List<ExtraBoatData>();
    public List<PointBonusData> PointBonusDataList = new List<PointBonusData>();
    public List<SpeedBonusData> SpeedBonusDataList = new List<SpeedBonusData>();
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