using CHARK.GameManagement.Systems;

namespace RIEVES.GGJ2026.Core.Input
{
    public interface IInputSystem : ISystem
    {
        /// <summary>
        /// Look sensitivity the user has set.
        /// </summary>
        public float LookSensitivity { get; set; }

        /// <summary>
        /// Current control scheme.
        /// </summary>
        public ControlScheme ControlScheme { get;  }

        /// <summary>
        /// Enable player input actions.
        /// </summary>
        public void EnablePlayerInput();

        /// <summary>
        /// Disable player input actions (e.g., during pause menu).
        /// </summary>
        public void DisablePlayerInput();

        /// <summary>
        /// Enable UI input actions.
        /// </summary>
        public void EnableUIInput();

        /// <summary>
        /// Disable UI input actions.
        /// </summary>
        public void DisableUIInput();
    }
}
