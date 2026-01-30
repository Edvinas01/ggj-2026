using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Movement
{
    internal abstract class InputProvider : MonoBehaviour
    {
        public abstract Vector2 MoveAxis { get; }
    }
}
