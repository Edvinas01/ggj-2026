using UnityEngine;
using UnityEngine.Events;

namespace RIEVES.GGJ2026.Runtime.Doors
{
    internal sealed class DoorActor : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField]
        private UnityEvent onOpened;

        [SerializeField]
        private UnityEvent onClosed;

        public bool IsOpen { get; private set; }

        public void Open()
        {
            if (IsOpen)
            {
                return;
            }

            onOpened.Invoke();
            IsOpen = true;
        }

        public void Close()
        {
            if (IsOpen == false)
            {
                return;
            }

            onClosed.Invoke();
            IsOpen = false;
        }

        public void ToggleOpen()
        {
            IsOpen = !IsOpen;

            if (IsOpen)
            {
                onOpened.Invoke();
            }
            else
            {
                onClosed.Invoke();
            }
        }
    }
}
