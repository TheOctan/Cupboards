using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityExtensions.Collections.Generic;
using UnityExtensions.Math;

namespace OctanGames.Map
{
    public class Pathfinding
    {
        private Transform _transform;

        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        private List<PathNode> _openList;
        private List<PathNode> _closeList;
        public CellsGrid<PathNode> CellsGrid { get; }

        public Pathfinding(Transform transform, int width, int height, float tileSize)
        {
            _transform = transform;
            CellsGrid = new CellsGrid<PathNode>(transform, width, height, tileSize, Vector3.zero,
                (grid, x, y) => new PathNode(grid, x, y));
        }

        public void UpdateDebug()
        {
            CellsGrid.UpdateDebug();
        }

        // public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
        // {
        //     
        // }

        public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
        {
            if (startX == endX && startY == endY)
            {
                return null;
            }

            PathNode startNode = GetNode(startX, startY);
            PathNode endNode = GetNode(endX, endY);

            _openList = new List<PathNode>() { startNode };
            _closeList = new List<PathNode>();

            for (var x = 0; x < CellsGrid.Width; x++)
            {
                for (var y = 0; y < CellsGrid.Height; y++)
                {
                    PathNode pathNode = GetNode(x, y);
                    pathNode.GCost = int.MaxValue;
                    pathNode.CalculateFCost();
                    pathNode.PrevNode = null;
                }
            }

            startNode.GCost = 0;
            startNode.HCost = CalculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();

            while (_openList.NotEmpty())
            {
                PathNode currentNode = GetLowestCostNode(_openList);
                if (currentNode == endNode)
                {
                    return CalculatePath(endNode);
                }

                _openList.Remove(currentNode);
                _closeList.Add(currentNode);

                foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
                {
                    if (_closeList.Contains(neighbourNode))
                    {
                        continue;
                    }

                    if (!neighbourNode.IsWalkable)
                    {
                        _closeList.Add(neighbourNode);
                        continue;
                    }

                    int tentativeGCost = currentNode.GCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if (tentativeGCost < neighbourNode.GCost)
                    {
                        neighbourNode.PrevNode = currentNode;
                        neighbourNode.GCost = tentativeGCost;
                        neighbourNode.HCost = CalculateDistanceCost(neighbourNode, endNode);
                        neighbourNode.CalculateFCost();

                        if (_openList.NotContains(neighbourNode))
                        {
                            _openList.Add(neighbourNode);
                        }
                    }
                }
            }

            return null;
        }

        public void BakePoints(Vector2Int[] points)
        {
            FillObstacles(false);
            foreach (Vector2Int point in points)
            {
                CellsGrid.GetGridObject(point.x - 1, point.y - 1).IsWalkable = true;
            }
        }

        public void BakeObstacles(Vector2Int[] obstacles)
        {
            foreach (Vector2Int obstacle in obstacles)
            {
                CellsGrid.GetGridObject(obstacle.x, obstacle.y).IsWalkable = false;
            }
        }

        public void BakeConnections(MapData mapData)
        {
            foreach (Connection connection in mapData.Connections)
            {
                Vector2Int startPosition = mapData.Points[(connection.StartPointNumber - 1)] - Vector2Int.one;
                Vector2Int endPosition = mapData.Points[(connection.EndPointNumber - 1)] - Vector2Int.one;
                Vector2Int dir = endPosition - startPosition;

                if (dir.x > 0)
                {
                    GetNode(startPosition.x, startPosition.y).IsRightConnected = true;
                    GetNode(endPosition.x, endPosition.y).IsLeftConnected = true;
                }
                else if (dir.x < 0)
                {
                    GetNode(startPosition.x, startPosition.y).IsLeftConnected = true;
                    GetNode(endPosition.x, endPosition.y).IsRightConnected = true;
                }
                else if (dir.y > 0)
                {
                    GetNode(startPosition.x, startPosition.y).IsUpConnected = true;
                    GetNode(endPosition.x, endPosition.y).IsDownConnected = true;
                }
                else if (dir.y < 0)
                {
                    GetNode(startPosition.x, startPosition.y).IsDownConnected = true;
                    GetNode(endPosition.x, endPosition.y).IsUpConnected = true;
                }
            }
        }

        public IReadOnlyNode GetReadOnlyNode(int x, int y)
        {
            return CellsGrid.GetGridObject(x, y);
        }

        private void FillObstacles(bool value)
        {
            for (var x = 0; x < CellsGrid.Width; x++)
            {
                for (var y = 0; y < CellsGrid.Height; y++)
                {
                    CellsGrid.GetGridObject(x, y).IsWalkable = value;
                }
            }
        }

        private void InverseObstacles(bool value)
        {
            for (var x = 0; x < CellsGrid.Width; x++)
            {
                for (var y = 0; y < CellsGrid.Height; y++)
                {
                    bool isWalkable = CellsGrid.GetGridObject(x, y).IsWalkable;
                    CellsGrid.GetGridObject(x, y).IsWalkable = !isWalkable;
                }
            }
        }

        private List<PathNode> GetNeighbourList(PathNode currentNode)
        {
            var neighbourList = new List<PathNode>();

            int x = currentNode.X;
            int y = currentNode.Y;

            if (x - 1 >= 0 && currentNode.IsLeftConnected)
            {
                neighbourList.Add(GetNode(x - 1, y));
            }
            if (x + 1 < CellsGrid.Width && currentNode.IsRightConnected)
            {
                neighbourList.Add(GetNode(x + 1, y));
            }
            if (y - 1 >= 0 && currentNode.IsDownConnected)
            {
                neighbourList.Add(GetNode(x, y - 1));
            }
            if (y + 1 < CellsGrid.Width && currentNode.IsUpConnected)
            {
                neighbourList.Add(GetNode(x, y + 1));
            }

            return neighbourList;
        }

        private PathNode GetNode(int x, int y)
        {
            return CellsGrid.GetGridObject(x, y);
        }

        private static List<PathNode> CalculatePath(PathNode endNode)
        {
            var path = new List<PathNode> { endNode };

            PathNode currentNode = endNode;
            while (currentNode.PrevNode != null)
            {
                path.Add(currentNode.PrevNode);
                currentNode = currentNode.PrevNode;
            }

            return path.Reversed();
        }

        private static int CalculateDistanceCost(PathNode a, PathNode b)
        {
            Vector2Int distance = (a.Position - b.Position).Abs();
            int remainingDistance = (distance.x - distance.y).Abs();

            return MOVE_DIAGONAL_COST * distance.MinDimensionValue() + MOVE_STRAIGHT_COST * remainingDistance;
        }

        private static PathNode GetLowestCostNode(IReadOnlyList<PathNode> pathNodes)
        {
            PathNode pathNode = pathNodes[0];
            for (var i = 1; i < pathNodes.Count; i++)
            {
                if (pathNodes[i].FCost < pathNode.FCost)
                {
                    pathNode = pathNodes[i];
                }
            }

            return pathNode;
        }
    }
}