public interface IState<T>
{
    void Tick();
    void OnStateEnter();
    void OnStateExit();
}
