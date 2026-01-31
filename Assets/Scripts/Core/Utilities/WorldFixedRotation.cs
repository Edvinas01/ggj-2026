using UnityEngine;

namespace RIEVES.GGJ2026.Core.Utilities
{
    public sealed class WorldFixedRotation : MonoBehaviour
    {
        [SerializeField]
        private Vector3 forwardAxis =  Vector3.forward;

        private void LateUpdate()
        {
            transform.forward = forwardAxis;
        }
    }
}
