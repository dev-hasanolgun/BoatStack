using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameStateMachine GameStateMachine;
    public Player Player;
    public LevelDatabase LevelDatabase;
    public MapGeneration MapGeneration;
    public int CurrentLevel;
    
    public static GameManager Instance { get; private set; }
    
    public void StartGame() // Create the player and start the game from selected level.
    {
        var startLevel = PlayerPrefs.GetInt("CurrentLevel", 0);
        var sliderData = LevelDatabase.LevelDB[startLevel].SlideData;
        var obstacleData = LevelDatabase.LevelDB[startLevel].ObstacleDataList;
        var extraBoatData = LevelDatabase.LevelDB[startLevel].ExtraBoatDataList;
        var pointBonusData = LevelDatabase.LevelDB[startLevel].PointBonusDataList;
        var speedBonusData = LevelDatabase.LevelDB[startLevel].SpeedBonusDataList;
        
        var rot = Quaternion.LookRotation(sliderData.LocalPoints[1] - sliderData.LocalPoints[0]);
        
        Player.ResetStats();
        Player.WaterSlideData = sliderData;
        Player.transform.position = sliderData.LocalPoints[0];
        Player.Child.transform.localPosition = Vector3.zero;
        Player.transform.rotation = rot;
        Player.Child.transform.rotation = rot;

        MapGeneration.DisableMapObjects();
        MapGeneration.GenerateMesh(sliderData.LocalPoints,sliderData.LocalNormals,sliderData.Density,sliderData.Width,sliderData.Depth);
        MapGeneration.GenerateObstacles(obstacleData);
        MapGeneration.GenerateExtraBoats(extraBoatData);
        MapGeneration.GeneratePointBonus(pointBonusData);
        MapGeneration.GenerateSpeedBoosts(speedBonusData);
        
        CurrentLevel = startLevel;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        GameStateMachine = new GameStateMachine(this);
        GameStateMachine.SetState(new TutorialState(GameStateMachine));
    }
    private void Update()
    {
        GameStateMachine.CurrentState.Tick();
    }
}