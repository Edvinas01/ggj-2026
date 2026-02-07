using System;
using RIEVES.GGJ2026.Core.Views;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Controls
{
    internal sealed class VirtualJoystickViewController : ViewController<VirtualJoystickView>
    {
        public event Action OnDragJoystickEntered;

        public event Action OnDragJoystickExited;

        public Vector2 JoystickAxis { get; private set; }

        public bool IsJoystickDragged { get; private set; }

        private bool isMobilePlatform;

        protected override void Start()
        {
            base.Start();

            View.IsJoystickEnabled = Application.isMobilePlatform;
            isMobilePlatform = Application.isMobilePlatform;
        }

        protected override void Update()
        {
            base.Update();

            UpdatePlatformChecks();

            if (isMobilePlatform)
            {
                UpdateJoystickChecks();
            }
        }

        private void UpdatePlatformChecks()
        {
            var isMobilePlatformNext = UnityEngine.Device.Application.isMobilePlatform;
            if (isMobilePlatformNext == isMobilePlatform)
            {
                return;
            }

            isMobilePlatform = isMobilePlatformNext;

            if (isMobilePlatform == false)
            {
                JoystickAxis = Vector2.zero;
                OnDragJoystickExited?.Invoke();
            }

            View.IsJoystickEnabled = isMobilePlatform;
        }

        private void UpdateJoystickChecks()
        {
            var joystickAxisNext = View.JoystickAxis;
            if (joystickAxisNext == JoystickAxis)
            {
                return;
            }

            JoystickAxis = joystickAxisNext;

            var isJoystickDraggedNext = joystickAxisNext != Vector2.zero;
            if (isJoystickDraggedNext == IsJoystickDragged)
            {
                return;
            }

            IsJoystickDragged = isJoystickDraggedNext;

            if (IsJoystickDragged)
            {
                OnDragJoystickEntered?.Invoke();
            }
            else
            {
                OnDragJoystickExited?.Invoke();
            }
        }
    }
}
