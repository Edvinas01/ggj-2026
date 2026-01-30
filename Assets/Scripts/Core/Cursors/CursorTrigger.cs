using CHARK.GameManagement;
using UnityEngine;

namespace RIEVES.GGJ2026.Core.Cursors
{
    internal sealed class CursorTrigger : MonoBehaviour
    {
        [SerializeField]
        private bool isLockOnStart;

        private ICursorSystem cursorSystem;

        private void Awake()
        {
            cursorSystem = GameManager.GetSystem<ICursorSystem>();
        }

        private void Start()
        {
            if (isLockOnStart)
            {
                cursorSystem.LockCursor();
            }
        }
    }
}
