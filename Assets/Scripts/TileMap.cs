using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityExtensions;
using UnityExtensions.Math;
using OctanGames.Map;
using OctanGames.Map.Node;
using OctanGames.StateMachine;

namespace OctanGames
{
    public class TileMap : MonoBehaviour
    {
        [SerializeField] private TileOptions _tileOptions;
        [SerializeField] private Chip _chipPrefab;
        [SerializeField] private SpriteRenderer _tilePrefab;

        [Header("Properties")] [SerializeField]
        private float _chipPercentSize = 0.5f;

        [SerializeField] private float _tileSize = 1f;
        [SerializeField] private float _chipMovementDuration = 0.2f;

        private readonly List<Chip> _chips = new List<Chip>();
        private MapData _mapData;
        private Pathfinding _pathfinding;
        private GameContext _gameContext;
        private GameStateMachine _stateMachine;

        private Vector2Int _size;
        private Chip _currentSelectedChip;
        private bool ChipIsSelected => _currentSelectedChip.ReferenceNotEquals(null);

        private void Start()
        {
            _mapData = MapParser.ParseMapData(Application.streamingAssetsPath, "map1.txt");
            _size = new Vector2Int(
                _mapData.Points.Max(p => p.x),
                _mapData.Points.Max(p => p.y));

            _pathfinding = new Pathfinding(transform, _size.x, _size.y, _tileSize);

            GenerateChips();
            _pathfinding.BakePoints(_mapData.Points);
            _pathfinding.BakeObstacles(_chips.Select(t => t.Position).ToArray());
            _pathfinding.BakeConnections(_mapData);
            GenerateTiles();

            _gameContext = new GameContext(transform, _mapData, _pathfinding, _chips, _tileSize, _chipMovementDuration);
            _stateMachine = new GameStateMachine(_gameContext);
            _stateMachine.SwitchState(GameState.Deselect);
        }

        private void Update()
        {
            _stateMachine.Update();
            _pathfinding.UpdateDebug();
        }

        private void GenerateTiles()
        {
            for (var i = 0; i < _mapData.Points.Length; i++)
            {
                SpriteRenderer tile = Instantiate(_tilePrefab, transform, false);

                Vector2Int position = _mapData.Points[i] - Vector2Int.one;

                tile.transform.localScale = Vector3.one;
                tile.transform.localPosition = (position.TileCenter() * _tileSize).ToVertical();

                IReadOnlyNode node = _pathfinding.GetReadOnlyNode(position.x, position.y);

                int index = node.ConnectionToIndex();
                tile.sprite = _tileOptions.TileSprites[index];
            }
        }

        private void GenerateChips()
        {
            for (var i = 0; i < _mapData.CountChips; i++)
            {
                Chip chip = Instantiate(_chipPrefab, transform, false);

                int positionIndex = _mapData.StartPositions[i] - 1;
                int winPositionIndex = _mapData.WinPositions[i] - 1;
                Vector2Int position = _mapData.Points[positionIndex] - Vector2Int.one;
                Vector2Int winPosition = _mapData.Points[winPositionIndex] - Vector2Int.one;

                chip.SetEndPosition(winPosition);
                chip.UpdateCurrentPosition(position);
                chip.transform.localScale = Vector3.one * _tileSize * _chipPercentSize;
                chip.transform.localPosition = (position.TileCenter() * _tileSize).ToVertical();
                chip.SetColor(_tileOptions.Colors[i]);

                _chips.Add(chip);
            }
        }

        private void ResetCurrentSelectedChip()
        {
            _currentSelectedChip?.UnSelect();
            _currentSelectedChip = null;
        }
    }
}