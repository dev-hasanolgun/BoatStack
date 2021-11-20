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
        _stateMachine.SetState(new TutorialState(_stateMachine));
    }

    public void OnStateExit()
    {
    }
}