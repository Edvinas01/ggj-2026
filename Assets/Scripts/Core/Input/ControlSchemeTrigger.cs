using System.Collections.Generic;
using CHARK.GameManagement;
using UnityEngine;
using UnityEngine.Events;

namespace RIEVES.GGJ2026.Core.Input
{
    public sealed class ControlSchemeTrigger : MonoBehaviour
    {
        [Header("Game Objects")]
        [SerializeField]
        private List<GameObject> keyboardMouseObjects;

        [SerializeField]
        private List<GameObject> gamepadObjects;

        [SerializeField]
        private List<GameObject> touchObjects;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onKeyboardMouse;

        [SerializeField]
        private UnityEvent onGamepad;

        [SerializeField]
        private UnityEvent onTouch;

        private IInputSystem inputSystem;

        private void Awake()
        {
            inputSystem = GameManager.GetSystem<IInputSystem>();
        }

        private void OnEnable()
        {
            SetControlScheme(inputSystem.ControlScheme);
            GameManager.AddListener<ControlSchemeChangedMessage>(OnControlSchemeChanged);
        }

        private void OnDisable()
        {
            GameManager.RemoveListener<ControlSchemeChangedMessage>(OnControlSchemeChanged);
        }

        private void OnControlSchemeChanged(ControlSchemeChangedMessage message)
        {
            SetControlScheme(message.ControlScheme);
        }

        private void SetControlScheme(ControlScheme controlScheme)
        {
            switch (controlScheme)
            {
                case ControlScheme.KeyboardMouse:
                {
                    foreach (var obj in keyboardMouseObjects)
                    {
                        obj.SetActive(true);
                    }

                    foreach (var obj in gamepadObjects)
                    {
                        obj.SetActive(false);
                    }

                    foreach (var obj in touchObjects)
                    {
                        obj.SetActive(false);
                    }

                    onKeyboardMouse.Invoke();
                    break;
                }
                case ControlScheme.Gamepad:
                {
                    foreach (var obj in keyboardMouseObjects)
                    {
                        obj.SetActive(false);
                    }

                    foreach (var obj in gamepadObjects)
                    {
                        obj.SetActive(true);
                    }

                    foreach (var obj in touchObjects)
                    {
                        obj.SetActive(false);
                    }

                    onGamepad.Invoke();
                    break;
                }
                case ControlScheme.Touch or ControlScheme.Joystick:
                {
                    foreach (var obj in keyboardMouseObjects)
                    {
                        obj.SetActive(false);
                    }

                    foreach (var obj in gamepadObjects)
                    {
                        obj.SetActive(false);
                    }

                    foreach (var obj in touchObjects)
                    {
                        obj.SetActive(true);
                    }

                    onTouch.Invoke();
                    break;
                }
            }
        }
    }
}
