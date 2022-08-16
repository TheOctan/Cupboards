using System.Collections.Generic;
using DG.Tweening;
using OctanGames.Map.Node;
using UnityEngine;
using UnityExtensions.Math;

namespace OctanGames.StateMachine.States
{
    public class FindPathState : BaseState<GameState>
    {
        public FindPathState(BaseStateMachine<GameState> stateMachine, GameContext gameContext)
            : base(stateMachine, gameContext)
        {
        }

        public override void EnterState()
        {
            Debug.Log(nameof(FindPathState));
            
            Vector2Int startPosition = GameContext.CurrentSelectedChip.Position;
            Vector2Int endPosition = GameContext.LastSelectedPosition;

            List<PathNode> path = GameContext.Pathfinding.FindPath(
                startPosition.x, startPosition.y, endPosition.x, endPosition.y);

            if (path != null)
            {
                MoveChip(path);
            }
            else
            {
                SwitchStateAsync(GameState.Deselect);
            }
        }

        public override void UpdateState()
        {
            
        }

        public override void ExitState()
        {
        }

        private void MoveChip(List<PathNode> path)
        {
            Sequence sequence = DOTween.Sequence();
            GameContext.Pathfinding.UnBakeObstacle(GameContext.CurrentSelectedChip.Position);
            foreach (PathNode pathNode in path)
            {
                sequence.Append(GameContext.CurrentSelectedChip.transform
                    .DOLocalMove((pathNode.Position.TileCenter() * GameContext.TileSize).ToVertical(), GameContext.ChipMovementDuration)
                    .SetEase(Ease.InOutSine));
            }

            sequence.OnComplete(() =>
            {
                GameContext.CurrentSelectedChip.UpdateCurrentPosition(path[path.Count - 1].Position);
                GameContext.Pathfinding.BakeObstacle(GameContext.CurrentSelectedChip.Position);
                SwitchState(GameState.Deselect);
            });
        }

        private void DrawDebugPath(List<PathNode> path)
        {
            float cellSize = GameContext.Pathfinding.CellsGrid.CellSize;
            Transform transform = GameContext.Transform;
            for (var i = 0; i < path.Count - 1; i++)
            {
                Vector3 startPoint =
                    transform.TransformPoint((path[i].Position.ToVector3() + Vector3.one * 0.5f) * cellSize);
                Vector3 endPoint =
                    transform.TransformPoint((path[i + 1].Position.ToVector3() + Vector3.one * 0.5f) *
                                             cellSize);
                Debug.DrawLine(startPoint, endPoint, Color.green, 2f);
            }
        }
    }
}