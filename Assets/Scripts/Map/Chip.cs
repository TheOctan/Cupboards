using UnityEngine;

namespace OctanGames.Map
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Chip : MonoBehaviour
    {
        [SerializeField] private bool _interactable = true;
        [SerializeField] private float _colorSelectionDelta = 0.5f;

        private SpriteRenderer _spriteRenderer;
        private Color _startColor;
        private Vector2Int _endPosition;

        public Vector2Int Position { get; private set; }

        public bool Interactable => _interactable;
        public int X => Position.x;
        public int Y => Position.y;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetEndPosition(Vector2Int endPosition)
        {
            _endPosition = endPosition;
        }

        public void UpdateCurrentPosition(Vector2Int currentPosition)
        {
            Position = currentPosition;
            if (Position == _endPosition)
            {
                _interactable = false;
                Disable();
            }
        }

        public void SetColor(Color color)
        {
            _spriteRenderer.color = color;
            _startColor = color;
        }

        public void Select()
        {
            _spriteRenderer.color = _startColor + Color.white * _colorSelectionDelta;
        }

        public void UnSelect()
        {
            _spriteRenderer.color = _startColor - Color.white * _colorSelectionDelta;
        }

        private void Disable()
        {
            _spriteRenderer.color = Color.gray;
        }
    }
}