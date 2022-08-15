using System;
using TMPro;
using UnityEngine;
using UnityExtensions;

namespace OctanGames.Map
{
    public class CellsGrid<T>
    {
        public event Action<Vector2Int> OnGridObjectChanged;

        private readonly Vector3 _originPosition;
        private readonly T[,] _gridArray;

        public bool ShowDebug { get; set; } = true;
        public int Width { get; }
        public int Height { get; }
        public float CellSize { get; }

        public CellsGrid(int width, int height, float cellSize, Vector3 originPosition,
            Func<CellsGrid<T>, int, int, T> createGridObject)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;
            _originPosition = originPosition;

            _gridArray = new T[width, height];

            for (var x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (var y = 0; y < _gridArray.GetLength(1); y++)
                {
                    _gridArray[x, y] = createGridObject(this, x, y);
                }
            }

            if (ShowDebug)
            {
                DrawDebug(width, height, cellSize);
            }
        }

        private void DrawDebug(int width, int height, float cellSize)
        {
            var debugTextArray = new TextMeshPro[width, height];

            for (var x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (var y = 0; y < _gridArray.GetLength(1); y++)
                {
                    debugTextArray[x, y] = TextExtensions.CreateWorldTextTMP(
                        _gridArray[x, y]?.ToString(), Color.white, null,
                        GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 5);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }

            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

            OnGridObjectChanged += (pos) =>
            {
                debugTextArray[pos.x, pos.y].text = _gridArray[pos.x, pos.y]?.ToString();
            };
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, y) * CellSize + _originPosition;
        }

        public void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition - _originPosition).x / CellSize);
            y = Mathf.FloorToInt((worldPosition - _originPosition).y / CellSize);
        }

        public void SetGridObject(int x, int y, T value)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                _gridArray[x, y] = value;
                OnGridObjectChanged?.Invoke(new Vector2Int(x, y));
            }
        }

        public void TriggerGridObjectChanged(int x, int y)
        {
            OnGridObjectChanged?.Invoke(new Vector2Int(x, y));
        }

        public void SetGridObject(Vector3 worldPosition, T value)
        {
            GetXY(worldPosition, out int x, out int y);
            SetGridObject(x, y, value);
        }

        public T GetGridObject(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                return _gridArray[x, y];
            }

            return default;
        }

        public T GetGridObject(Vector3 worldPosition)
        {
            GetXY(worldPosition, out int x, out int y);
            return GetGridObject(x, y);
        }
    }
}