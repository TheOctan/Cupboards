using System;
using UnityEngine;

namespace OctanGames.Map
{
    public class MapData
    {
        public int CountChips;
        public int CountPoints;

        public Vector2Int[] Points;
        public int[] StartPositions;
        public int[] WinPositions;
        public int CountConnections;
        public Connection[] Connections;
    }

    public class Connection
    {
        public int StartPoint;
        public int EndPoint;
    }
}