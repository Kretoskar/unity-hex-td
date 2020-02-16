using Game.TD.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.TD.Mobs
{
    public class Mob : MonoBehaviour
    {
        [SerializeField]
        protected float _waypointTolerance = 0.1f;

        private int _currentWaypointIndex;
        protected HexMapGenerator _hexMapGenerator;
        protected float _speed;

        public float Speed { get => _speed; set => _speed = value; }

        protected virtual void Start()
        {
            _currentWaypointIndex = 0;
            _hexMapGenerator = FindObjectOfType<HexMapGenerator>();
            transform.position = _hexMapGenerator.Waypoints[0].transform.position;
        }

        protected virtual void Update()
        {
            Move();
        }

        protected virtual void Move()
        {
            if (ReachedWaypoint())
            {
                CycleWaypoints();
            }

            RotateTowardsNextWaypoint();
            MoveTowardsNextWaypoint();
        }

        private void MoveTowardsNextWaypoint()
        {
            transform.position = Vector3.MoveTowards(transform.position,
                _hexMapGenerator.Waypoints[_currentWaypointIndex].transform.position,
                _speed * Time.deltaTime);
        }

        private void CycleWaypoints()
        {
            _currentWaypointIndex++;
            if (_currentWaypointIndex == _hexMapGenerator.Waypoints.Count)
            {
                Destroy(gameObject);
            }
        }

        private void RotateTowardsNextWaypoint()
        {
            Vector3 targetDirection = _hexMapGenerator.Waypoints[_currentWaypointIndex].transform.position - transform.position;
            float singleStep = _speed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }

        private bool ReachedWaypoint()
        {
            return Vector3.Distance(transform.position, _hexMapGenerator.Waypoints[_currentWaypointIndex].transform.position) < _waypointTolerance;
        }
    }
}
