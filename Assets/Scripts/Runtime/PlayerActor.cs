using CHARK.GameManagement;
using RIEVES.GGJ2026.Core.Cursors;
using RIEVES.GGJ2026.Runtime.Movement;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime
{
    internal sealed class PlayerActor : MonoBehaviour
    {
        [SerializeField]
        private MovementController movementController;

        private ICursorSystem cursorSystem;

        public MovementController MovementController => movementController;

        private void Awake()
        {
            cursorSystem = GameManager.GetSystem<ICursorSystem>();
        }

        private void Start()
        {
            cursorSystem.LockCursor();
        }

        private void OnDestroy()
        {
            cursorSystem.UnLockCursor();
        }
    }
}
