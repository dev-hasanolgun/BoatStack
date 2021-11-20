using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameStateMachine GameStateMachine;
    public Player Player;
    public LevelDatabase LevelDatabase;
    public int CurrentLevel;
    
    public static GameManager Instance { get; private set; }
    
    public void StartGame(int startLevel = 0) // Create the player and start the game from selected level.
    {
        CurrentLevel = startLevel;
        Player.WaterSlideData = LevelDatabase.LevelDB[startLevel].SlideData;
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