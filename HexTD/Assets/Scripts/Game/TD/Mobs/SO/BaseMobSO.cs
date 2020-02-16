using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Mobs/Base mob", fileName = "Base mob")]
public class BaseMobSO : ScriptableObject
{
    [SerializeField]
    [Range(0.01f, 10000)]
    private float _speed;

    public float Speed { get => _speed; set => _speed = value; }

}
