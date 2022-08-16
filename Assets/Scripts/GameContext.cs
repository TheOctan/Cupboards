using System.Collections.Generic;
using System.Linq;
using OctanGames.Map;
using UnityEngine;
using UnityExtensions;

namespace OctanGames
{
    public class GameContext
    {
        private readonly List<Chip> _chips;

        public Transform Transform { get; }
        public MapData MapData { get; }
        public Pathfinding Pathfinding { get; }
        public Chip CurrentSelectedChip { get; private set; }
        public bool IsChipSelected => CurrentSelectedChip.ReferenceNotEquals(null);
        public Vector2Int LastSelectedPosition { get; set; }
        public float TileSize { get; }
        public float ChipMovementDuration { get; }

        public GameContext(Transform transform, MapData mapData, Pathfinding pathfinding, List<Chip> chips,
            float tileSize, float chipMovementDuration)
        {
            Transform = transform;
            MapData = mapData;
            Pathfinding = pathfinding;
            _chips = chips;
            TileSize = tileSize;
            ChipMovementDuration = chipMovementDuration;
        }

        public bool SetSelectedChip(int x, int y)
        {
            CurrentSelectedChip = GetChip(x, y);
            if (IsChipSelected && CurrentSelectedChip.Interactable)
            {
                CurrentSelectedChip.Select();
                return true;
            }

            return false;
        }

        public Chip GetChip(int x, int y)
        {
            return _chips.FirstOrDefault(c => c.X == x && c.Y == y);
        }

        public void ResetCurrentSelectedChip()
        {
            if (IsChipSelected)
            {
                CurrentSelectedChip.UnSelect();
                CurrentSelectedChip = null;
            }
        }
    }
}