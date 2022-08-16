using System.Collections.Generic;
using UnityEngine;
using UnityExtensions.Input;
using UnityExtensions.Math;
using OctanGames.Map.Node;

namespace OctanGames.StateMachine.States
{
    public class SelectState : BaseState<GameState>
    {
        public SelectState(BaseStateMachine<GameState> stateMachine, GameContext gameContext)
            : base(stateMachine, gameContext)
        {
        }

        public override void EnterState()
        {
            Debug.Log(nameof(SelectState));
        }

        public override void UpdateState()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorldPosition = InputExtensions.GetMouseWorldPosition();

                GameContext.Pathfinding.CellsGrid.GetXY(mouseWorldPosition, out int endX, out int endY);

                Vector2Int startPosition = GameContext.CurrentSelectedChip.Position;
                GameContext.LastSelectedPosition = new Vector2Int(endX, endY);

                SwitchState(
                    GameContext.Pathfinding.IsValidPositions(startPosition.x, startPosition.y, endX, endY)
                    ? GameState.FindPath
                    : GameState.Deselect);
            }
        }

        public override void ExitState()
        {
        }

        
    }
}