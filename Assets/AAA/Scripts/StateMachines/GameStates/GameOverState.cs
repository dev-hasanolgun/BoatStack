using System.Collections.Generic;

public class GameOverState : IState<GameStateMachine>
{
    private GameStateMachine _stateMachine;
    public GameOverState(GameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Tick()
    {
    }

    public void OnStateEnter()
    {
        EventManager.StartListening("OnRestartLevel", RestartLevel);
        _stateMachine.GameManager.GameUI.RestartButton.gameObject.SetActive(true);
    }

    public void OnStateExit()
    {
        _stateMachine.GameManager.GameUI.RestartButton.gameObject.SetActive(false);
    }
    private void RestartLevel(Dictionary<string, object> message)
    {
        _stateMachine.SetState(new TutorialState(_stateMachine));
    }
}