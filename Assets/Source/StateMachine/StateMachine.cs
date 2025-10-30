using FishNet.Object;
using UnityEngine;

public class StateMachine : NetworkBehaviour
{
    [SerializeField] private bool _notStartOnEnable;
    [SerializeField] private State _startState;
    public State CurrentState { get; private set; }

    public void DefaultState()
    {
        if (!base.IsServer)
            return;
        ChangeState(_startState);
    }

    public override void OnStartClient()
    {
        if (!base.IsServer)
            return;
        if (_notStartOnEnable)
        {
            CurrentState = _startState;

            return;
        }

        CurrentState = _startState;
        CurrentState.Enter();
    }


    public void ChangeState(State state)
    {
        if (!base.IsServer)
            return;
        if (CurrentState.CanChanged && CurrentState != state)
        {
            CurrentState.Exit();
            CurrentState = state;
            CurrentState.Enter();
        }
    }
}