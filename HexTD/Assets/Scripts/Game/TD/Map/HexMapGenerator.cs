using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private int _numberOfCurves;
        private float _hexEdgeLength;
        private float _spaceBetweenHexes;

        private float _distanceBetweenHexesX = 1.5f;
        private Vector3 _lastPos;
        private Transform _hexParent;
        private PathEntrancePosition _currentPathEntrancePosition;

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
            List<int> curvesPositions = CalculateCurvesPositions();

            //Spawn entrance
            SpawnPathEntrance();

            //Spawn rest
            do
            {
                if (curvesPositions.Contains((int)_lastPos.x))
                {
                    SpawnCurvedPathHex();
                }
                else
                {
                    //Spawn straight path
                    SpawnStraightPathHex();
                }
            } while (_lastPos.x < (_width - 1) * _distanceBetweenHexesX);
        }

        private void SpawnCurvedPathHex()
        {
            GameObject prefab;
            bool isCurvedToTop = false;
            PathEntrancePosition _pathEntrancePositionForThisHex = _currentPathEntrancePosition;

            //Spawn curved path
            switch(_currentPathEntrancePosition)
            {
                case (PathEntrancePosition.Upper):
                    isCurvedToTop = true;
                    _currentPathEntrancePosition = PathEntrancePosition.Center;
                    break;
                case (PathEntrancePosition.Center):
                    isCurvedToTop = UnityEngine.Random.Range(0,2) == 0;
                    if (isCurvedToTop)
                    {
                        _currentPathEntrancePosition = PathEntrancePosition.Lower;
                    }
                    else
                    {
                        _currentPathEntrancePosition = PathEntrancePosition.Upper;
                    }
                    break;
                case (PathEntrancePosition.Lower):
                    isCurvedToTop = false;
                    _currentPathEntrancePosition = PathEntrancePosition.Center;
                    break;
                default:
                    break;
            }

            if (isCurvedToTop)
            {
                prefab = _curveUpPathHexPrefab;
            }
            else
            {
                prefab = _curveDownPathHexPrefab;
            }

            GameObject curvePathHex = Instantiate(prefab, _hexParent);
            GameObject pathGO = curvePathHex.transform.GetChild(1).gameObject;
            switch (_pathEntrancePositionForThisHex)
            {
                case (PathEntrancePosition.Upper):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, 60);
                    _lastPos.x += (Mathf.Sqrt(3) * _hexEdgeLength + _spaceBetweenHexes) / 2;
                    _lastPos.z -= _hexEdgeLength * _distanceBetweenHexesX;
                    break;
                case (PathEntrancePosition.Center):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, 0);
                    _lastPos.x += Mathf.Sqrt(3) * _hexEdgeLength + _spaceBetweenHexes;
                    break;
                case (PathEntrancePosition.Lower):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, -60);
                    _lastPos.x += (Mathf.Sqrt(3) * _hexEdgeLength + _spaceBetweenHexes) / 2;
                    _lastPos.z += _hexEdgeLength * _distanceBetweenHexesX;
                    break;
                default:
                    break;
            }
            curvePathHex.transform.position = _lastPos;
        }

        private void SpawnStraightPathHex()
        {
            GameObject pathHex = Instantiate(_straightPathHexPrefab, _hexParent);
            pathHex.name = "Straight path hex";

            GameObject pathGO = pathHex.transform.GetChild(1).gameObject;
            switch(_currentPathEntrancePosition)
            {
                case (PathEntrancePosition.Upper):
                    pathGO.transform.rotation = Quaternion.Euler(-90,0,0);
                    _lastPos.x += (Mathf.Sqrt(3) * _hexEdgeLength + _spaceBetweenHexes)/2;
                    _lastPos.z -= _hexEdgeLength * _distanceBetweenHexesX;
                    break;
                case (PathEntrancePosition.Center):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, 120);
                    _lastPos.x += Mathf.Sqrt(3) * _hexEdgeLength + _spaceBetweenHexes;
                    break;
                case (PathEntrancePosition.Lower):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, 60);
                    _lastPos.x += (Mathf.Sqrt(3) * _hexEdgeLength + _spaceBetweenHexes)/2;
                    _lastPos.z += _hexEdgeLength * _distanceBetweenHexesX;
                    break;
                default:
                    break;
            }
            pathHex.transform.position = _lastPos;
        }

        private void SpawnPathEntrance()
        {
            GameObject hex = Instantiate(_straightPathHexPrefab, _hexParent);
            hex.name = "Path entrance hes";
            _lastPos = CalculateEntrancePosition();
            hex.transform.position = _lastPos;
            _currentPathEntrancePosition = PathEntrancePosition.Center;
        }

        private List<int> CalculateCurvesPositions()
        {
            List<int> curvesPositions = new List<int>();
            for(int i = 0; i < _numberOfCurves; i++)
            {
                curvesPositions.Add(UnityEngine.Random.Range(0,_width));
            }
            return curvesPositions.Distinct().ToList();
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
            _numberOfCurves = _hexMapSO.NumberOfCurves;
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
        private enum PathEntrancePosition
        {
            Upper,
            Center,
            Lower
        }
    }
}