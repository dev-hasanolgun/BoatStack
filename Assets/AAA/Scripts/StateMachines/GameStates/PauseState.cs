using System.Collections.Generic;
using UnityEngine;

public class PauseState : IState<GameStateMachine>
{
    private GameStateMachine _stateMachine;
    public PauseState(GameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Tick()
    {
    }

    public void OnStateEnter()
    {
        //UIManager.Instance.InGameUI.PauseMenuUI.gameObject.SetActive(true);
        EventManager.StartListening("OnResumeGame", OnResumeGame);
        EventManager.StartListening("OnQuitGame", OnQuitGame);
    }

    public void OnStateExit()
    {
        EventManager.StopListening("OnResumeGame", OnResumeGame);
        EventManager.StopListening("OnQuitGame", OnQuitGame);
    }

    private void OnResumeGame(Dictionary<string,object> message)
    {
        _stateMachine.SetState(new GameState(_stateMachine));
    }

    private void OnQuitGame(Dictionary<string,object> message)
    {
        _stateMachine.SetState(new MenuState(_stateMachine));
    }
}