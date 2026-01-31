using System.Collections.Generic;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    internal sealed class CharacterWaypointMover : MonoBehaviour
    {
        [SerializeField]
        private CharacterActor character;

        [Min(0f)]
        [SerializeField]
        private float reachThreshold = 0.1f;

        [SerializeField]
        private List<Transform> waypoints;

        private Transform currentWaypoint;

        private void Start()
        {
            PickRandomWaypoint();
        }

        private void Update()
        {
            if (waypoints.Count == 0)
            {
                return;
            }

            character.TargetPosition = currentWaypoint.position;

            var distanceSqr = (character.transform.position - currentWaypoint.position).sqrMagnitude;
            if (distanceSqr <= reachThreshold * reachThreshold)
            {
                PickRandomWaypoint();
            }
        }

        private void PickRandomWaypoint()
        {
            if (waypoints.Count == 0)
            {
                return;
            }

            currentWaypoint = waypoints[Random.Range(0, waypoints.Count)];
        }
    }
}
