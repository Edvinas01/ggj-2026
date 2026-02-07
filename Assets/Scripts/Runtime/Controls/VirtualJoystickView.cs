using RIEVES.GGJ2026.Core.Views;
using Terresquall;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Controls
{
    internal sealed class VirtualJoystickView : View
    {
        [SerializeField]
        private VirtualJoystick virtualJoystick;

        public Vector2 JoystickAxis => virtualJoystick ? virtualJoystick.GetAxis() : Vector2.zero;

        public bool IsJoystickEnabled
        {
            get => virtualJoystick && virtualJoystick.gameObject.activeInHierarchy;
            set
            {
                if (virtualJoystick)
                {
                    virtualJoystick.gameObject.SetActive(value);
                }
            }
        }
    }
}
