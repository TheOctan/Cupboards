using UnityEngine;
using UnityExtensions.Input;

namespace OctanGames.StateMachine.States
{
    public class DeselectState : BaseState<GameState>
    {
        public DeselectState(BaseStateMachine<GameState> stateMachine, GameContext gameContext)
            : base(stateMachine, gameContext)
        {
        }

        public override void EnterState()
        {
            GameContext.ResetCurrentSelectedChip();
        }

        public override void UpdateState()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorldPosition = InputExtensions.GetMouseWorldPosition();

                GameContext.Pathfinding.CellsGrid.GetXY(mouseWorldPosition, out int x, out int y);

                if (GameContext.SetSelectedChip(x, y))
                {
                    SwitchState(GameState.Select);
                }
            }
        }

        public override void ExitState()
        {
        }
    }
}