using Game.TD.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.TD.Camera
{
    public class TDCamera : MonoBehaviour
    {
        [SerializeField]
        private GameObject _cameraPrefab;

        [SerializeField]
        [Range(-500,500)]
        private float _camZFix = -100;

        public void SetupCameraPosition(HexMap hexMap)
        {
            _cameraPrefab.transform.position = new Vector3(hexMap.MaxXZ.x + hexMap.MinXZ.x, 0, hexMap.MinXZ.y + _camZFix);
        }
    }
}