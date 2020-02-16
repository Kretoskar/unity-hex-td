using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.TD.Map
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Single instances/Hex types", fileName = "Hex types")]
    public class HexTypesSO : ScriptableObject
    {
        [SerializeField]
        private GameObject _emptyHexPrefab;

        [SerializeField]
        private GameObject _straightPathHexPrefab;

        [SerializeField]
        private GameObject _curveUpPathHexPrefab;

        [SerializeField]
        private GameObject _curveDownPathHexPrefab;

        public GameObject EmptyHexPrefab { get => _emptyHexPrefab; set => _emptyHexPrefab = value; }
        public GameObject StraightPathHexPrefab { get => _straightPathHexPrefab; set => _straightPathHexPrefab = value; }
        public GameObject CurveUpPathHexPrefab { get => _curveUpPathHexPrefab; set => _curveUpPathHexPrefab = value; }
        public GameObject CurveDownPathHexPrefab { get => _curveDownPathHexPrefab; set => _curveDownPathHexPrefab = value; }
    }
}
