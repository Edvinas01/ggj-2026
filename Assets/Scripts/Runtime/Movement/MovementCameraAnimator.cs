using System;
using Unity.Cinemachine;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Movement
{
    internal sealed class MovementCameraAnimator : MonoBehaviour
    {
        [SerializeField]
        private MovementController movementController;

        [SerializeField]
        private CinemachineBasicMultiChannelPerlin cinemachineNoise;

        [Min(0f)]
        [SerializeField]
        private float maxSpeed = 5f;

        [Min(0f)]
        [SerializeField]
        private float movingLerpSpeed = 5f;

        [Min(0f)]
        [SerializeField]
        private float stopLerpSpeed = 15f;

        private float initialAmplitude;

        private float currentMultiplier;
        private float targetMultiplier;

        private void Awake()
        {
            initialAmplitude = cinemachineNoise.AmplitudeGain;
        }

        private void Update()
        {
            targetMultiplier = Mathf.InverseLerp(0f, maxSpeed, movementController.Speed);

            currentMultiplier = Mathf.Lerp(
                currentMultiplier,
                targetMultiplier,
                Time.deltaTime * (movementController.IsMoving ? movingLerpSpeed : stopLerpSpeed)
            );

            if (currentMultiplier <= 0.0001f)
            {
                currentMultiplier = 0f;
            }

            cinemachineNoise.AmplitudeGain = initialAmplitude * currentMultiplier;
        }
    }
}
