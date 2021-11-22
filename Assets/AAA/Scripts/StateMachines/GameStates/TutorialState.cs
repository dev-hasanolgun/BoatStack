using UnityEngine;

public class TutorialState : IState<GameStateMachine>
{
    private readonly GameStateMachine _stateMachine;
    
    public TutorialState(GameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }
    
    public void Tick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _stateMachine.SetState(new GameState(_stateMachine));
        }
    }

    public void OnStateEnter()
    {
        _stateMachine.GameManager.StartGame();
        _stateMachine.GameManager.GameUI.TutorialUI.gameObject.SetActive(true);
        _stateMachine.GameManager.GameUI.CurrentLevelText.text = "Level " + (PlayerPrefs.GetInt("CurrentLevel", 0) + 1).ToString("0");
    }

    public void OnStateExit()
    {
        _stateMachine.GameManager.GameUI.TutorialUI.gameObject.SetActive(false);
    }
}