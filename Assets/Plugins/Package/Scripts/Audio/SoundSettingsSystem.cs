//using _Shared.Scripts.Constants;
using Cysharp.Threading.Tasks;
//using MapMagic.Expose;
//using MaxProFitness.SharedSound;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace maxprofitness.login
{
    public class SoundSettingsSystem : Singleton<SoundSettingsSystem>
    {
        #region VARIABLES


        public AudioSourcePropertiesDatabase audioSourcePropertiesDatabase;
        public AudioMixer _audioMixer;
        public GameObject soundPlayerPrefab;

        private bool _initialized;


        #endregion


        #region MONOBEHAVIOURS


        //-----------------------------//
        protected override void Awake()
        //-----------------------------//
        {
            Init();

        } // END Awake


        //-----------------//
        private void Start()
        //-----------------//
        {
            InitializeSoundSettings();

        } // END Start


        //--------------------------------//
        protected override void OnDestroy()
        //--------------------------------//
        {
            base.OnDestroy();

        } // END OnDestroy


        #endregion


        #region INIT


        //-----------------//
        private void Init()
        //-----------------//
        {
            base.Awake();

            //_audioMixer = Resources.Load<AudioMixer>("CompleteMainMix");
            //soundPlayerPrefab = Resources.Load<GameObject>("SharedSoundPlayer");
            //audioSourcePropertiesDatabase = Resources.Load<AudioSourcePropertiesDatabase>("AudioSourcePropertiesDatabase");

            SoundManager.SetSoundPlayerPrefab(soundPlayerPrefab);
            SoundManager.Initialize(audioSourcePropertiesDatabase);

        } // END Init


        #endregion


        #region SET SOUND SETTING


        //-------------------------------------------------//
        public void SetSoundSetting(string key, float value)
        //-------------------------------------------------//
        {
            _audioMixer.SetFloat(key, Mathf.Log10(value) * 20f);

        } // END SetSoundSetting


        #endregion


        #region SAVE SOUND PREFERENCES


        //-------------------------------------------------------//
        public void SaveSoundPreferences(string key, float value)
        //-------------------------------------------------------//
        {
            PlayerPrefs.SetFloat(key, value);

        } // END SaveSoundPreference


        #endregion


        #region INITIALIZE SOUND SETTINGS


        //------------------------------------//
        private void InitializeSoundSettings()
        //------------------------------------//
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            if (PlayerPrefs.HasKey(SettingsKeys.MusicVolume))
            {
                float savedMusicVolume = PlayerPrefs.GetFloat(SettingsKeys.MusicVolume);

                SetSoundSetting(AudioMixerParameters.Music, savedMusicVolume);
            }

            if (PlayerPrefs.HasKey(SettingsKeys.SfxVolume))
            {
                float savedSfxVolume = PlayerPrefs.GetFloat(SettingsKeys.SfxVolume);

                SetSoundSetting(AudioMixerParameters.Sfx, savedSfxVolume);
            }

            if (PlayerPrefs.HasKey(SettingsKeys.AmbienceVolume))
            {
                float savedAmbienceVolume = PlayerPrefs.GetFloat(SettingsKeys.AmbienceVolume);

                SetSoundSetting(AudioMixerParameters.Ambience, savedAmbienceVolume);
            }

            if (PlayerPrefs.HasKey(SettingsKeys.VoiceOverVolume))
            {
                float savedVoiceOverVolume = PlayerPrefs.GetFloat(SettingsKeys.VoiceOverVolume);

                SetSoundSetting(AudioMixerParameters.VoiceOver, savedVoiceOverVolume);
            }

            FinishSoundSettingsInitialization();

        } // END InitializeSoundSettings


        #endregion
        
        
        #region FINISH SOUND SETTINGS INITIALIZATION


        //----------------------------------------------------//
        private async void FinishSoundSettingsInitialization()
        //----------------------------------------------------//
        {
            TimeSpan duration = TimeSpan.FromSeconds(1);
            await UniTask.Delay(duration);

            Debug.Log($"[{nameof(SoundSettingsSystem)}] - Sound settings loaded");
            //new SoundsSettingsLoadedEvent().TryInvokeShared(this);

        } // END FinishSoundSettingsInitialization


        #endregion


    } // END CLASS

} // END NAMESPACE
