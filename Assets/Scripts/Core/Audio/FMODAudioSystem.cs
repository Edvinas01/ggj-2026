using System;
using CHARK.GameManagement;
using CHARK.GameManagement.Systems;
using FMODUnity;
using RIEVES.GGJ2026.Core.Settings;
using UnityEngine;

namespace RIEVES.GGJ2026.Core.Audio
{
    public sealed class FMODAudioSystem : MonoSystem, IAudioSystem
    {
        [Header("Banks")]
        [SerializeField]
        private StudioBankLoader bankLoader;

        [Header("Global Parameters")]
        [SerializeField]
        [ParamRef]
        private string globalMasterVolumeParameter;

        [SerializeField]
        [ParamRef]
        private string globalMusicVolumeParameter;

        [SerializeField]
        [ParamRef]
        private string globalSfxVolumeParameter;

        private ISettingsSystem settingsSystem;

        public bool IsLoading
        {
            get
            {
                if (RuntimeManager.HaveAllBanksLoaded == false)
                {
                    return true;
                }

                foreach (var bank in bankLoader.Banks)
                {
                    if (RuntimeManager.HasBankLoaded(bank) == false)
                    {
                        return true;
                    }
                }

                if (RuntimeManager.AnySampleDataLoading())
                {
                    return true;
                }

                return false;
            }
        }

        public void LoadBanks()
        {
            bankLoader.Load();
        }

        public void UnLoadBanks()
        {
            bankLoader.Unload();
        }

        public override void OnInitialized()
        {
            base.OnInitialized();
            settingsSystem = GameManager.GetSystem<ISettingsSystem>();
        }

        private void Start()
        {
            InitializeGlobalVolumeParameters();
        }

        public float GetVolume(VolumeType type)
        {
            var settings = settingsSystem.Settings;
            var volume = type switch
            {
                VolumeType.Master => settings.MasterVolume,
                VolumeType.Music => settings.MusicVolume,
                VolumeType.SFX => settings.SfxVolume,
                _ => GeneralSettings.MaxVolume,
            };

            return GetNormalizedVolume(volume);
        }

        public void SetVolume(VolumeType type, float volume)
        {
            var clampedVolume = GetNormalizedVolume(volume);
            var settings = settingsSystem.Settings;

            switch (type)
            {
                case VolumeType.Master:
                    RuntimeManager.StudioSystem.setParameterByName(globalMasterVolumeParameter, clampedVolume);
                    settings.MasterVolume = clampedVolume;
                    break;
                case VolumeType.Music:
                    RuntimeManager.StudioSystem.setParameterByName(globalMusicVolumeParameter, clampedVolume);
                    settings.MusicVolume = clampedVolume;
                    break;
                case VolumeType.SFX:
                    RuntimeManager.StudioSystem.setParameterByName(globalSfxVolumeParameter, clampedVolume);
                    settings.SfxVolume = clampedVolume;
                    break;
                default:
                    Debug.LogWarning($"Unsupported volume type: {type}", this);
                    break;
            }

            settingsSystem.Settings = settings;
        }

        private void InitializeGlobalVolumeParameters()
        {
            RuntimeManager.StudioSystem.setParameterByName(globalMasterVolumeParameter, GetVolume(VolumeType.Master));
            RuntimeManager.StudioSystem.setParameterByName(globalMusicVolumeParameter, GetVolume(VolumeType.Music));
            RuntimeManager.StudioSystem.setParameterByName(globalSfxVolumeParameter, GetVolume(VolumeType.SFX));
        }

        private static float GetNormalizedVolume(float volume)
        {
            var clampedVolume = Mathf.Clamp(
                volume,
                GeneralSettings.MinVolume,
                GeneralSettings.MaxVolume
            );

            var roundVolume = (float)Math.Round(clampedVolume, 2);

            return roundVolume;
        }
    }
}
