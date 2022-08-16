using System;
using TMPro;
using UnityEngine;
using UnityExtensions;

namespace OctanGames.Map
{
    public class GridInt
    {
        public event Action<Vector2Int> OnValueChanged; 

        private readonly Transform _transform;

        private readonly int[,] _gridArray;
        private readonly TextMeshPro[,] _debugTextArray;
        private readonly Vector3 _originPosition;

        public bool ShowDebug { get; set; } = true;
        public int Width { get; }
        public int Height { get; }
        public float CellSize { get; }

        public GridInt(Transform transform, int width, int height, float cellSize = 1, Vector3 originPosition = default)
        {
            _transform = transform;
            CellSize = cellSize;
            Height = height;
            Width = width;
            _originPosition = originPosition;

            _gridArray = new int[width, height];
            _debugTextArray = new TextMeshPro[width, height];

            if (ShowDebug)
            {
                DrawDebug();
            }
        }

        private void DrawDebug()
        {
            for (var x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (var y = 0; y < _gridArray.GetLength(1); y++)
                {
                    _debugTextArray[x, y] = TextExtensions.CreateWorldTextTMP(_gridArray[x, y].ToString(), Color.white,
                        _transform, WorldPosition(x, y) + Vector3.one * CellSize * 0.5f, 5);

                    Debug.DrawLine(WorldPosition(x, y), WorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(WorldPosition(x, y), WorldPosition(x + 1, y), Color.white, 100f);
                }
            }

            Debug.DrawLine(WorldPosition(0, Height), WorldPosition(Width, Height), Color.white, 100f);
            Debug.DrawLine(WorldPosition(Width, 0), WorldPosition(Width, Height), Color.white, 100f);
        }

        public void SetValue(int x, int y, int value)
        {
            if (IsValidCoordinate(x, y))
            {
                _gridArray[x, y] = value;
                _debugTextArray[x, y].text = value.ToString();
                OnValueChanged?.Invoke(new Vector2Int(x, y));
            }
        }

        public void SetValue(Vector3 worldPosition, int value)
        {
            (int x, int y) = GetCoordinate(worldPosition);
            SetValue(x, y, value);
        }

        public int GetValue(Vector3 worldPosition)
        {
            (int x, int y) = GetCoordinate(worldPosition);
            return GetValue(x, y);
        }

        public int GetValue(int x, int y)
        {
            if (IsValidCoordinate(x, y))
            {
                return _gridArray[x, y];
            }
            else
            {
                return 0;
            }
        }

        private Vector3 WorldPosition(int x, int y)
        {
            return new Vector3(x, y) * CellSize + _originPosition;
        }

        private (int x, int y) GetCoordinate(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt((worldPosition - _originPosition).x / CellSize);
            int y = Mathf.FloorToInt((worldPosition - _originPosition).y / CellSize);
            return (x, y);
        }

        private bool IsValidCoordinate(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }
    }
}