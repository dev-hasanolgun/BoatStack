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
    
    public void StartGame(int startLevel = 0) // Create the player and start the game from selected level.
    {
        var sliderData = LevelDatabase.LevelDB[startLevel].SlideData;
        var obstacleData = LevelDatabase.LevelDB[startLevel].ObstacleDataList;
        
        Player.BoatAmount = 1;
        Player.WaterSlideData = sliderData;
        
        Player.transform.position = sliderData.LocalPoints[0];
        Player.Child.transform.localPosition = Vector3.zero;
        
        var rot = Quaternion.LookRotation(sliderData.LocalPoints[1] - sliderData.LocalPoints[0]);
        Player.transform.rotation = rot;
        Player.Child.transform.rotation = rot;
        
        MapGeneration.GenerateMesh(sliderData.LocalPoints,sliderData.LocalNormals,sliderData.Density,sliderData.Width,sliderData.Depth);
        MapGeneration.GenerateObstacles(obstacleData);
        
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