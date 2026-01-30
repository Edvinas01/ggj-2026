using CHARK.GameManagement.Systems;

namespace RIEVES.GGJ2026.Core.Cursors
{
    public interface ICursorSystem : ISystem
    {
        public bool IsCursorLocked { get; }

        /// <summary>
        /// Lock and hide game cursor.
        /// </summary>
        public void LockCursor();

        /// <summary>
        /// Unlock and show game cursor.
        /// </summary>
        public void UnLockCursor();
    }
}
