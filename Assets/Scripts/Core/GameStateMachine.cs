using System.Collections.Generic;

namespace NeonBreaker.Core
{
    public interface IGameState
    {
        void Enter();
        void Exit();
        void Tick();
    }

    public class GameStateMachine
    {
        private readonly Dictionary<GameState, IGameState> states;
        private IGameState currentState;

        public GameState CurrentType { get; private set; }

        public GameStateMachine(Dictionary<GameState, IGameState> availableStates)
        {
            states = availableStates;
        }

        public void Initialize(GameState initialState)
        {
            CurrentType = initialState;
            currentState = states[initialState];
            currentState.Enter();
        }

        public void ChangeState(GameState nextState)
        {
            if (nextState == CurrentType || !states.TryGetValue(nextState, out IGameState next))
            {
                return;
            }

            currentState?.Exit();
            CurrentType = nextState;
            currentState = next;
            currentState.Enter();
        }

        public void Tick()
        {
            currentState?.Tick();
        }
    }
}
