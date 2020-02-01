using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.TD.Map
{
    public class Hex : MonoBehaviour
    {
        private Material _material;
        private Color _startingColor;

        private void Awake()
        {
            _material = gameObject.GetComponent<Renderer>().material;
            _startingColor = _material.color;
        }

        private void OnMouseOver()
        {
            _material.color = Color.red;
        }

        private void OnMouseExit()
        {
            _material.color = _startingColor;
        }
    }
}