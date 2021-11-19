using UnityEngine.SceneManagement;

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
        SceneManager.LoadScene("GameOverScene");
        _stateMachine.SetState(new MenuState(_stateMachine));
    }

    public void OnStateExit()
    {
    }
}