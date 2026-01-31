using UnityEngine;
using UnityEngine.Events;

namespace RIEVES.GGJ2026.Runtime.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    internal sealed class MovementController : MonoBehaviour
    {
        [Header("General")]
        [SerializeField]
        private MovementInputProvider inputProvider;

        [SerializeField]
        private Transform forwardSource;

        [Header("Forces")]
        [SerializeField]
        private AnimationCurve forceBySpeed;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onMoveEntered;

        [SerializeField]
        private UnityEvent onMoveExited;

        private Rigidbody rigidBody;
        private bool isMoving;

        public bool IsMoving
        {
            get => isMoving;
            set
            {
                var isMovingPrev = isMoving;
                var isMovingNext = value;

                if (isMovingPrev == isMovingNext)
                {
                    return;
                }

                isMoving = isMovingNext;

                if (isMovingNext)
                {
                    onMoveEntered.Invoke();
                }
                else
                {
                    onMoveExited.Invoke();
                }
            }
        }

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            var axis = inputProvider.MoveAxis;

            var forward = forwardSource.forward;
            forward.y = 0f;
            forward.Normalize();

            var right = Vector3.Cross(Vector3.up, forward);

            var moveDirection = forward * axis.y + right * axis.x;
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

            var velocity = rigidBody.linearVelocity;
            var planarSpeed = new Vector3(velocity.x, 0f, velocity.z).magnitude;

            var moveForce = forceBySpeed.Evaluate(planarSpeed);
            var force = moveDirection * moveForce;

            rigidBody.AddForce(force, ForceMode.Acceleration);

            IsMoving = axis.sqrMagnitude > 0.001f && planarSpeed > 0.001f;
        }
    }
}
