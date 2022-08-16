using UnityEngine;

namespace OctanGames.Map.Node
{
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
        public int ConnectionToIndex();
    }
}