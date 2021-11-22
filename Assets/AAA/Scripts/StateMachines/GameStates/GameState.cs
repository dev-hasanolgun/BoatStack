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
        if (player.CurrentBoatAmount <= 0)
        {
            _stateMachine.SetState(new GameOverState(_stateMachine));
        }
        player.CharacterMovement();
        _stateMachine.GameManager.GameUI.TotalScoreText.text = player.TotalScore.ToString("0");
    }
    public void OnStateEnter()
    {
        EventManager.TriggerEvent("OnLevelStart", null);
        EventManager.StartListening("OnLevelFinish", FinishLevel);
    }
    public void OnStateExit()
    {
        _stateMachine.GameManager.Player.IsSliding = false;
        EventManager.StopListening("OnLevelFinish", FinishLevel);
    }

    private void FinishLevel(Dictionary<string,object> message)
    {
        _stateMachine.SetState(new VictoryState(_stateMachine));
    }
}
