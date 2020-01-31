using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.TD.Map
{
    public class HexMapGenerator : MonoBehaviour
    {
        [SerializeField]
        private GameObject _hexPrefab;

        [SerializeField]
        [Range(10,100)]
        private int _width = 12;

        [SerializeField]
        [Range(10,100)]
        private int _height = 12;

        [SerializeField]
        [Range(0.1f, 10)]
        private float _hexEdgeLength = 1;

        [SerializeField]
        [Range(0.01f, 0.1f)]
        private float _spaceBetweenHexes = 0.01f;

        private void Start()
        {
            SpawnMap();
        }

        private void SpawnMap()
        {
            for(int i = 0; i < _width; i++)
                for(int j = 0; j < _height; j++)
                {
                    //Instantiate hex
                    GameObject hex = Instantiate(_hexPrefab, transform);

                    //Set hex position
                    float xPos = i * (1.5f * _hexEdgeLength + _spaceBetweenHexes);
                    float yPos = 0;
                    float zPos = j * (Mathf.Sqrt(3) * _hexEdgeLength + _spaceBetweenHexes);
                    zPos += i % 2 == 0 ? _hexEdgeLength * Mathf.Sqrt(3) / 2 : 0;
                    hex.transform.position = new Vector3(xPos, yPos, zPos);

                    //Set other hex settings
                    hex.name = $"Hex: {i * _width +j}";
                }
        }
    }
}