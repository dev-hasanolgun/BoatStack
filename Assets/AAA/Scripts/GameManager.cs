using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameStateMachine GameStateMachine;
    public GameUI GameUI;
    public Player Player;
    public LevelDatabase LevelDatabase;
    public MapGeneration MapGeneration;
    public GameObject EndGamePlatform;
    public int CurrentLevel;
    
    public static GameManager Instance { get; private set; }
    
    public void StartGame() // Create the player and start the game from selected level.
    {
        // Get and set current level index into the player pref data
        CurrentLevel = HelperFunctions.ReverseClampToInt(CurrentLevel, 0, LevelDatabase.LevelDB.Count-1);
        PlayerPrefs.SetInt("CurrentLevelIndex", CurrentLevel);
        
        var sliderData = LevelDatabase.LevelDB[CurrentLevel].SlideData;
        var obstacleData = LevelDatabase.LevelDB[CurrentLevel].ObstacleDataList;
        var extraBoatData = LevelDatabase.LevelDB[CurrentLevel].ExtraBoatDataList;
        var pointBonusData = LevelDatabase.LevelDB[CurrentLevel].PointBonusDataList;
        var speedBonusData = LevelDatabase.LevelDB[CurrentLevel].SpeedBonusDataList;
        
        var rot = Quaternion.LookRotation(sliderData.LocalPoints[1] - sliderData.LocalPoints[0]);
        var dir = sliderData.LocalPoints[^1] - sliderData.LocalPoints[^2];
        
        // Set player datas
        Player.ResetStats();
        Player.WaterSlideData = sliderData;
        Player.transform.position = sliderData.LocalPoints[0];
        Player.Child.transform.localPosition = Vector3.zero;
        Player.transform.rotation = rot;
        Player.Child.transform.rotation = rot;

        // Generate Map and all the objects along with it
        MapGeneration.DisableMapObjects();
        MapGeneration.GenerateMesh(sliderData.LocalPoints,sliderData.LocalNormals,sliderData.Density,sliderData.Width,sliderData.Depth);
        MapGeneration.GenerateObstacles(obstacleData);
        MapGeneration.GenerateExtraBoats(extraBoatData);
        MapGeneration.GeneratePointBonus(pointBonusData);
        MapGeneration.GenerateSpeedBoosts(speedBonusData);
        MapGeneration.GenerateEndGamePlatform(EndGamePlatform, sliderData.LocalPoints[^1],dir);
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
        CurrentLevel = PlayerPrefs.GetInt("CurrentLevelIndex", 0);
        GameStateMachine = new GameStateMachine(this);
        GameStateMachine.SetState(new TutorialState(GameStateMachine));
    }
    private void Update()
    {
        GameStateMachine.CurrentState.Tick();
    }
}