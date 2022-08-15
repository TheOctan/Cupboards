using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityExtensions.Input;
using UnityExtensions.Math;
using OctanGames.Map;

namespace OctanGames
{
    public class TileMap : MonoBehaviour
    {
        [SerializeField] private TileOptions _tileOptions;
        [SerializeField] private Chip _chipPrefab;
        [SerializeField] private SpriteRenderer _tilePrefab;

        [Header("Properties")]
        [SerializeField] private float _tileSize = 1f;
        [SerializeField] private float _chipPercentSize = 0.5f;
        [SerializeField] private float _chipMovementDuration = 0.2f;

        private readonly List<Chip> _chips = new List<Chip>();
        private Pathfinding _pathfinding;
        private MapData _mapData;

        private Vector2Int _size;

        private Chip _currentSelectedChip;
        private bool _chipIsSelected;

        private void Start()
        {
            _mapData = MapParser.ParseMapData(Application.streamingAssetsPath, "map1.txt");

            _size.x = _mapData.Points.Max(p => p.x);
            _size.y = _mapData.Points.Max(p => p.y);

            _pathfinding = new Pathfinding(transform, _size.x, _size.y, _tileSize);

            GenerateChips();

            _pathfinding.BakePoints(_mapData.Points);
            _pathfinding.BakeObstacles(_chips.Select(t => t.Position).ToArray());
            _pathfinding.BakeConnections(_mapData);

            GenerateTiles();
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

                int index = ConnectionToIndex(node);
                tile.sprite = _tileOptions.TileSprites[index];
            }
        }

        private static int ConnectionToIndex(IReadOnlyNode node)
        {
            switch (node.Connections)
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

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorldPosition = InputExtensions.GetMouseWorldPosition();

                if (!_chipIsSelected)
                {
                    _pathfinding.CellsGrid.GetXY(mouseWorldPosition, out int x, out int y);
                    _currentSelectedChip = _chips.FirstOrDefault(c => c.X == x && c.Y == y);

                    _chipIsSelected = _currentSelectedChip.ReferenceNotEquals(null);
                    if (_chipIsSelected && _currentSelectedChip.Interactable)
                    {
                        _currentSelectedChip.Select();
                    }
                }
                else
                {
                    _pathfinding.CellsGrid.GetXY(mouseWorldPosition, out int endX, out int endY);

                    List<PathNode> path = _pathfinding.FindPath(_currentSelectedChip.X, _currentSelectedChip.Y, endX, endY);
                    if (path != null)
                    {
                        DrawDebugPath(path);
                        MoveChip(path);
                    }
                    else
                    {
                        ResetCurrentSelectedChip();
                    }
                }
            }

            _pathfinding.UpdateDebug();
        }

        private void ResetCurrentSelectedChip()
        {
            _chipIsSelected = false;
            _currentSelectedChip?.UnSelect();
            _currentSelectedChip = null;
        }

        private void MoveChip(List<PathNode> path)
        {
            Sequence sequence = DOTween.Sequence();
            foreach (PathNode pathNode in path)
            {
                sequence.Append(_currentSelectedChip.transform
                    .DOMove((pathNode.Position.TileCenter() * _tileSize).ToVertical(), _chipMovementDuration)
                    .SetEase(Ease.InOutSine));
            }

            sequence.OnComplete(() =>
            {
                _currentSelectedChip.UpdateCurrentPosition(path[path.Count - 1].Position);
                ResetCurrentSelectedChip();
            });
        }

        private void DrawDebugPath(List<PathNode> path)
        {
            float cellSize = _pathfinding.CellsGrid.CellSize;
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