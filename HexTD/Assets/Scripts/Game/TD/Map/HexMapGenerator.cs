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

        #endregion

        private void Awake()
        {
            _hexPathParent = transform;
            InjectDataFromScriptableObject();
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

        private void InjectDataFromScriptableObject()
        {
            _emptyHexPrefab = _hexTypesSO.EmptyHexPrefab;
            _straightPathHexPrefab = _hexTypesSO.StraightPathHexPrefab;
            _curveUpPathHexPrefab = _hexTypesSO.CurveUpPathHexPrefab;
            _curveDownPathHexPrefab = _hexTypesSO.CurveDownPathHexPrefab;
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
                if (curvesPositions.Contains(i))
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
            _currentPathEntrancePosition = PathEntrancePosition.CenterLeft;
        }

        private void SpawnCurvedPathHex()
        {
            GameObject prefab;
            bool isCurvedToTop = UnityEngine.Random.Range(0,2) == 0;
            PathEntrancePosition pathEntrancePositionForThisHex = _currentPathEntrancePosition;

            if (isCurvedToTop)
            {
                prefab = _curveUpPathHexPrefab;
            }
            else
            {
                prefab = _curveDownPathHexPrefab;
            }

            _currentPathEntrancePosition = NextPathEntrance(isCurvedToTop);

            GameObject curvePathHex = Instantiate(prefab, _hexPathParent);
            GameObject pathGO = curvePathHex.transform.GetChild(1).gameObject;

            print("Entrance for this hex: " + pathEntrancePositionForThisHex);

            switch (pathEntrancePositionForThisHex)
            {
                case (PathEntrancePosition.UpperLeft):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, 60);
                    _hexMap.MoveIndexes(ref _lastPos, HexMap.MoveDirection.LowRight);
                    break;
                case (PathEntrancePosition.CenterLeft):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, 0);
                    _hexMap.MoveIndexes(ref _lastPos, HexMap.MoveDirection.Right);
                    break;
                case (PathEntrancePosition.LowerLeft):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, -60);
                    _hexMap.MoveIndexes(ref _lastPos, HexMap.MoveDirection.TopRight);
                    break;
                case (PathEntrancePosition.UpperRight):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, isCurvedToTop ? 0 : -120) ;
                    _hexMap.MoveIndexes(ref _lastPos, HexMap.MoveDirection.LowLeft);
                    break;
                case (PathEntrancePosition.CenterRight):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, isCurvedToTop ? 60 : -60);
                    _hexMap.MoveIndexes(ref _lastPos, HexMap.MoveDirection.Left);
                    break;
                case (PathEntrancePosition.LowerRight):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, isCurvedToTop ? 120 : 0);
                    _hexMap.MoveIndexes(ref _lastPos, HexMap.MoveDirection.TopLeft);
                    break;
                default:
                    break;
            }
            curvePathHex.transform.position = _hexMap.HexPosition(_lastPos);
        }

        private void SpawnStraightPathHex()
        {
            GameObject pathHex = Instantiate(_straightPathHexPrefab, _hexPathParent);
            pathHex.name = "Straight path hex";

            print("Entrance for this hex: " + _currentPathEntrancePosition);

            GameObject pathGO = pathHex.transform.GetChild(1).gameObject;
            switch (_currentPathEntrancePosition)
            {
                case (PathEntrancePosition.UpperLeft):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, 0);
                    _hexMap.MoveIndexes(ref _lastPos, HexMap.MoveDirection.LowRight);
                    break;
                case (PathEntrancePosition.CenterLeft):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, 120);
                    _hexMap.MoveIndexes(ref _lastPos, HexMap.MoveDirection.Right);
                    break;
                case (PathEntrancePosition.LowerLeft):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, 60);
                    _hexMap.MoveIndexes(ref _lastPos, HexMap.MoveDirection.TopRight);
                    break;
                case (PathEntrancePosition.UpperRight):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, 60);
                    _hexMap.MoveIndexes(ref _lastPos, HexMap.MoveDirection.LowLeft);
                    break;
                case (PathEntrancePosition.CenterRight):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, 120);
                    _hexMap.MoveIndexes(ref _lastPos, HexMap.MoveDirection.Left);
                    break;
                case (PathEntrancePosition.LowerRight):
                    pathGO.transform.rotation = Quaternion.Euler(-90, 0, 0);
                    _hexMap.MoveIndexes(ref _lastPos, HexMap.MoveDirection.TopLeft);
                    break;
                default:
                    break;
            }
            pathHex.transform.position = _hexMap.HexPosition(_lastPos);
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
                        //Spawn hexes to top
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
                                SpawnEmptyHex(thisHex.transform.position + j * _hexMap.HexPosition(pos));
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
                            moveDir = HexMap.MoveDirection.TopRight;
                        }
                        SpawnEmptyHex(thisHex.transform.position + j * _hexMap.MoveVector(moveDir));
                    }
                    else
                    {
                        //Spawn hexes to bottom
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
                                _hexMap.MoveIndexes(ref pos, HexMap.MoveDirection.LowLeft);
                                SpawnEmptyHex(thisHex.transform.position + (j - hexesToTopCount) * _hexMap.HexPosition(pos));
                            }
                            //This is a straight path hex
                            moveDir = HexMap.MoveDirection.LowRight;
                        }
                        else if ((thisHex.transform.position.z - nextHex.transform.position.z) < -0.1f)
                        {
                            if (thisHex.transform.position.z - previousHex.transform.position.z < 0.1f) continue; //This is in a pothole
                            //This is a straight path going upwards
                                moveDir = HexMap.MoveDirection.LowLeft;
                        }
                        else if ((thisHex.transform.position.z - nextHex.transform.position.z) > 0.1f)
                        {
                            //This is a straight path going downwards
                            moveDir = HexMap.MoveDirection.LowRight;
                        }
                        SpawnEmptyHex(thisHex.transform.position + (j - hexesToTopCount) * _hexMap.MoveVector(moveDir));
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

        private PathEntrancePosition NextPathEntrance(bool isCurvedToTop)
        {
            int posIndex = (int)_currentPathEntrancePosition;
            int nexPos;
            if (posIndex == 1 || posIndex > 4)
            {
                nexPos = isCurvedToTop ? posIndex - 1 : posIndex + 1;
            }
            else
            {
                nexPos = isCurvedToTop ? posIndex + 1 : posIndex - 1;
            }
            if (nexPos == 7)
                nexPos = 1;
            if (nexPos == 0)
                nexPos = 6;
            return (PathEntrancePosition)nexPos;
        }

        private enum PathEntrancePosition
        {
            UpperLeft = 1,
            CenterLeft = 6,
            LowerLeft = 5,
            UpperRight = 2,
            CenterRight = 3,
            LowerRight = 4
        }
    }
}