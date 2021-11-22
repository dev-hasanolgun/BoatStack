using System.Collections.Generic;
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
        var player = _stateMachine.GameManager.Player;
        player.EndPlatformMovement();
        
        var pos = new Vector3(player.EndPoint.x,0,player.EndPoint.z);
        if (player.transform.position == pos)
        {
            _stateMachine.SetState(new LevelEndState(_stateMachine));
        }
    }

    public void OnStateEnter()
    {
    }

    public void OnStateExit()
    {
    }
}