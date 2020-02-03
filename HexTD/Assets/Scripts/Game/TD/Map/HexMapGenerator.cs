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
        private Vector2 _lastPos;
        private Transform _hexPathParent;
        private Transform _emptyHexParent;
        private PathEntrancePosition _currentPathEntrancePosition;
        private HexMap _hexMap;

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
            InjectDataFromScriptableObject();
        }

        private void InjectDataFromScriptableObject()
        {
            _emptyHexPrefab = _hexTypesSO.EmptyHexPrefab;
            _straightPathHexPrefab = _hexTypesSO.StraightPathHexPrefab;
            _curveUpPathHexPrefab = _hexTypesSO.CurveUpPathHexPrefab;
            _curveDownPathHexPrefab = _hexTypesSO.CurveDownPathHexPrefab;
        }

        private void Start()
        {
            GameObject hexParentGO = new GameObject("Hex path");
            hexParentGO.transform.parent = transform;
            _hexPathParent = hexParentGO.transform;

            GameObject emptyHexParentGO = new GameObject("Empty hexes");
            emptyHexParentGO.transform.parent = transform;
            _emptyHexParent = emptyHexParentGO.transform;

            GenerateHexMapObject();
            SpawnPath();
            SpawnEmptyHexes();
        }

        private void GenerateHexMapObject()
        {
            _hexMap = new HexMap(_hexTypesSO.EmptyHexPrefab, _hexTypesSO.StraightPathHexPrefab, _hexTypesSO.CurveUpPathHexPrefab, _hexTypesSO.CurveDownPathHexPrefab,
                _hexMapSO.Width, _hexMapSO.NumberOfCurves, _hexMapSO.HexEdgeLength, _hexMapSO.SpaceBetweenHexes);
        }

        private void SpawnPath()
        {
            List<int> curvesPositions = _hexMap.CalculateCurvesPositions();

            //Spawn entrance
            SpawnPathEntrance();

            //Spawn rest
            for(int i = 0; i < _hexMap.Width; i++) {
                print(_lastPos.x);
                if (curvesPositions.Contains((int)_lastPos.x))
                {
                    SpawnCurvedPathHex();
                }
                else
                {
                    //Spawn straight path
                    SpawnStraightPathHex();
                }
            }
        }

        private void SpawnPathEntrance()
        {
            GameObject hex = Instantiate(_straightPathHexPrefab, _hexPathParent);
            hex.name = "Path entrance hes";
            _lastPos = _hexMap.EntrancePosition();
            hex.transform.position = _lastPos;
            _currentPathEntrancePosition = PathEntrancePosition.Center;
        }

        private void SpawnCurvedPathHex()
        {
            GameObject prefab;
            bool isCurvedToTop = false;
            PathEntrancePosition _pathEntrancePositionForThisHex = _currentPathEntrancePosition;

            //Spawn curved path
            switch (_currentPathEntrancePosition)
            {
                case (PathEntrancePosition.Upper):
                    isCurvedToTop = true;
                    _currentPathEntrancePosition = PathEntrancePosition.Center;
                    break;
                case (PathEntrancePosition.Center):
                    isCurvedToTop = UnityEngine.Random.Range(0, 2) == 0;
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
                    _hexMap.MoveIndexes(ref _lastPos, HexMap.MoveDirection.LowRight);
                    break;
                case (PathEntrancePosition.Center):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, 0);
                    _hexMap.MoveIndexes(ref _lastPos, HexMap.MoveDirection.Right);
                    break;
                case (PathEntrancePosition.Lower):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, -60);
                    _hexMap.MoveIndexes(ref _lastPos, HexMap.MoveDirection.TopRight);
                    break;
                default:
                    break;
            }
            curvePathHex.transform.position = _hexMap.HexPositionRight(_lastPos);
        }

        private void SpawnStraightPathHex()
        {
            GameObject pathHex = Instantiate(_straightPathHexPrefab, _hexPathParent);
            pathHex.name = "Straight path hex";

            GameObject pathGO = pathHex.transform.GetChild(1).gameObject;
            switch (_currentPathEntrancePosition)
            {
                case (PathEntrancePosition.Upper):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, 0);
                    _hexMap.MoveIndexes(ref _lastPos, HexMap.MoveDirection.LowRight);
                    break;
                case (PathEntrancePosition.Center):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, 120);
                    _hexMap.MoveIndexes(ref _lastPos, HexMap.MoveDirection.Right);
                    break;
                case (PathEntrancePosition.Lower):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, 60);
                    _hexMap.MoveIndexes(ref _lastPos, HexMap.MoveDirection.TopRight);
                    break;
                default:
                    break;
            }
            pathHex.transform.position = _hexMap.HexPositionRight(_lastPos);
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
                            Transform previousHex = i == 0 ? null : _hexPathParent.GetChild(i - 1);
                            Transform thisHex = _hexPathParent.GetChild(i);
                            Transform nextHex = i == _hexPathParent.childCount - 1 ? null : _hexPathParent.GetChild(i + 1);

                            HexMap.MoveDirection moveDir = 0;

                            if (nextHex == null) continue;
                            if (previousHex == null) continue;

                            if (Mathf.Abs(thisHex.transform.position.z - nextHex.transform.position.z) < 0.1f)
                            {
                                if (thisHex.transform.position.z - previousHex.transform.position.z > 0.1f)
                                {
                                    //This is a lower left to right curve path hex
                                    Vector2 pos = Vector2.zero;
                                    _hexMap.MoveIndexes(ref pos, HexMap.MoveDirection.TopLeft);
                                    SpawnEmptyHex(thisHex.transform.position + _hexMap.HexPositionRight(pos));
                                }
                                moveDir = HexMap.MoveDirection.TopRight;
                            }
                            else if ((thisHex.transform.position.z - nextHex.transform.position.z) < -0.1f)
                            {
                                if (thisHex.transform.position.z - previousHex.transform.position.z < 0.1f) continue; //This is in a pothole
                                //This is a straight path going upwards
                                moveDir = HexMap.MoveDirection.TopLeft;
                            }
                            else if ((thisHex.transform.position.z - nextHex.transform.position.z) > 0.1f)
                            {
                                //This is a straight path going downwards
                                moveDir = HexMap.MoveDirection.TopRight;
                            }
                            SpawnEmptyHex(thisHex.transform.position + _hexMap.MoveVector(moveDir));
                        }
                    //    //Spawn the other hexes to the top
                    //    else
                    //    {
                    //        float xPos = 0, yPos = 0, zPos = 0;

                    //        Transform previousHex = i == 0 ? null : _hexPathParent.GetChild(i - 1);
                    //        Transform thisHex = _hexPathParent.GetChild(i);
                    //        Transform nextHex = i == _hexPathParent.childCount - 1 ? null : _hexPathParent.GetChild(i + 1);

                    //        if (nextHex == null) continue;
                    //        if (previousHex == null) continue;

                    //        if (Mathf.Abs(thisHex.transform.position.z - nextHex.transform.position.z) < 0.1f)
                    //        {
                    //            if (thisHex.transform.position.z - previousHex.transform.position.z > 0.1f)
                    //            {
                    //                //This is a lower left to right curve path hex
                    //                xPos = thisHex.transform.position.x -  j *((Mathf.Sqrt(3) * _hexEdgeLength) / 2 - _spaceBetweenHexes);
                    //                yPos = 0;
                    //                zPos = thisHex.transform.position.z + j * 1.5f * (_hexEdgeLength + _spaceBetweenHexes);
                    //                SpawnEmptyHex(new Vector3(xPos, yPos, zPos));
                    //            }
                    //            //This is a straight path hex
                    //            xPos = thisHex.transform.position.x + j * ((Mathf.Sqrt(3) * _hexEdgeLength) / 2 + _spaceBetweenHexes);
                    //            yPos = 0;
                    //            zPos = thisHex.transform.position.z + j * ((1.5f * _hexEdgeLength + _spaceBetweenHexes));
                    //        }
                    //        else if ((thisHex.transform.position.z - nextHex.transform.position.z) < -0.1f)
                    //        {
                    //            if (thisHex.transform.position.z - previousHex.transform.position.z < 0.1f) continue; //This is in a pothole
                    //                                                                                                  //This is a straight path going upwards
                    //            xPos = thisHex.transform.position.x - j * ((Mathf.Sqrt(3) * _hexEdgeLength) / 2 - _spaceBetweenHexes);
                    //            yPos = 0;
                    //            zPos = thisHex.transform.position.z + (j * 1.5f * _hexEdgeLength + _spaceBetweenHexes);
                    //        }
                    //        else if ((thisHex.transform.position.z - nextHex.transform.position.z) > 0.1f)
                    //        {
                    //            //This is a straight path going downwards
                    //            xPos = thisHex.transform.position.x + (j * (Mathf.Sqrt(3) * _hexEdgeLength) / 2 + _spaceBetweenHexes);
                    //            yPos = 0;
                    //            zPos = thisHex.transform.position.z + j * 1.5f * (_hexEdgeLength + _spaceBetweenHexes);
                    //        }
                    //        SpawnEmptyHex(new Vector3(xPos, yPos, zPos));
                    //    }
                    //}
                    //else
                    //{
                    //    //Spawn first to bottom
                    //    if (j == emptyHexCount - hexesToTopCount - 1)
                    //    {
                    //        float xPos = 0, yPos = 0, zPos = 0;

                    //        Transform previousHex = i == 0 ? null : _hexPathParent.GetChild(i - 1);
                    //        Transform thisHex = _hexPathParent.GetChild(i);
                    //        Transform nextHex = i == _hexPathParent.childCount - 1 ? null : _hexPathParent.GetChild(i + 1);

                    //        if (nextHex == null) continue;
                    //        if (previousHex == null) continue;

                    //        if (Mathf.Abs(thisHex.transform.position.z - nextHex.transform.position.z) < 0.1f)
                    //        {
                    //            if (thisHex.transform.position.z - previousHex.transform.position.z > 0.1f)
                    //            {
                    //                //This is a lower left to right curve path hex
                    //                xPos = thisHex.transform.position.x - (Mathf.Sqrt(3) * _hexEdgeLength) / 2 - _spaceBetweenHexes;
                    //                yPos = 0;
                    //                zPos = thisHex.transform.position.z - 1.5f * _hexEdgeLength - _spaceBetweenHexes;
                    //                SpawnEmptyHex(new Vector3(xPos, yPos, zPos));
                    //            }
                    //            //This is a straight path hex
                    //            xPos = thisHex.transform.position.x + (Mathf.Sqrt(3) * _hexEdgeLength) / 2 + _spaceBetweenHexes;
                    //            yPos = 0;
                    //            zPos = thisHex.transform.position.z - 1.5f * _hexEdgeLength - _spaceBetweenHexes;
                    //        }
                    //        else if ((thisHex.transform.position.z - nextHex.transform.position.z) < -0.1f)
                    //        {
                    //            if (thisHex.transform.position.z - previousHex.transform.position.z < 0.1f) continue; //This is in a pothole
                    //                                                                                                  //This is a straight path going upwards
                    //            xPos = thisHex.transform.position.x - (Mathf.Sqrt(3) * _hexEdgeLength) / 2 - _spaceBetweenHexes;
                    //            yPos = 0;
                    //            zPos = thisHex.transform.position.z - 1.5f * _hexEdgeLength - _spaceBetweenHexes;
                    //        }
                    //        else if ((thisHex.transform.position.z - nextHex.transform.position.z) > 0.1f)
                    //        {
                    //            //This is a straight path going downwards
                    //            xPos = thisHex.transform.position.x + (Mathf.Sqrt(3) * _hexEdgeLength) / 2 + _spaceBetweenHexes;
                    //            yPos = 0;
                    //            zPos = thisHex.transform.position.z - 1.5f * _hexEdgeLength - _spaceBetweenHexes;
                    //        }
                    //        SpawnEmptyHex(new Vector3(xPos, yPos, zPos));
                    //    }
                    //    //Spawn others to bottom
                    //    else
                    //    {
                    //        float xPos = 0, yPos = 0, zPos = 0;

                    //        Transform previousHex = i == 0 ? null : _hexPathParent.GetChild(i - 1);
                    //        Transform thisHex = _hexPathParent.GetChild(i);
                    //        Transform nextHex = i == _hexPathParent.childCount - 1 ? null : _hexPathParent.GetChild(i + 1);

                    //        if (nextHex == null) continue;
                    //        if (previousHex == null) continue;

                    //        if (Mathf.Abs(thisHex.transform.position.z - nextHex.transform.position.z) < 0.1f)
                    //        {
                    //            if (thisHex.transform.position.z - previousHex.transform.position.z > 0.1f)
                    //            {
                    //                //This is a lower left to right curve path hex
                    //                xPos = thisHex.transform.position.x - (j - hexesToTopCount) * (Mathf.Sqrt(3) * _hexEdgeLength / 2 - _spaceBetweenHexes);
                    //                yPos = 0;
                    //                zPos = thisHex.transform.position.z - (j - hexesToTopCount) * (1.5f * _hexEdgeLength - _spaceBetweenHexes);
                    //                SpawnEmptyHex(new Vector3(xPos, yPos, zPos));
                    //            }
                    //            //This is a straight path hex
                    //            xPos = thisHex.transform.position.x + (j - hexesToTopCount) * ((Mathf.Sqrt(3) * _hexEdgeLength) / 2 + _spaceBetweenHexes);
                    //            yPos = 0;
                    //            zPos = thisHex.transform.position.z - (j - hexesToTopCount) * (1.5f * _hexEdgeLength - _spaceBetweenHexes);
                    //        }
                    //        else if ((thisHex.transform.position.z - nextHex.transform.position.z) < -0.1f)
                    //        {
                    //            if (thisHex.transform.position.z - previousHex.transform.position.z < 0.1f) continue; //This is in a pothole
                    //            //This is a straight path going upwards
                    //            xPos = thisHex.transform.position.x - (j - hexesToTopCount) * ((Mathf.Sqrt(3) * _hexEdgeLength) / 2 - _spaceBetweenHexes);
                    //            yPos = 0;
                    //            zPos = thisHex.transform.position.z - (j - hexesToTopCount) * (1.5f * _hexEdgeLength - _spaceBetweenHexes);
                    //        }
                    //        else if ((thisHex.transform.position.z - nextHex.transform.position.z) > 0.1f)
                    //        {
                    //            //This is a straight path going downwards
                    //            xPos = thisHex.transform.position.x + (j - hexesToTopCount) * ((Mathf.Sqrt(3) * _hexEdgeLength) / 2 + _spaceBetweenHexes);
                    //            yPos = 0;
                    //            zPos = thisHex.transform.position.z - (j - hexesToTopCount) * (1.5f * _hexEdgeLength - _spaceBetweenHexes);
                    //        }
                    //        SpawnEmptyHex(new Vector3(xPos, yPos, zPos));
                    //    }
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

        private enum PathEntrancePosition
        {
            Upper,
            Center,
            Lower
        }
    }
}