using System.Collections.Generic;
using UnityEngine;

public class GameState : IState<GameStateMachine>
{
    private GameStateMachine _stateMachine;
    public GameState(GameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Tick()
    {
        var player = _stateMachine.GameManager.Player;
        if (player.BoatAmount <= 0)
        {
            _stateMachine.SetState(new GameOverState(_stateMachine));
        }
        player.MoveCharacter();
    }
    public void OnStateEnter()
    {
        EventManager.TriggerEvent("OnLevelStart", null);
        EventManager.StartListening("OnLevelFinish", NextLevel);
    }
    public void OnStateExit()
    {
        _stateMachine.GameManager.Player.IsSliding = false;
        EventManager.StopListening("OnLevelFinish", NextLevel);
    }

    private void NextLevel(Dictionary<string,object> message)
    {
        _stateMachine.GameManager.CurrentLevel++;
        PlayerPrefs.SetInt("CurrentLevel", _stateMachine.GameManager.CurrentLevel);
        _stateMachine.SetState(new VictoryState(_stateMachine));
    }
}
