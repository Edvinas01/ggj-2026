using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Movement
{
    internal sealed class MovementPositionInputProvider : MovementInputProvider
    {
        private const float DestinationReachedDistanceSqrThreshold = 0.0001f;

        private bool isTargetPositionSet;

        private Vector3 targetPosition;
        private Vector2 moveAxis;

        public Vector3 TargetPosition
        {
            get => targetPosition;
            set
            {
                isTargetPositionSet = true;
                targetPosition = value;
            }
        }

        public override Vector2 MoveAxis => moveAxis;

        private void OnDrawGizmos()
        {
            if (isTargetPositionSet == false)
            {
                return;
            }

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, TargetPosition);
            Gizmos.DrawSphere(TargetPosition, 0.3f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, new Vector3(moveAxis.x, 0, moveAxis.y));
        }

        private void Update()
        {
            if (isTargetPositionSet == false)
            {
                return;
            }

            var delta = TargetPosition - transform.position;
            delta.y = 0f;

            if (delta.sqrMagnitude < DestinationReachedDistanceSqrThreshold)
            {
                moveAxis = Vector2.zero;
                isTargetPositionSet = false;
                return;
            }

            moveAxis = new Vector2(delta.x, delta.z).normalized;
        }
    }
}
