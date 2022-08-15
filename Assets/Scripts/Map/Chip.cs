using System;
using UnityEngine;
using UnityExtensions.Math;

namespace OctanGames.Map
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Chip : MonoBehaviour, ITile
    {
        private SpriteRenderer _spriteRenderer;
        
        public Vector2Int Position { get; set; }

        public int X
        {
            get => Position.x;
            set => Position.SetX(value);
        }
        public int Y 
        {
            get => Position.y;
            set => Position.SetY(value);
        }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetColor(Color color)
        {
            _spriteRenderer.color = color;
        }
    }
}