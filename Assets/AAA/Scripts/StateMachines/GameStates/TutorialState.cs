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
        if (Input.GetKeyDown(KeyCode.K))
        {
            _stateMachine.SetState(new GameState(_stateMachine));
        }
    }

    public void OnStateEnter()
    {
        _stateMachine.GameManager.StartGame();
    }

    public void OnStateExit()
    {
        
    }
}