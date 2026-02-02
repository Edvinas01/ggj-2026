using System;
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

        [SerializeField]
        private AnimationCurve forceMulByAngle;

        [Header("Ground")]
        [SerializeField]
        private float groundCheckDistance = 5f;

        [SerializeField]
        private float groundCheckCooldown = 0.3f;

        [SerializeField]
        private LayerMask groundMask;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onMoveEntered;

        [SerializeField]
        private UnityEvent onMoveExited;

        private RaycastHit groundHit;
        private Rigidbody rigidBody;

        private float groundedCooldown;
        private bool isGrounded;
        private bool isMoving;

        public event Action OnMoveEntered;
        public event Action OnMoveExited;

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
                    OnMoveEntered?.Invoke();
                    onMoveEntered.Invoke();
                }
                else
                {
                    OnMoveExited?.Invoke();
                    onMoveExited.Invoke();
                }
            }
        }

        public float Speed => rigidBody.linearVelocity.magnitude;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (groundedCooldown <= 0f)
            {
                UpdateGroundChecks();
                groundedCooldown = groundCheckCooldown;
            }
            groundedCooldown -= Time.deltaTime;
            UpdateMovement();
        }

        private void UpdateGroundChecks()
        {
            isGrounded = Physics.Raycast(
                origin: transform.position + Vector3.up * 0.1f,
                direction: Vector3.down,
                layerMask: groundMask,
                hitInfo: out groundHit,
                maxDistance: groundCheckDistance
            );
        }

        private void UpdateMovement()
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

            if (isGrounded)
            {
                var angle = Vector3.Angle(groundHit.normal, Vector3.up);
                if (angle > 0f)
                {
                    var mul = forceMulByAngle.Evaluate(angle);
                    force *= mul;
                }
            }

            rigidBody.AddForce(force, ForceMode.Acceleration);

            IsMoving = axis.sqrMagnitude > 0.001f && planarSpeed > 0.001f;
        }
    }
}
