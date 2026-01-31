using UnityEngine;
using UnityEngine.Events;

namespace RIEVES.GGJ2026.Core.Utilities
{
    internal sealed class Billboard : MonoBehaviour
    {
        [System.Serializable]
        private sealed class AngleStep
        {
            [SerializeField]
            [Range(-180f, 180f)]
            private float minAngle;

            [SerializeField]
            [Range(-180f, 180f)]
            private float maxAngle;

            [SerializeField]
            private UnityEvent onEnter;

            [SerializeField]
            private UnityEvent onExit;

            public float MinAngle => minAngle;

            public float MaxAngle => maxAngle;

            public void TriggerEnter()
            {
                onEnter.Invoke();
            }

            public void TriggerExit()
            {
                onExit.Invoke();
            }
        }

        [SerializeField]
        private bool isFlipDirection;

        [SerializeField]
        private Transform angleReference;

        [SerializeField]
        private AngleStep[] angleSteps;

        private Camera mainCamera;
        private AngleStep currentStep;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (!mainCamera)
                return;

            var direction = mainCamera.transform.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                return;

            transform.rotation = Quaternion.LookRotation(
                isFlipDirection ? -direction : direction
            );

            if (angleReference == false)
            {
                return;
            }

            var angle = GetCameraRelativeAngle(direction);

            var newStep = GetStepForAngle(angle);
            if (newStep != null && newStep != currentStep)
            {
                currentStep?.TriggerExit();
                currentStep = newStep;
                currentStep.TriggerEnter();
            }
        }

        private float GetCameraRelativeAngle(Vector3 cameraDirection)
        {
            var forward = angleReference.forward;
            cameraDirection.y = 0f;

            return Vector3.SignedAngle(forward, cameraDirection, Vector3.up);
        }

        private AngleStep GetStepForAngle(float angle)
        {
            foreach (var step in angleSteps)
            {
                if (step.MinAngle <= step.MaxAngle)
                {
                    if (angle >= step.MinAngle && angle <= step.MaxAngle)
                    {
                        return step;
                    }
                }
                else
                {
                    if (angle >= step.MinAngle || angle <= step.MaxAngle)
                    {
                        return step;
                    }
                }
            }

            return null;
        }
    }
}
