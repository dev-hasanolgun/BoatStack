using System.Collections.Generic;

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
    }
    public void OnStateEnter()
    {
        EventManager.TriggerEvent("OnLevelStart", null);
        EventManager.StartListening("OnLevelFinish", EndLevel);
    }
    public void OnStateExit()
    {
        _stateMachine.GameManager.Player.IsSliding = false;
        EventManager.StopListening("OnLevelFinish", EndLevel);
    }

    private void EndLevel(Dictionary<string,object> message)
    {
        _stateMachine.SetState(new VictoryState(_stateMachine));
    }
}
