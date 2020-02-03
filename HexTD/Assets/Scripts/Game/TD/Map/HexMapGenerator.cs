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

        private float _distanceBetweenHexesX = 1.5f;
        private Vector3 _lastPos;
        private Transform _hexPathParent;
        private Transform _emptyHexParent;
        private PathEntrancePosition _currentPathEntrancePosition;

        #region injected from scriptable objects

        //Injected from scriptable objects
        private GameObject _emptyHexPrefab;
        private GameObject _straightPathHexPrefab;
        private GameObject _curveUpPathHexPrefab;
        private GameObject _curveDownPathHexPrefab;
        private int _width;
        private int _numberOfCurves;
        private float _hexEdgeLength;
        private float _spaceBetweenHexes;

        #endregion

        private void Awake()
        {
            _hexPathParent = transform;
            InjectDataFromScriptableObjects();
        }

        private void Start()
        {
            GameObject hexParentGO = new GameObject("Hex path");
            hexParentGO.transform.parent = transform;
            _hexPathParent = hexParentGO.transform;

            GameObject emptyHexParentGO = new GameObject("Empty hexes");
            emptyHexParentGO.transform.parent = transform;
            _emptyHexParent = emptyHexParentGO.transform;

            SpawnPath();
            SpawnEmptyHexes();
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

        private void SpawnEmptyHexes()
        {
            for (int i = 0; i < _hexPathParent.childCount; i++)
            {
                int emptyHexCount = _hexMapSO.RandomHexCount;
                int hexesToTopCount = UnityEngine.Random.Range(0, emptyHexCount);
                for (int j = 0; j < emptyHexCount; j++)
                {
                    if (j < hexesToTopCount)
                    {
                        //Spawn the first hex to the top
                        if (j == 0)
                        {
                            float xPos = 0, yPos = 0, zPos = 0;

                            Transform previousHex = i == 0 ? null : _hexPathParent.GetChild(i - 1);
                            Transform thisHex = _hexPathParent.GetChild(i);
                            Transform nextHex = i == _hexPathParent.childCount - 1 ? null : _hexPathParent.GetChild(i + 1);

                            if (nextHex == null) continue;
                            if (previousHex == null) continue;

                            if (Mathf.Abs(thisHex.transform.position.z - nextHex.transform.position.z) < 0.1f)
                            {
                                if (thisHex.transform.position.z - previousHex.transform.position.z > 0.1f)
                                {
                                    //This is a lower left to right curve path hex
                                    xPos = thisHex.transform.position.x - (Mathf.Sqrt(3) * _hexEdgeLength) / 2 - _spaceBetweenHexes;
                                    yPos = 0;
                                    zPos = thisHex.transform.position.z + 1.5f * _hexEdgeLength + _spaceBetweenHexes;
                                    SpawnEmptyHex(new Vector3(xPos, yPos, zPos));
                                }
                                //This is a straight path hex
                                xPos = thisHex.transform.position.x + (Mathf.Sqrt(3) * _hexEdgeLength) / 2 + _spaceBetweenHexes;
                                yPos = 0;
                                zPos = thisHex.transform.position.z + 1.5f * _hexEdgeLength + _spaceBetweenHexes;
                            }
                            else if ((thisHex.transform.position.z - nextHex.transform.position.z) < -0.1f)
                            {
                                if (thisHex.transform.position.z - previousHex.transform.position.z < 0.1f) continue; //This is in a pothole
                                                                                                                      //This is a straight path going upwards
                                xPos = thisHex.transform.position.x - (Mathf.Sqrt(3) * _hexEdgeLength) / 2 - _spaceBetweenHexes;
                                yPos = 0;
                                zPos = thisHex.transform.position.z + 1.5f * _hexEdgeLength + _spaceBetweenHexes;
                            }
                            else if ((thisHex.transform.position.z - nextHex.transform.position.z) > 0.1f)
                            {
                                //This is a straight path going downwards
                                xPos = thisHex.transform.position.x + (Mathf.Sqrt(3) * _hexEdgeLength) / 2 + _spaceBetweenHexes;
                                yPos = 0;
                                zPos = thisHex.transform.position.z + 1.5f * _hexEdgeLength + _spaceBetweenHexes;
                            }

                            SpawnEmptyHex(new Vector3(xPos, yPos, zPos));
                        }
                        //Spawn the other hexes to the top
                        else
                        {
                            float xPos = 0, yPos = 0, zPos = 0;

                            Transform previousHex = i == 0 ? null : _hexPathParent.GetChild(i - 1);
                            Transform thisHex = _hexPathParent.GetChild(i);
                            Transform nextHex = i == _hexPathParent.childCount - 1 ? null : _hexPathParent.GetChild(i + 1);

                            if (nextHex == null) continue;
                            if (previousHex == null) continue;

                            if (Mathf.Abs(thisHex.transform.position.z - nextHex.transform.position.z) < 0.1f)
                            {
                                if (thisHex.transform.position.z - previousHex.transform.position.z > 0.1f)
                                {
                                    //This is a lower left to right curve path hex
                                    xPos = thisHex.transform.position.x -  j *((Mathf.Sqrt(3) * _hexEdgeLength) / 2 - _spaceBetweenHexes);
                                    yPos = 0;
                                    zPos = thisHex.transform.position.z + j * 1.5f * (_hexEdgeLength + _spaceBetweenHexes);
                                    SpawnEmptyHex(new Vector3(xPos, yPos, zPos));
                                }
                                //This is a straight path hex
                                xPos = thisHex.transform.position.x + j * ((Mathf.Sqrt(3) * _hexEdgeLength) / 2 + _spaceBetweenHexes);
                                yPos = 0;
                                zPos = thisHex.transform.position.z + j * ((1.5f * _hexEdgeLength + _spaceBetweenHexes));
                            }
                            else if ((thisHex.transform.position.z - nextHex.transform.position.z) < -0.1f)
                            {
                                if (thisHex.transform.position.z - previousHex.transform.position.z < 0.1f) continue; //This is in a pothole
                                                                                                                      //This is a straight path going upwards
                                xPos = thisHex.transform.position.x - j * ((Mathf.Sqrt(3) * _hexEdgeLength) / 2 - _spaceBetweenHexes);
                                yPos = 0;
                                zPos = thisHex.transform.position.z + (j * 1.5f * _hexEdgeLength + _spaceBetweenHexes);
                            }
                            else if ((thisHex.transform.position.z - nextHex.transform.position.z) > 0.1f)
                            {
                                //This is a straight path going downwards
                                xPos = thisHex.transform.position.x + (j * (Mathf.Sqrt(3) * _hexEdgeLength) / 2 + _spaceBetweenHexes);
                                yPos = 0;
                                zPos = thisHex.transform.position.z + j * 1.5f * (_hexEdgeLength + _spaceBetweenHexes);
                            }
                            SpawnEmptyHex(new Vector3(xPos, yPos, zPos));
                        }
                    }
                    else
                    {
                        //Spawn first to bottom
                        if (j == emptyHexCount - hexesToTopCount - 1)
                        {
                            float xPos = 0, yPos = 0, zPos = 0;

                            Transform previousHex = i == 0 ? null : _hexPathParent.GetChild(i - 1);
                            Transform thisHex = _hexPathParent.GetChild(i);
                            Transform nextHex = i == _hexPathParent.childCount - 1 ? null : _hexPathParent.GetChild(i + 1);

                            if (nextHex == null) continue;
                            if (previousHex == null) continue;

                            if (Mathf.Abs(thisHex.transform.position.z - nextHex.transform.position.z) < 0.1f)
                            {
                                if (thisHex.transform.position.z - previousHex.transform.position.z > 0.1f)
                                {
                                    //This is a lower left to right curve path hex
                                    xPos = thisHex.transform.position.x - (Mathf.Sqrt(3) * _hexEdgeLength) / 2 - _spaceBetweenHexes;
                                    yPos = 0;
                                    zPos = thisHex.transform.position.z - 1.5f * _hexEdgeLength - _spaceBetweenHexes;
                                    SpawnEmptyHex(new Vector3(xPos, yPos, zPos));
                                }
                                //This is a straight path hex
                                xPos = thisHex.transform.position.x + (Mathf.Sqrt(3) * _hexEdgeLength) / 2 + _spaceBetweenHexes;
                                yPos = 0;
                                zPos = thisHex.transform.position.z - 1.5f * _hexEdgeLength - _spaceBetweenHexes;
                            }
                            else if ((thisHex.transform.position.z - nextHex.transform.position.z) < -0.1f)
                            {
                                if (thisHex.transform.position.z - previousHex.transform.position.z < 0.1f) continue; //This is in a pothole
                                                                                                                      //This is a straight path going upwards
                                xPos = thisHex.transform.position.x - (Mathf.Sqrt(3) * _hexEdgeLength) / 2 - _spaceBetweenHexes;
                                yPos = 0;
                                zPos = thisHex.transform.position.z - 1.5f * _hexEdgeLength - _spaceBetweenHexes;
                            }
                            else if ((thisHex.transform.position.z - nextHex.transform.position.z) > 0.1f)
                            {
                                //This is a straight path going downwards
                                xPos = thisHex.transform.position.x + (Mathf.Sqrt(3) * _hexEdgeLength) / 2 + _spaceBetweenHexes;
                                yPos = 0;
                                zPos = thisHex.transform.position.z - 1.5f * _hexEdgeLength - _spaceBetweenHexes;
                            }
                            SpawnEmptyHex(new Vector3(xPos, yPos, zPos));
                        }
                        //Spawn others to bottom
                        else
                        {
                            float xPos = 0, yPos = 0, zPos = 0;

                            Transform previousHex = i == 0 ? null : _hexPathParent.GetChild(i - 1);
                            Transform thisHex = _hexPathParent.GetChild(i);
                            Transform nextHex = i == _hexPathParent.childCount - 1 ? null : _hexPathParent.GetChild(i + 1);

                            if (nextHex == null) continue;
                            if (previousHex == null) continue;

                            if (Mathf.Abs(thisHex.transform.position.z - nextHex.transform.position.z) < 0.1f)
                            {
                                if (thisHex.transform.position.z - previousHex.transform.position.z > 0.1f)
                                {
                                    //This is a lower left to right curve path hex
                                    xPos = thisHex.transform.position.x - (j - hexesToTopCount) * (Mathf.Sqrt(3) * _hexEdgeLength / 2 - _spaceBetweenHexes);
                                    yPos = 0;
                                    zPos = thisHex.transform.position.z - (j - hexesToTopCount) * (1.5f * _hexEdgeLength - _spaceBetweenHexes);
                                    SpawnEmptyHex(new Vector3(xPos, yPos, zPos));
                                }
                                //This is a straight path hex
                                xPos = thisHex.transform.position.x + (j - hexesToTopCount) * ((Mathf.Sqrt(3) * _hexEdgeLength) / 2 + _spaceBetweenHexes);
                                yPos = 0;
                                zPos = thisHex.transform.position.z - (j - hexesToTopCount) * (1.5f * _hexEdgeLength - _spaceBetweenHexes);
                            }
                            else if ((thisHex.transform.position.z - nextHex.transform.position.z) < -0.1f)
                            {
                                if (thisHex.transform.position.z - previousHex.transform.position.z < 0.1f) continue; //This is in a pothole
                                //This is a straight path going upwards
                                xPos = thisHex.transform.position.x - (j - hexesToTopCount) * ((Mathf.Sqrt(3) * _hexEdgeLength) / 2 - _spaceBetweenHexes);
                                yPos = 0;
                                zPos = thisHex.transform.position.z - (j - hexesToTopCount) * (1.5f * _hexEdgeLength - _spaceBetweenHexes);
                            }
                            else if ((thisHex.transform.position.z - nextHex.transform.position.z) > 0.1f)
                            {
                                //This is a straight path going downwards
                                xPos = thisHex.transform.position.x + (j - hexesToTopCount) * ((Mathf.Sqrt(3) * _hexEdgeLength) / 2 + _spaceBetweenHexes);
                                yPos = 0;
                                zPos = thisHex.transform.position.z - (j - hexesToTopCount) * (1.5f * _hexEdgeLength - _spaceBetweenHexes);
                            }
                            SpawnEmptyHex(new Vector3(xPos, yPos, zPos));
                        }
                    }
                }
            }
        }

        private bool CheckIfTaken(Vector3 position)
        {
            bool taken = false;
            foreach(Transform hex in _emptyHexParent)
            {
                if(Vector3.Distance(hex.transform.position, position) < 0.5f)
                {
                    taken = true;
                }
            }
            foreach(Transform pathHex in _hexPathParent)
            {
                if(Vector3.Distance(pathHex.transform.position, position) < 0.5f)
                {
                    taken = true;
                }
            }
            return taken;
        }

        private void SpawnEmptyHex(Vector3 position)
        {
            if (!CheckIfTaken(position))
            {
                GameObject emptyHex = Instantiate(_emptyHexPrefab, _emptyHexParent);
                emptyHex.name = "Empty hex";
                emptyHex.transform.position = position;
                emptyHex.transform.GetChild(0).rotation = Quaternion.Euler(-90, 0, 0);
            }
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

            GameObject curvePathHex = Instantiate(prefab, _hexPathParent);
            GameObject pathGO = curvePathHex.transform.GetChild(1).gameObject;
            switch (_pathEntrancePositionForThisHex)
            {
                case (PathEntrancePosition.Upper):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, 60);
                    _lastPos.x += (Mathf.Sqrt(3) * _hexEdgeLength) / 2 + _spaceBetweenHexes;
                    _lastPos.z -= _hexEdgeLength * _distanceBetweenHexesX;
                    break;
                case (PathEntrancePosition.Center):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, 0);
                    _lastPos.x += Mathf.Sqrt(3) * _hexEdgeLength + _spaceBetweenHexes;
                    break;
                case (PathEntrancePosition.Lower):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, -60);
                    _lastPos.x += (Mathf.Sqrt(3) * _hexEdgeLength) / 2 + _spaceBetweenHexes;
                    _lastPos.z += _hexEdgeLength * _distanceBetweenHexesX;
                    break;
                default:
                    break;
            }
            curvePathHex.transform.position = _lastPos;
        }

        private void SpawnStraightPathHex()
        {
            GameObject pathHex = Instantiate(_straightPathHexPrefab, _hexPathParent);
            pathHex.name = "Straight path hex";

            GameObject pathGO = pathHex.transform.GetChild(1).gameObject;
            switch(_currentPathEntrancePosition)
            {
                case (PathEntrancePosition.Upper):
                    pathGO.transform.rotation = Quaternion.Euler(-90,0,0);
                    _lastPos.x += (Mathf.Sqrt(3) * _hexEdgeLength)/2 + +_spaceBetweenHexes;
                    _lastPos.z -= _hexEdgeLength * _distanceBetweenHexesX + _spaceBetweenHexes;
                    break;
                case (PathEntrancePosition.Center):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, 120);
                    _lastPos.x += Mathf.Sqrt(3) * _hexEdgeLength + _spaceBetweenHexes;
                    break;
                case (PathEntrancePosition.Lower):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, 60);
                    _lastPos.x += (Mathf.Sqrt(3) * _hexEdgeLength)/2 + _spaceBetweenHexes;
                    _lastPos.z += _hexEdgeLength * _distanceBetweenHexesX + _spaceBetweenHexes;
                    break;
                default:
                    break;
            }
            pathHex.transform.position = _lastPos;
        }

        private void SpawnPathEntrance()
        {
            GameObject hex = Instantiate(_straightPathHexPrefab, _hexPathParent);
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
            _hexEdgeLength = _hexMapSO.HexEdgeLength;
            _spaceBetweenHexes = _hexMapSO.SpaceBetweenHexes;
            _numberOfCurves = _hexMapSO.NumberOfCurves;
        }

        private Vector3 CalculateEntrancePosition()
        {
            //int entrancePosition = UnityEngine.Random.Range(1,_height);
            float xPos = 0;
            float yPos = 0;
            float zPos = 0;
            return new Vector3(xPos,yPos,zPos);
        }

        private enum PathEntrancePosition
        {
            Upper,
            Center,
            Lower
        }
    }
}