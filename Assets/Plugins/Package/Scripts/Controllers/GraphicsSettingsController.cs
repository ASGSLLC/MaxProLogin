//using _Shared.Scripts.Constants;
//using MaxProFitness.SharedSound;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using maxprofitness.login;

namespace App.Scripts.UI.Settings
{
    public class GraphicsSettingsController : MonoBehaviour
    {
        #region VARIABLES


        [SerializeField] private Button _saveButton;
        [SerializeField] private Button[] _qualityOptions;
        [SerializeField] private TMP_Text[] _qualityOptionsTexts;

        [Header("Normal Button Data")]
        [SerializeField] private Color _normalButtonTextColor;
        [SerializeField] private Sprite _normalButtonSprite;

        [Header("Selected Button Data")]
        [SerializeField] private Color _selectedButtonTextColor;
        [SerializeField] private Sprite _selectedButtonSprite;

        private int _selectedQualityIndex;
        
        
        #endregion


        #region MONOBEHAVIORS


        //-----------------//
        private void Awake()
        //-----------------//
        {
            ListenToEvents();

        } // END Awake


        //--------------------//
        private void OnEnable()
        //--------------------//
        {
            Init();

        } // END OnEnable


        //---------------------//
        private void OnDestroy()
        //---------------------//
        {
            RemoveEventListeners();

        } // END OnDestroy


        #endregion
        
        
        #region INIT


        //----------------------//
        private void Init()
        //----------------------//
        {
            SetQualityOptionButtonValue(PlayerPrefs.GetInt(SettingsKeys.QualitySettings));
            _selectedQualityIndex = PlayerPrefs.GetInt(SettingsKeys.QualitySettings);

        } // END Init


        #endregion


        #region ADD/REMOVE LISTENERS


        //---------------------------//
        private void ListenToEvents()
        //---------------------------//
        {
            for (int i = 0; i < _qualityOptions.Length; i++)
            {
                int qualityIndex = i;
                _qualityOptions[i].onClick.AddListener(() => HandleQualityOptionPressed(qualityIndex));
            }

            _saveButton.onClick.AddListener(HandleSaveButtonPressed);

        } // END ListenToEvents


        //---------------------------------//
        private void RemoveEventListeners()
        //---------------------------------//
        {
            foreach (Button qualityOption in _qualityOptions)
            {
                qualityOption.onClick.RemoveAllListeners();
            }

            _saveButton.onClick.RemoveAllListeners();

        } // END RemoveEventListeners


        #endregion


        #region HANDLE BUTTON PRESSES


        //------------------------------------//
        private void HandleSaveButtonPressed()
        //------------------------------------//
        {
            SoundManager.PlaySound(SharedGameSound.APP_UI_BUTTON_CLICK);

            QualitySettings.SetQualityLevel(_selectedQualityIndex);
            PlayerPrefs.SetInt(SettingsKeys.QualitySettings, _selectedQualityIndex);

            _saveButton.interactable = false;

        } // END HandleSaveButtonPressed


        //-------------------------------------------------------//
        private void HandleQualityOptionPressed(int qualityIndex)
        //-------------------------------------------------------//
        {
            SoundManager.PlaySound(SharedGameSound.APP_UI_BUTTON_CLICK);

            SetQualityOptionButtonValue(qualityIndex);

        } // END HandleQualityOptionPressed


        #endregion


        #region SET QUALITY OPTION BUTTON VALUE


        //-------------------------------------------------//
        private void SetQualityOptionButtonValue(int value)
        //-------------------------------------------------//
        {
            Button qualityOptionPressed = _qualityOptions[value];
            _selectedQualityIndex = value;

            for (int i = 0; i < _qualityOptions.Length; i++)
            {
                if (qualityOptionPressed == _qualityOptions[i])
                {
                    _qualityOptions[i].image.sprite = _selectedButtonSprite;
                    _qualityOptionsTexts[i].color = _selectedButtonTextColor;
                    QualitySettings.SetQualityLevel(i, true);
                }
                else
                {
                    _qualityOptions[i].image.sprite = _normalButtonSprite;
                    _qualityOptionsTexts[i].color = _normalButtonTextColor;
                }
            }

            _saveButton.interactable = value != PlayerPrefs.GetInt(SettingsKeys.QualitySettings);

        } // END SetQualityOptionButtonValue


        #endregion


    } // END CLASS

} // END NAMESPACE
