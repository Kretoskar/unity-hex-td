using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.TD.Map
{
    public class HexMapGenerator : MonoBehaviour
    {
        [SerializeField]
        private HexTypesSO _hexTypesSO;
        [SerializeField]
        private HexMapSO _hexMapSO;

        //Injected from scriptable objects
        private GameObject _emptyHexPrefab;
        private GameObject _straightPathHexPrefab;
        private GameObject _curveUpPathHexPrefab;
        private GameObject _curveDownPathHexPrefab;
        private int _width;
        private int _height;
        private float _hexEdgeLength;
        private float _spaceBetweenHexes;

        private float _distanceBetweenHexesX = 1.5f;
        private Transform _hexParent;

        private void Awake()
        {
            _hexParent = transform;
            InjectDataFromScriptableObjects();
        }

        private void Start()
        {
            SpawnPath();
        }

        private void SpawnPath()
        {
            Vector3 lastPos;

            //Spawn entrance
            GameObject pathEntranceHex = Instantiate(_straightPathHexPrefab, _hexParent);
            pathEntranceHex.name = "Path entrance hex";
            lastPos = CalculateEntrancePosition();
            pathEntranceHex.transform.position = lastPos;

            //Spawn rest
            do
            {
                GameObject nextPathHex = Instantiate(_straightPathHexPrefab, _hexParent);
                lastPos.x += Mathf.Sqrt(3) * _hexEdgeLength + _spaceBetweenHexes;
                nextPathHex.transform.position = lastPos;
            } while (lastPos.x < (_width - 1) * _distanceBetweenHexesX);
        }

        private void InjectDataFromScriptableObjects()
        {
            //Inject data from hex type SO
            _emptyHexPrefab = _hexTypesSO.EmptyHexPrefab;
            _straightPathHexPrefab = _hexTypesSO.StraightPathHexPrefab;
            _curveUpPathHexPrefab = _hexTypesSO.CurveUpPathHexPrefab;
            _curveDownPathHexPrefab = _hexTypesSO.CurveDownPathHexPrefab;

            //Inject data from hex map SO
            _width = _hexMapSO.Width;
            _height = _hexMapSO.Height;
            _hexEdgeLength = _hexMapSO.HexEdgeLength;
            _spaceBetweenHexes = _hexMapSO.SpaceBetweenHexes;
        }

        private Vector3 CalculateEntrancePosition()
        {
            int entrancePosition = UnityEngine.Random.Range(1,_height);
            float xPos = 0;
            float yPos = 0;
            float zPos = entrancePosition;
            return new Vector3(xPos,yPos,zPos);
        }

        private void SpawnEmptyMap()
        {
            for(int i = 0; i < _width; i++)
                for(int j = 0; j < _height; j++)
                {
                    //Instantiate hex
                    GameObject hex = Instantiate(_emptyHexPrefab, transform);

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