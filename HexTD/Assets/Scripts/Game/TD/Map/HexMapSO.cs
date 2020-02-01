using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.TD.Map
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Hex map", fileName = "Hex map")]
    public class HexMapSO : ScriptableObject
    {
        [SerializeField]
        [Range(10, 100)]
        private int _width = 12;

        [SerializeField]
        [Range(10, 100)]
        private int _height = 12;

        [SerializeField]
        [Range(0.1f, 10)]
        private float _hexEdgeLength = 1;

        [SerializeField]
        [Range(0.01f, 0.1f)]
        private float _spaceBetweenHexes = 0.03f;

        [SerializeField]
        [Range(0, 1)]
        private float _curvesDensity;

        public int Width { get => _width; }
        public int Height { get => _height; }
        public float HexEdgeLength { get => _hexEdgeLength; }
        public float SpaceBetweenHexes { get => _spaceBetweenHexes; }
        public float CurvesDensity { get => Mathf.FloorToInt(_curvesDensity * _width / 2); }
    }
}