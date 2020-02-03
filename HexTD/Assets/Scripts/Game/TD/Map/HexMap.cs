using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.TD.Map
{
    public class HexMap
    {
        //Hex edge length
        private const float _a = 1;
        //Distance between hexes in neighbour rows on x axis
        private const float _x = 0.5f;
        //Distance between hexes in neighbour rows on y axis
        private const float _y = 1.5f;
        //Height of triangle
        private const float _h = 0.8660254037844385f;

        //Injected from constructor
        private GameObject _emptyHexPrefab;
        private GameObject _straightPathHexPrefab;
        private GameObject _curveUpPathHexPrefab;
        private GameObject _curveDownPathHexPrefab;
        private int _width;
        private int _numberOfCurves;
        private float _hexEdgeLength;
        private float _spaceBetweenHexes;

        public int Width { get => _width; set => _width = value; }

        public HexMap(GameObject emptyHexPrefab, GameObject straightPathHexPrefab, GameObject curveUpPathHexPrefab, GameObject curveDownPathHexPrefab, 
            int width, int numberOfCurves, float hexEdgeLength, float spaceBetweenHexes)
        {
            _emptyHexPrefab = emptyHexPrefab;
            _straightPathHexPrefab = straightPathHexPrefab;
            _curveUpPathHexPrefab = curveUpPathHexPrefab;
            _curveDownPathHexPrefab = curveDownPathHexPrefab;
            Width = width;
            _numberOfCurves = numberOfCurves;
            _hexEdgeLength = hexEdgeLength;
            _spaceBetweenHexes = spaceBetweenHexes;
        }

        public Vector3 HexPosition(Vector2 indexes)
        {
            float xPos = 0, yPos = 0, zPos = 0;
            xPos = indexes.x * _h * 2 + indexes.y * _h + indexes.x * _spaceBetweenHexes + indexes.y * _spaceBetweenHexes / 2;
            zPos = indexes.y * _y + indexes.y * _spaceBetweenHexes;
            return new Vector3(xPos, yPos, zPos);
        }

        public Vector3 MoveVector (MoveDirection dir)
        {
            Vector2 indexes = Vector2.zero;
            Vector3 pos = Vector3.zero;
            switch (dir)
            {
                case (MoveDirection.TopRight):
                    indexes.y += 1;
                    break;
                case (MoveDirection.Right):
                    indexes.x += 1;
                    break;
                case (MoveDirection.LowRight):
                    indexes.x += 1;
                    indexes.y -= 1;
                    break;
                case (MoveDirection.LowLeft):
                    indexes.y -= 1;
                    break;
                case (MoveDirection.Left):
                    indexes.x -= 1;
                    break;
                case (MoveDirection.TopLeft):
                    indexes.x -= 1;
                    indexes.y += 1;
                    break;
            }
            return HexPosition(indexes);
        }

        public void MoveIndexes (ref Vector2 position, MoveDirection dir)
        {
            switch (dir)
            {
                case (MoveDirection.TopRight):
                    position.y += 1;
                    break;
                case (MoveDirection.Right):
                    position.x += 1;
                    break;
                case (MoveDirection.LowRight):
                    position.x += 1;
                    position.y -= 1;
                    break;
                case (MoveDirection.LowLeft):
                    position.y -= 1;
                    break;
                case (MoveDirection.Left):
                    position.x -= 1;
                    break;
                case (MoveDirection.TopLeft):
                    position.x -= 1;
                    position.y += 1;
                    break;
            }
        }

        public List<int> CalculateCurvesPositions()
        {
            List<int> curvesPositions = new List<int>();
            for (int i = 0; i < _numberOfCurves; i++)
            {
                int curvePosition = Random.Range(0, Width);
                if(!curvesPositions.Contains(curvePosition))
                    curvesPositions.Add(UnityEngine.Random.Range(0, Width));
            }
            return curvesPositions;
        }

        public Vector3 EntrancePosition()
        {
            float xPos = 0;
            float yPos = 0;
            float zPos = 0;
            return new Vector3(xPos, yPos, zPos);
        }
   
        public enum MoveDirection
        {
            TopRight,
            Right,
            LowRight,
            LowLeft,
            Left,
            TopLeft
        }
    }
}