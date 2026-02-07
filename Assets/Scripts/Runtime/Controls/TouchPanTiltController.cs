using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;

namespace RIEVES.GGJ2026.Runtime.Controls
{
    internal sealed class TouchPanTiltController : MonoBehaviour
    {
        [SerializeField]
        private CinemachineInputAxisController panTiltInput;

        [SerializeField]
        private CinemachinePanTilt panTilt;

        [Min(0f)]
        [SerializeField]
        private float panLerpSpeed = 5f;

        [FormerlySerializedAs("panSpeed")]
        [Min(0f)]
        [SerializeField]
        private float panSensitivity = 0.1f;

        private bool isTouching;

        public bool IsTouching
        {
            get => isTouching;
            private set
            {
                var isTouchingNext = value;
                if (isTouching == isTouchingNext)
                {
                    return;
                }

                isTouching = isTouchingNext;

                if (isTouchingNext == false)
                {
                    currentTouchDelta =  Vector2.zero;
                    targetTouchDelta = Vector2.zero;
                }
            }
        }

        private readonly List<RaycastResult> touchRaycastResults = new();
        private readonly List<TouchControl> validTouches = new();

        private Vector2 currentTouchDelta;
        private Vector2 targetTouchDelta;

        private void Update()
        {
            var isMobilePlatform = UnityEngine.Device.Application.isMobilePlatform;
            if (isMobilePlatform == false)
            {
                panTiltInput.enabled = true;
                return;
            }

            panTiltInput.enabled = false;

            var touchScreen = Touchscreen.current;
            if (touchScreen == null)
            {
                IsTouching = false;
                return;
            }

            var touches = touchScreen.touches;

            validTouches.Clear();
            for (var index = 0; index < touches.Count && index < 2; index++)
            {
                var touchControl = touches[index];
                if (touchControl.isInProgress == false)
                {
                    continue;
                }

                var eventData = new PointerEventData(EventSystem.current)
                {
                    position = touchControl.position.ReadValue(),
                };

                EventSystem.current.RaycastAll(eventData, touchRaycastResults);
                var isOverUI = touchRaycastResults.Count > 0;
                if (isOverUI)
                {
                    continue;
                }

                validTouches.Add(touchControl);
            }

            if (validTouches.Count == 0)
            {
                IsTouching = false;
                return;
            }

            targetTouchDelta = Vector2.zero;
            IsTouching = true;

            foreach (var touchControl in validTouches)
            {
                targetTouchDelta += touchControl.delta.ReadValue();
            }

            currentTouchDelta = Vector2.Lerp(currentTouchDelta, targetTouchDelta, panLerpSpeed * Time.deltaTime);
            if ((currentTouchDelta - targetTouchDelta).sqrMagnitude < 0.0001f)
            {
                currentTouchDelta = targetTouchDelta;
            }

            panTilt.TiltAxis.Value -= currentTouchDelta.y * panSensitivity;
            panTilt.PanAxis.Value += currentTouchDelta.x * panSensitivity;
        }
    }
}
