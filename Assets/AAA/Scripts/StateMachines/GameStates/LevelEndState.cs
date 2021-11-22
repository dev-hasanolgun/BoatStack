using System.Collections.Generic;
using UnityEngine;

public class LevelEndState : IState<GameStateMachine>
{
    private readonly GameStateMachine _stateMachine;
    
    public LevelEndState(GameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }
    
    public void Tick()
    {
        _stateMachine.GameManager.Player.EndPlatformMovement();
        if (Input.GetKeyDown(KeyCode.K))
        {
            _stateMachine.SetState(new TutorialState(_stateMachine));
        }
    }

    public void OnStateEnter()
    {
        EventManager.StartListening("OnPlayLevel", PlayLevel);
        EventManager.StartListening("OnRestartLevel", RestartLevel);
        _stateMachine.GameManager.GameUI.NextLevelButton.gameObject.SetActive(true);
    }

    public void OnStateExit()
    {
        EventManager.StopListening("OnPlayLevel", PlayLevel);
        EventManager.StopListening("OnRestartLevel", RestartLevel);
        _stateMachine.GameManager.GameUI.NextLevelButton.gameObject.SetActive(false);
    }
    private void PlayLevel(Dictionary<string, object> message)
    {
        _stateMachine.GameManager.CurrentLevel++;
        _stateMachine.SetState(new TutorialState(_stateMachine));
    }
    private void RestartLevel(Dictionary<string, object> message)
    {
        _stateMachine.SetState(new TutorialState(_stateMachine));
    }
}