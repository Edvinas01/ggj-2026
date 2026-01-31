using System;
using UnityEngine;

namespace RIEVES.GGJ2026.Core.Utilities
{
    public sealed class RigidBodyVelocityFollower : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody rigidBody;

        [Min(0f)]
        [SerializeField]
        private float rotationSpeed = 5f;

        private void OnDrawGizmosSelected()
        {
            if (rigidBody == false)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawRay(rigidBody.position, rigidBody.linearVelocity);
        }

        private void LateUpdate()
        {
            var direction = rigidBody.linearVelocity;
            direction.y = 0;

            if (direction.sqrMagnitude < 0.001f)
            {
                return;
            }

            var targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
        }
    }
}
