using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.TD.Mobs
{
    public class BaseMob : Mob
    {
        [SerializeField]
        protected BaseMobSO _mobSO;

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            InjectDataFromScriptableObject();
        }

        protected override void Start()
        {
            base.Start();
            _animator.SetBool("Grounded", true);
            _animator.SetFloat("MoveSpeed", _speed);
        }

        private void InjectDataFromScriptableObject()
        {
            _speed = _mobSO.Speed;
        }
    }
}