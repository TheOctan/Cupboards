using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace OctanGames
{
    [CreateAssetMenu()]
    public class TileOptions : ScriptableObject
    {
        public IReadOnlyList<Color> Colors => _colors;
        public IReadOnlyList<Sprite> TileSprites => _tileSprites;

        [Header("Colors")]
        [SerializeField] private List<Color> _colors;
        [SerializeField] private Color _defaultColor;
        [FormerlySerializedAs("_type")] [Header("Sprites")] [SerializeField]
        private List<Sprite> _tileSprites;
    }
}