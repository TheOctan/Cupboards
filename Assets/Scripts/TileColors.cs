using System.Collections.Generic;
using UnityEngine;

namespace OctanGames
{
    [CreateAssetMenu()]
    public class TileColors : ScriptableObject
    {
        public IReadOnlyList<Color> Colors => _colors;
        public Color this[int i] => _colors[i]; 

        [SerializeField] private List<Color> _colors;
        [SerializeField] private Color _defaultColor;
    }
}