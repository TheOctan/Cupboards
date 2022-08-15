using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityExtensions.Input;
using UnityExtensions.Math;
using OctanGames.Map;

namespace OctanGames
{
    public class TileMap : MonoBehaviour
    {
        [SerializeField] private TileColors _tileColors;
        [SerializeField] private Tile _tilePrefab;

        [Header("Properties")]
        [SerializeField] private float _tileSize = 1f;

        private Pathfinding _pathfinding;
        private MapData _mapData;

        private readonly List<Tile> _tiles = new List<Tile>();
        private Vector2Int _size;

        private void Start()
        {
            _mapData = MapParser.ParseMapData(Application.streamingAssetsPath, "map1.txt");

            _size.x = _mapData.Points.Max(p => p.x);
            _size.y = _mapData.Points.Max(p => p.y);

            _pathfinding = new Pathfinding(transform, _size.x, _size.y, _tileSize);
            GenerateTiles();

            _pathfinding.BakePoints(_mapData.Points);
            _pathfinding.BakeObstacles(_tiles.Select(t => t.Position).ToArray());
        }

        private void GenerateTiles()
        {
            for (var i = 0; i < _mapData.CountChips; i++)
            {
                Tile tile = Instantiate(_tilePrefab, transform, false);

                int positionIndex = _mapData.StartPositions[i] - 1;
                Vector2Int position = _mapData.Points[positionIndex] - Vector2Int.one;

                tile.Position = position;
                tile.transform.localScale = Vector3.one * _tileSize;
                tile.transform.localPosition = (position.ToVector2().TileCenter() * _tileSize).ToVertical();
                tile.SetColor(_tileColors[i]);

                _tiles.Add(tile);
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorldPosition = InputExtensions.GetMouseWorldPosition();
                float cellSize = _pathfinding.CellsGrid.CellSize;
                _pathfinding.CellsGrid.GetXY(mouseWorldPosition, out int x, out int y);
                List<PathNode> path = _pathfinding.FindPath(0, 0, x, y);
                if (path != null)
                {
                    for (var i = 0; i < path.Count - 1; i++)
                    {
                        Vector3 startPoint =
                            transform.TransformPoint((path[i].Position.ToVector3() + Vector3.one * 0.5f) * cellSize);
                        Vector3 endPoint =
                            transform.TransformPoint((path[i + 1].Position.ToVector3() + Vector3.one * 0.5f) *
                                                     cellSize);
                        Debug.DrawLine(startPoint, endPoint, Color.green, 5f);
                    }
                }
            }

            _pathfinding.UpdateDebug();
        }
    }
}