using System;
using System.Collections.Generic;
using CHARK.GameManagement;
using CHARK.GameManagement.Systems;
using RIEVES.GGJ2026.Core.Settings;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RIEVES.GGJ2026.Core.Input
{
    public sealed class SimpleInputSystem : MonoSystem, IInputSystem
    {
        [Header("General")]
        [SerializeField]
        private PlayerInput playerInput;

        [Header("Control Schemes")]
        [SerializeField]
        private string keyboardMouseControlScheme = "KeyboardMouse";

        [SerializeField]
        private string gamepadControlScheme = "Gamepad";

        [Header("Action Maps")]
        [SerializeField]
        private string playerActionMapName = "Player";

        [SerializeField]
        private string uiActionMapName = "UI";

        private ISettingsSystem settingsSystem;

        public float LookSensitivity
        {
            get => GetLookSensitivity();
            set => SetLookSensitivity(value);
        }

        public ControlScheme ControlScheme => GetControlScheme(playerInput);

        public override void OnInitialized()
        {
            settingsSystem = GameManager.GetSystem<ISettingsSystem>();
            playerInput.onControlsChanged += OnControlsChanged;
        }

        public override void OnDisposed()
        {
            playerInput.onControlsChanged -= OnControlsChanged;
        }

        private void OnControlsChanged(PlayerInput input)
        {
            var controlScheme = GetControlScheme(input);
            var message = new ControlSchemeChangedMessage(controlScheme);

            GameManager.Publish(message);
        }

        public void EnablePlayerInput()
        {
            foreach (var inputAction in GetActions(playerActionMapName))
            {
                inputAction.Enable();
            }
        }

        public void DisablePlayerInput()
        {
            foreach (var inputAction in GetActions(playerActionMapName))
            {
                inputAction.Disable();
            }
        }

        public void EnableUIInput()
        {
            foreach (var inputAction in GetActions(uiActionMapName))
            {
                inputAction.Enable();
            }
        }

        public void DisableUIInput()
        {
            foreach (var inputAction in GetActions(uiActionMapName))
            {
                inputAction.Disable();
            }
        }

        private IEnumerable<InputAction> GetActions(string actionMapName)
        {
            if (playerInput == false)
            {
                yield break;
            }

            var inputActions = playerInput.actions;
            if (inputActions == false)
            {
                yield break;
            }

            foreach (var inputActionMap in inputActions.actionMaps)
            {
                if (string.Equals(actionMapName, inputActionMap.name) == false)
                {
                    continue;
                }

                foreach (var inputAction in inputActionMap.actions)
                {
                    yield return inputAction;
                }
            }
        }

        private float GetLookSensitivity()
        {
            var settings = settingsSystem.Settings;
            return GetNormalizedSensitivity(settings.LookSensitivity);
        }

        private void SetLookSensitivity(float newLookSensitivity)
        {
            var settings = settingsSystem.Settings;
            settings.LookSensitivity = GetNormalizedSensitivity(newLookSensitivity);

            settingsSystem.Settings = settings;
        }

        private static float GetNormalizedSensitivity(float volume)
        {
            var clampedSensitivity = Mathf.Clamp(
                volume,
                GeneralSettings.MinLookSensitivity,
                GeneralSettings.MaxLookSensitivity
            );

            return (float)Math.Round(clampedSensitivity, 2);
        }

        private ControlScheme GetControlScheme(PlayerInput input)
        {
            if (input == false)
            {
                // Defaults to keyboard & mouse.
                return ControlScheme.KeyboardMouse;
            }

            var schemeName = input.currentControlScheme;
            if (string.Equals(schemeName, keyboardMouseControlScheme))
            {
                return ControlScheme.KeyboardMouse;
            }

            if (string.Equals(schemeName, gamepadControlScheme))
            {
                return ControlScheme.Gamepad;
            }

            // Defaults to keyboard & mouse.
            return ControlScheme.KeyboardMouse;
        }
    }
}
