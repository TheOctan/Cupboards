using UnityEngine;

namespace OctanGames.Map.Node
{
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

        public int ConnectionToIndex()
        {
            switch (Connections)
            {
                case { U: 1, R: 1, D: 1, L: 1 }: return 5;
                case { U: 1, R: 1, D: 1, L: 0 }: return 4;
                case { U: 1, R: 1, D: 0, L: 1 }: return 9;
                case { U: 1, R: 1, D: 0, L: 0 }: return 8;
                case { U: 1, R: 0, D: 1, L: 1 }: return 6;
                case { U: 1, R: 0, D: 1, L: 0 }: return 7;
                case { U: 1, R: 0, D: 0, L: 1 }: return 10;
                case { U: 1, R: 0, D: 0, L: 0 }: return 11;
                case { U: 0, R: 1, D: 1, L: 1 }: return 1;
                case { U: 0, R: 1, D: 1, L: 0 }: return 0;
                case { U: 0, R: 1, D: 0, L: 1 }: return 13;
                case { U: 0, R: 1, D: 0, L: 0 }: return 12;
                case { U: 0, R: 0, D: 1, L: 1 }: return 2;
                case { U: 0, R: 0, D: 1, L: 0 }: return 3;
                case { U: 0, R: 0, D: 0, L: 1 }: return 14;
                case { U: 0, R: 0, D: 0, L: 0 }: return 15;
                default: return 15;
            }
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