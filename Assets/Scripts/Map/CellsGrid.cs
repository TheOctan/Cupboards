using System;
using TMPro;
using UnityEngine;
using UnityExtensions;
using UnityExtensions.Math;

namespace OctanGames.Map
{
    public class CellsGrid<T>
    {
        public event Action<Vector2Int> OnGridObjectChanged;

        private readonly Transform _transform;

        private readonly Vector3 _originPosition;
        private readonly T[,] _gridArray;
        private readonly TextMeshPro[,] _debugTextArray;

        public bool ShowDebug { get; set; } = true;
        public int Width { get; }
        public int Height { get; }
        public float CellSize { get; }

        public CellsGrid(Transform transform, int width, int height, float cellSize,
            Vector3 originPosition, Func<CellsGrid<T>, int, int, T> createGridObject, bool showDebug)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;
            ShowDebug = showDebug;
            _originPosition = originPosition;
            _transform = transform;

            _gridArray = new T[width, height];
            _debugTextArray = new TextMeshPro[width, height];

            for (var x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (var y = 0; y < _gridArray.GetLength(1); y++)
                {
                    _gridArray[x, y] = createGridObject(this, x, y);
                }
            }

            if (ShowDebug)
            {
                AddDebugText(width, height, cellSize);
            }
        }

        public void SetDebugText(int x, int y, Color color)
        {
            if (!ShowDebug)
            {
                return;
            }
            _debugTextArray[x, y].color = color;
        }

        public void UpdateDebug()
        {
            if (!ShowDebug)
            {
                return;
            }

            int width = _gridArray.GetLength(0);
            int height = _gridArray.GetLength(1);

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white);
                }
            }

            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white);
        }

        public Vector3 GetLocalPosition(int x, int y)
        {
            return new Vector3(x, y) * CellSize + _originPosition;
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return _transform.TransformPoint(GetLocalPosition(x, y));
        }

        public void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            Vector2Int position = _transform.InverseTransformPoint((worldPosition - _originPosition).Divide(CellSize)).ToVector2IntFloor();
            x = position.x;
            y = position.y;
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

        private void AddDebugText(int width, int height, float cellSize)
        {
            for (var x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (var y = 0; y < _gridArray.GetLength(1); y++)
                {
                    _debugTextArray[x, y] = TextExtensions.CreateWorldTextTMP(
                        _gridArray[x, y]?.ToString(), Color.white, _transform,
                        GetLocalPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 5);
                }
            }

            OnGridObjectChanged += (pos) =>
            {
                _debugTextArray[pos.x, pos.y].text = _gridArray[pos.x, pos.y]?.ToString();
            };
        }
    }
}