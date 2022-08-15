using UnityEngine;

namespace OctanGames.Map
{
    public struct Connections
    {
        public int U;
        public int R;
        public int D;
        public int L;

        public Connections(int u, int r, int d, int l)
        {
            L = l;
            D = d;
            R = r;
            U = u;
        }
    }

    public interface IReadOnlyNode
    {
        int X { get; }
        int Y { get; }
        Vector2Int Position { get; }
        bool IsLeftConnected { get; }
        bool IsRightConnected { get; }
        bool IsUpConnected { get; }
        bool IsDownConnected { get; }
        public Connections Connections { get; }
    }

    public class PathNode : IReadOnlyNode
    {
        private readonly CellsGrid<PathNode> _cellsGrid;
        public int X => Position.x;
        public int Y => Position.y;
        public Vector2Int Position { get; }
        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost { get; private set; }

        public bool IsLeftConnected { get; set; }
        public bool IsRightConnected { get; set; }
        public bool IsUpConnected { get; set; }
        public bool IsDownConnected { get; set; }

        public Connections Connections =>
            new Connections(IsUpConnected ? 1 : 0,
                IsRightConnected ? 1 : 0,
                IsDownConnected ? 1 : 0,
                IsLeftConnected ? 1 : 0);

        public bool IsWalkable
        {
            get => _isWalkable;
            set
            {
                _isWalkable = value;
                _cellsGrid.SetDebugText(X, Y, _isWalkable ? Color.green : Color.red);
            }
        }

        private bool _isWalkable = true;

        public PathNode PrevNode { get; set; }

        public PathNode(CellsGrid<PathNode> cellsGrid, int x, int y)
        {
            _cellsGrid = cellsGrid;
            Position = new Vector2Int(x, y);
        }

        public void CalculateFCost()
        {
            FCost = GCost + HCost;
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }
}