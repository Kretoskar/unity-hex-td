using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.TD.Map
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Hex map", fileName = "Hex map")]
    public class HexMapSO : ScriptableObject
    {
        [Header("Map")]

        [SerializeField]
        [Range(10, 100)]
        private int _width = 12;

        [SerializeField]
        [Range(0.1f, 10)]
        private float _hexEdgeLength = 1;

        [SerializeField]
        [Range(0.01f, 0.1f)]
        private float _spaceBetweenHexes = 0.03f;

        [Header("Path")]

        [SerializeField]
        [Range(0, 1)]
        private float _curvesDensity = 0;

        [Header("Empty hexes")]

        [SerializeField]
        [Range(0,20)]
        private int _minEmptyHexes = 0;

        [SerializeField]
        [Range(0,20)]
        private int _maxEmptyHexes = 0;

        [SerializeField]
        [Range(0, 1)]
        private float _emptyHexDensityRandomization = 0;

        [SerializeField]
        [Range(0, 1)]
        private float _topDownDiversity = 0;

        public int Width { get => _width; }
        public float HexEdgeLength { get => _hexEdgeLength; }
        public float SpaceBetweenHexes { get => _spaceBetweenHexes; }
        public int NumberOfCurves { get => Mathf.FloorToInt(_curvesDensity * _width); }
        public int RandomHexCount 
        {
            get
            {
                int center = _maxEmptyHexes - _minEmptyHexes;
                return Mathf.Clamp(Random.Range(center - Mathf.FloorToInt(_emptyHexDensityRandomization * center), center + Mathf.FloorToInt(_emptyHexDensityRandomization * center)), _minEmptyHexes, _maxEmptyHexes);
            }
        }

        public float TopDownDiversity { get => _topDownDiversity; set => _topDownDiversity = value; }
    }
}