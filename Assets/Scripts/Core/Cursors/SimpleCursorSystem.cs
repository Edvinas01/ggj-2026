using CHARK.GameManagement.Systems;
using UnityEngine;

namespace RIEVES.GGJ2026.Core.Cursors
{
    public sealed class SimpleCursorSystem : MonoSystem, ICursorSystem
    {
        // ReSharper disable once UnusedMember.Local
        [Header("Cursor (Web)")]
        [SerializeField]
        private Texture2D webCursorTexture;

        // ReSharper disable once UnusedMember.Local
        [SerializeField]
        private Vector2 cursorHotspotWeb = new(12f, 1f);

        // ReSharper disable once UnusedMember.Local
        [Header("Cursor (PC)")]
        [SerializeField]
        private Texture2D cursorTexture;

        // ReSharper disable once UnusedMember.Local
        [SerializeField]
        private Vector2 cursorHotspot = new(47f, 6f);

        public bool IsCursorLocked => Cursor.lockState != CursorLockMode.None;

        public override void OnInitialized()
        {
            base.OnInitialized();

#if UNITY_WEBGL
            Cursor.SetCursor(
                webCursorTexture,
                cursorHotspotWeb,
                CursorMode.ForceSoftware
            );
#else
            Cursor.SetCursor(
                cursorTexture,
                cursorHotspot,
                CursorMode.ForceSoftware
            );
#endif
        }

        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void UnLockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
