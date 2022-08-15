using UnityEngine;

namespace OctanGames.Map
{
    public class PathNode
    {
        private CellsGrid<PathNode> _cellsGrid;
        public int X => Position.x;
        public int Y => Position.y;

        public Vector2Int Position { get; }
        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost { get; private set; }
        public bool IsWalkable { get; set; }

        public PathNode PrevNode { get; set; }

        public PathNode(CellsGrid<PathNode> cellsGrid, int x, int y)
        {
            _cellsGrid = cellsGrid;
            Position = new Vector2Int(x, y);
            IsWalkable = true;
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