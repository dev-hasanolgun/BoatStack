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
        if (Input.GetKeyDown(KeyCode.K))
        {
            _stateMachine.SetState(new TutorialState(_stateMachine));
        }
    }

    public void OnStateEnter()
    {
        
    }

    public void OnStateExit()
    {
        
    }
}