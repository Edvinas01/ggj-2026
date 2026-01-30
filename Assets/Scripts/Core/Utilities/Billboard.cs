using UnityEngine;

namespace RIEVES.GGJ2026.Core.Utilities
{
    internal sealed class Billboard : MonoBehaviour
    {
        [SerializeField]
        private bool isFlipDirection;

        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (mainCamera == false)
            {
                return;
            }

            var direction = mainCamera.transform.position - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(isFlipDirection ? -direction : direction);
        }
    }
}
