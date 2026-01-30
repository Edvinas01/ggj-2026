using CHARK.GameManagement.Systems;

namespace RIEVES.GGJ2026.Core.Settings
{
    public interface ISettingsSystem : ISystem
    {
        /// <summary>
        /// Current settings.
        /// </summary>
        public SettingsData Settings { get; set; }
    }
}
