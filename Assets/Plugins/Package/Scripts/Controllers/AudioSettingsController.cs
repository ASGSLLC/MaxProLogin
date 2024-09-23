using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using maxprofitness.login;
using maxprofitness.shared;

namespace App.Scripts.UI.Settings
{
    public class AudioSettingsController : MonoBehaviour
    {
        #region VARIABLES


        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private Slider _ambienceSlider;
        [SerializeField] private Slider _voiceOverSlider;
        [SerializeField] private Button _saveButton;

        private float _lastSavedMusicVolume;
        private float _lastSavedSfxVolume;
        private float _lastSavedAmbienceVolume;
        private float _lastSavedVoiceOverVolume;

        private bool _hasUnsavedChanges;


        #endregion
        
        
        #region MONOBEHAVIOURS


        //-------------------//
        private void Awake()
        //------------------//
        {
            ListenToEvents();

        } // END Awake


        //---------------------//
        private void OnEnable()
        //--------------------//
        {
            Initialize();

        } // END OnEnable


        //----------------------//
        private void OnDisable()
        //---------------------//
        {
            if (!_hasUnsavedChanges)
            {
                return;
            }

            _musicSlider.value = _lastSavedMusicVolume;
            _sfxSlider.value = _lastSavedSfxVolume;
            _ambienceSlider.value = _lastSavedAmbienceVolume;
            _voiceOverSlider.value = _lastSavedVoiceOverVolume;

        } // END OnDisable


        //---------------------//
        private void OnDestroy()
        //--------------------//
        {
            RemoveEventListeners();

        } // END OnDestroy


        #endregion


        #region INIT


        //-----------------------//
        private void Initialize()
        //-----------------------//
        {
            if (PlayerPrefs.HasKey(SettingsKeys.MusicVolume))
            {
                float _savedMusicVolume = PlayerPrefs.GetFloat(SettingsKeys.MusicVolume);

                _musicSlider.value = _savedMusicVolume;
            }

            if (PlayerPrefs.HasKey(SettingsKeys.SfxVolume))
            {
                float _savedSfxVolume = PlayerPrefs.GetFloat(SettingsKeys.SfxVolume);

                _sfxSlider.value = _savedSfxVolume;
            }

            if (PlayerPrefs.HasKey(SettingsKeys.AmbienceVolume))
            {
                float _savedAmbienceVolume = PlayerPrefs.GetFloat(SettingsKeys.AmbienceVolume);

                _ambienceSlider.value = _savedAmbienceVolume;
            }

            if (PlayerPrefs.HasKey(SettingsKeys.VoiceOverVolume))
            {
                float _savedVoiceOverVolume = PlayerPrefs.GetFloat(SettingsKeys.VoiceOverVolume);

                _voiceOverSlider.value = _savedVoiceOverVolume;
            }

            _lastSavedMusicVolume = _musicSlider.value;
            _lastSavedSfxVolume = _sfxSlider.value;
            _lastSavedAmbienceVolume = _ambienceSlider.value;
            _lastSavedVoiceOverVolume = _voiceOverSlider.value;

            _saveButton.interactable = false;

        } // END Init


        #endregion


        #region ADD/REMOVE LISTENER


        //---------------------------//
        private void ListenToEvents()
        //---------------------------//
        {
            _musicSlider.onValueChanged.AddListener(SetMusicVolume);
            _sfxSlider.onValueChanged.AddListener(SetSfxVolume);
            _ambienceSlider.onValueChanged.AddListener(SetAmbienceVolume);
            _voiceOverSlider.onValueChanged.AddListener(SetVoiceOverVolume);

            _saveButton.onClick.AddListener(HandleSaveButtonPressed);

        } // END ListenToEvents


        //---------------------------------//
        private void RemoveEventListeners()
        //--------------------------------//
        {
            _musicSlider.onValueChanged.RemoveAllListeners();
            _sfxSlider.onValueChanged.RemoveAllListeners();
            _ambienceSlider.onValueChanged.RemoveAllListeners();
            _voiceOverSlider.onValueChanged.RemoveAllListeners();

            _saveButton.onClick.RemoveAllListeners();

        } // END RemoveEventListeners


        #endregion


        #region HANDLE SAVE BUTTON PRESSED


        //------------------------------------//
        private void HandleSaveButtonPressed()
        //------------------------------------//
        {
            SoundManager.PlaySound(SharedGameSound.APP_UI_BUTTON_CLICK);

            float _newMusicVolume = _musicSlider.value;
            float _newSfxVolume = _sfxSlider.value;
            float _newAmbienceVolume = _ambienceSlider.value;
            float _newVoiceOverVolume = _voiceOverSlider.value;

            _lastSavedMusicVolume = _newMusicVolume;
            _lastSavedSfxVolume = _newSfxVolume;
            _lastSavedAmbienceVolume = _newAmbienceVolume;
            _lastSavedVoiceOverVolume = _newVoiceOverVolume;

            SoundSettingsSystem.Instance.SaveSoundPreferences(SettingsKeys.MusicVolume, _newMusicVolume);
            SoundSettingsSystem.Instance.SaveSoundPreferences(SettingsKeys.SfxVolume, _newSfxVolume);
            SoundSettingsSystem.Instance.SaveSoundPreferences(SettingsKeys.AmbienceVolume, _newAmbienceVolume);
            SoundSettingsSystem.Instance.SaveSoundPreferences(SettingsKeys.VoiceOverVolume, _newVoiceOverVolume);

            _saveButton.interactable = false;
            _hasUnsavedChanges = false;

        } // END HandleSaveButtonPressed


        #endregion


        #region SET VOLUME
        
        
        //--------------------------------------------//
        private void SetMusicVolume(float sliderValue)
        //--------------------------------------------//
        {
            const string key = "Music";

            SoundSettingsSystem.Instance.SetSoundSetting(key, sliderValue);

            _saveButton.interactable = true;
            _hasUnsavedChanges = true;

        } // END SetMusicVolume


        //------------------------------------------//
        private void SetSfxVolume(float sliderValue)
        //------------------------------------------//
        {
            const string key = AudioMixerParameters.Sfx;

            SoundSettingsSystem.Instance.SetSoundSetting(key, sliderValue);

            _saveButton.interactable = true;
            _hasUnsavedChanges = true;

        } // END SetSfxVolume


        //-----------------------------------------------//
        private void SetAmbienceVolume(float sliderValue)
        //----------------------------------------------//
        {
            const string key = AudioMixerParameters.Ambience;

            SoundSettingsSystem.Instance.SetSoundSetting(key, sliderValue);

            _saveButton.interactable = true;
            _hasUnsavedChanges = true;

        } // END SetAmbienceVolume


        //-------------------------------------------------//
        private void SetVoiceOverVolume(float sliderValue)
        //-------------------------------------------------//
        {
            const string key = AudioMixerParameters.VoiceOver;

            SoundSettingsSystem.Instance.SetSoundSetting(key, sliderValue);

            _saveButton.interactable = true;
            _hasUnsavedChanges = true;

        } // END SetVoiceOverVolume


        #endregion


    } // END Class


} // END Namespace
