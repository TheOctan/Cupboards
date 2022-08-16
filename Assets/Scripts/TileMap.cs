using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityExtensions.Math;
using OctanGames.Map;
using OctanGames.Map.Node;
using OctanGames.StateMachine;
using UnityEngine.Events;

namespace OctanGames
{
    public enum MapType
    {
        StartPositions,
        WinPositions
    }

    public class TileMap : MonoBehaviour
    {
        [SerializeField] private TileOptions _tileOptions;
        [SerializeField] private Chip _chipPrefab;
        [SerializeField] private SpriteRenderer _tilePrefab;

        [Header("Properties")]
        [SerializeField] private float _chipPercentSize = 0.5f;
        [SerializeField] private float _tileSize = 3f;
        [SerializeField] private float _chipMovementDuration = 0.1f;
        [SerializeField] private MapType _mapType = MapType.StartPositions;
        [SerializeField] private bool _showDebug;
        
        [Header("Events")]
        [SerializeField] private UnityEvent _onWin;

        private readonly List<Chip> _chips = new List<Chip>();
        private MapData _mapData;
        private Pathfinding _pathfinding;
        private GameContext _gameContext;
        private GameStateMachine _stateMachine;

        private Vector2Int _size;
        private int _totalChipsCount;
        private int _completedChipCount;

        private void Start()
        {
            _mapData = MapParser.ParseMapData(Application.streamingAssetsPath, "map1.txt");
            _size = new Vector2Int(
                _mapData.Points.Max(p => p.x),
                _mapData.Points.Max(p => p.y));
            _totalChipsCount = _mapData.CountChips;

            _pathfinding = new Pathfinding(transform, _size.x, _size.y, _tileSize, _showDebug);

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
            if (_mapType == MapType.StartPositions)
            {
                _stateMachine.Update();
            }
            _pathfinding.UpdateDebug();
        }

        private void GenerateTiles()
        {
            for (var i = 0; i < _mapData.Points.Length; i++)
            {
                SpriteRenderer tile = Instantiate(_tilePrefab, transform, false);

                Vector2Int position = _mapData.Points[i] - Vector2Int.one;

                tile.transform.localScale = Vector3.one * _tileSize;
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
                chip.OnCompleted += OnChipCompleted;

                int positionIndex = _mapData.StartPositions[i] - 1;
                int winPositionIndex = _mapData.WinPositions[i] - 1;
                Vector2Int startPosition = _mapData.Points[positionIndex] - Vector2Int.one;
                Vector2Int winPosition = _mapData.Points[winPositionIndex] - Vector2Int.one;

                switch (_mapType)
                {
                    case MapType.StartPositions:
                        chip.SetStartPosition(startPosition);
                        chip.SetEndPosition(winPosition);
                        chip.transform.localPosition = (startPosition.TileCenter() * _tileSize).ToVertical();
                        break;
                    case MapType.WinPositions:
                        chip.SetStartPosition(winPosition);
                        chip.transform.localPosition = (winPosition.TileCenter() * _tileSize).ToVertical();
                        break;
                    default:
                        Debug.Log($"Map type {_mapType} undefined");
                        break;
                }

                chip.transform.localScale = Vector3.one * _tileSize * _chipPercentSize;
                chip.SetColor(_tileOptions.Colors[i]);

                _chips.Add(chip);
            }
        }

        private void OnChipCompleted()
        {
            _completedChipCount++;
            if (_completedChipCount == _totalChipsCount)
            {
                _onWin.Invoke();
            }
        }
    }
}