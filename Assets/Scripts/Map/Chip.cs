using System;
using UnityEngine;

namespace OctanGames.Map
{
    public class Chip : MonoBehaviour
    {
        public event Action OnCompleted;

        [SerializeField] private SpriteRenderer _chip;
        [SerializeField] private SpriteRenderer _shadow;

        [Header("Properties")] [SerializeField]
        private bool _interactable = true;

        [SerializeField] private float _colorSelectionDelta = 0.25f;

        private Color _startColor;
        private Vector2Int _endPosition;

        public Vector2Int Position { get; private set; }

        public bool Interactable => _interactable;
        public int X => Position.x;
        public int Y => Position.y;

        public void SetEndPosition(Vector2Int endPosition)
        {
            _endPosition = endPosition;
        }

        public void SetStartPosition(Vector2Int startPosition)
        {
            Position = startPosition;
        }

        public void UpdateCurrentPosition(Vector2Int currentPosition)
        {
            Position = currentPosition;
            if (Position == _endPosition)
            {
                _interactable = false;
                Disable();
                OnCompleted?.Invoke();
            }
        }

        public void SetColor(Color color)
        {
            _chip.color = color;
            _shadow.color = color - new Color(_colorSelectionDelta, _colorSelectionDelta, _colorSelectionDelta, 0);
            _startColor = color;
        }

        public void Select()
        {
            if (!Interactable)
            {
                return;
            }

            _chip.color =
                _startColor + new Color(_colorSelectionDelta, _colorSelectionDelta, _colorSelectionDelta);
        }

        public void UnSelect()
        {
            if (!Interactable)
            {
                return;
            }

            _chip.color = _startColor;
        }

        private void Disable()
        {
            _chip.color = _startColor - new Color(_colorSelectionDelta * 1.5f, 
                _colorSelectionDelta * 1.5f,
                _colorSelectionDelta * 1.5f, 0);
        }
    }
}