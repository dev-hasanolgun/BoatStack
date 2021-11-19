using UnityEngine;

public class VictoryState : IState<GameStateMachine>
{
    private readonly GameStateMachine _stateMachine;
    
    public VictoryState(GameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }
    
    public void Tick()
    {
        
    }

    public void OnStateEnter()
    {
        _stateMachine.GameManager.StartGame();
    }

    public void OnStateExit()
    {
        
    }
}