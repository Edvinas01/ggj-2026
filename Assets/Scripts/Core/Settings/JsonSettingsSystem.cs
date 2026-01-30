using CHARK.GameManagement;
using CHARK.GameManagement.Systems;
using UnityEngine;

namespace RIEVES.GGJ2026.Core.Settings
{
    public sealed class JsonSettingsSystem : MonoSystem, ISettingsSystem
    {
        private const string SettingsPath = "Settings.json";

        [SerializeField]
        private GeneralSettings generalSettings;

        public SettingsData Settings { get; set; }

        public override void OnInitialized()
        {
            ReadSettings();
        }

        public override void OnDisposed()
        {
            // TODO: probably not the best idea to save on exit as this doesn't handle crashes
            WriteSettings();
        }

        private void DeleteSettings()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                Debug.LogWarning("Can only delete during playmode", this);
                return;
            }
#endif

            GameManager.DeleteData(SettingsPath);
            Settings = CreateDefaultSettings();
        }

        private void ReadSettings()
        {
            if (GameManager.TryReadData<SettingsData>(SettingsPath, out var data))
            {
                Settings = data;
            }
            else
            {
                Settings = CreateDefaultSettings();
            }
        }

        private void WriteSettings()
        {
            GameManager.SaveData(SettingsPath, Settings);
        }

        private SettingsData CreateDefaultSettings()
        {
            return new SettingsData(
                lookSensitivity: generalSettings.DefaultLookSensitivity,
                masterVolume: generalSettings.DefaultMasterVolume,
                musicVolume: generalSettings.DefaultMusicVolume,
                sfxVolume: generalSettings.DefaultSfxVolume
            );
        }
    }
}
