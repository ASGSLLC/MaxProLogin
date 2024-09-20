//using _Project.App.Scripts.Minigames;
//using App.Scripts.System.MinigameSelection;
//using App.Scripts.UI.DifficultySelection.ScriptableObjects;
using DG.Tweening;
//using MaxProFitness.SharedSound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using _Project.App.Scripts.Highscore;
//using _Project.RowingCanoe.Scripts;
//using FitFighter.RhythmRevamp.Scripts;

#if MAXPRO_LOGIN
using maxprofitness.login;
#endif

#if ROWING
using maxprofitness.rowing;
#endif

namespace maxprofitness.shared
{
    /// <summary>
    /// This class is responsible to set the information on the screen and manage the animations of the difficulty selection bar
    /// </summary>
    public class DifficultySelectionView : MonoBehaviour
    {
        public CanvasGroup canvasGroup;

        [SerializeField] private MinigameUnlockingSettings unlockSettings;
        [Header("Screen Data")]
        [SerializeField] private DifficultySelectionScreenDataSO _minigameDataReceived;

        [Header("Locked Handles Pop Up")]
        [SerializeField] private CanvasGroup _popupCanvasGroup;
        [SerializeField] private CanvasGroup _notPairedCanvasGroup;
        [SerializeField] private Transform _notPairedHolder;
        [SerializeField] private Transform _popUpHolder;
        [SerializeField] private Button _popUpContinueButton;

        [Header("Descriptions Texts")]
        [SerializeField] private TextMeshProUGUI _firstLineInformationText;
        [SerializeField] private TextMeshProUGUI _firstLineInformationValue;
        [SerializeField] private TextMeshProUGUI _secondLineInformationText;
        [SerializeField] private TextMeshProUGUI _secondLineInformationValue;
        [SerializeField] private TextMeshProUGUI _knobLevelSuggestionText;
        [SerializeField] private TextMeshProUGUI _difficultyDescriptionText;

        [Header("Difficulty Text")]
        [SerializeField] private TextMeshProUGUI _difficultyText;

        [Header("Difficulties Buttons")]
        [SerializeField] private Button _firstDifficultyButton;
        [SerializeField] private Button _secondDifficultyButton;
        [SerializeField] private Button _thirdDifficultyButton;
        [SerializeField] private Button _fourthDifficultyButton;
        [SerializeField] private Button _fifthDifficultyButton;

        [Header("Selection Arrow")]
        [SerializeField] private GameObject _selectionArrow;

        [Header("Play Button")]
        [SerializeField] private Button _playButton;

        private int _currentLeftKnobLevel;
        private int _currentRightKnobLevel;

        private string _firstDifficultyText;
        private string _secondDifficultyText;
        private string _thirdDifficultyText;
        private string _fourthDifficultyText;
        private string _fifthDifficultyText;

        [SerializeField] private Button _currentButtonSelected;
        private DifficultyDataSO _currentDifficultyData;
#if FIT_FIGHTER
        private FitFighterManager fitFighterManager;
        private RhythmGamemodeSystem rhythmGamemodeSystem;
#endif
        private const float TextFadeDuration = 1f;
        private const float PopUpFadeDuration = .15f;
        private const float PopUpFadeScaleIn = 1.9f;

        public MinigameDifficulty CurrentMinigameDifficulty { get; private set; } = MinigameDifficulty.None;

        private ButtonAnchorController SecondButtonController => _secondDifficultyButton.GetComponent<ButtonAnchorController>();
        private ButtonAnchorController ThirdButtonController => _thirdDifficultyButton.GetComponent<ButtonAnchorController>();
        private ButtonAnchorController FourthButtonController => _fourthDifficultyButton.GetComponent<ButtonAnchorController>();
        private ButtonAnchorController FifthButtonController => _fifthDifficultyButton.GetComponent<ButtonAnchorController>();
        private SelectionArrowPositionController ArrowPositionController => _selectionArrow.GetComponent<SelectionArrowPositionController>();
#if ROWING
        [SerializeField]private RowingTrackManager rowingTrackManager;
#endif


        private void Awake()
        {
            _firstDifficultyButton.onClick.AddListener(HandleFirstButtonClick);
            _secondDifficultyButton.onClick.AddListener(HandleSecondButtonClick);
            _thirdDifficultyButton.onClick.AddListener(HandleThirdButtonClick);
            _fourthDifficultyButton.onClick.AddListener(HandleFourthButtonClick);
            _fifthDifficultyButton.onClick.AddListener(HandleFifthButtonClick);

            _playButton.onClick.AddListener(HandlePlayButtonClick);
            _popUpContinueButton.onClick.AddListener(HandleLockedHandlePopUpContinueClick);
#if FIT_FIGHTER
            rhythmGamemodeSystem = FindObjectOfType<RhythmGamemodeSystem>();
            fitFighterManager = FindObjectOfType<FitFighterManager>();
#endif
            canvasGroup = GetComponent<CanvasGroup>();

        }

        private void Start()
        {
            InitializeView();
            _currentButtonSelected.onClick?.Invoke();

        }

        private void InitializeView()
        {
            _firstLineInformationText.text = _minigameDataReceived.FirstLineInformationText;
            _firstLineInformationValue.text = _minigameDataReceived.FirstDifficultyData.FirstLineParameter;
            _secondLineInformationText.text = _minigameDataReceived.SecondLineInformationText;
            _secondLineInformationValue.text = _minigameDataReceived.FirstDifficultyData.SecondLineParameter;
            _knobLevelSuggestionText.text = _minigameDataReceived.FirstDifficultyData.KnobLevelSuggestion;
            _difficultyDescriptionText.text = _minigameDataReceived.DifficultyDescription;

            _firstDifficultyText = _minigameDataReceived.FirstDifficultyData.DifficultyName;
            _secondDifficultyText = _minigameDataReceived.SecondDifficultyData.DifficultyName;
            _thirdDifficultyText = _minigameDataReceived.ThirdDifficultyData.DifficultyName;
            _fourthDifficultyText = _minigameDataReceived.FourthDifficultyData.DifficultyName;
            _fifthDifficultyText = _minigameDataReceived.FifthDifficultyData.DifficultyName;

            if (_currentDifficultyData == null)
            {
                _currentDifficultyData = _minigameDataReceived.FirstDifficultyData;
            }

            if (_currentButtonSelected == null)
            {
                _currentButtonSelected = _firstDifficultyButton;
            }

            if (CurrentMinigameDifficulty == MinigameDifficulty.None)
            {
                CurrentMinigameDifficulty = MinigameDifficulty.Beginner;
            }
        }

        //-------------------------------------//
        private void HandleLockDifficultyEvent()
        //------------------------------------//
        {
            bool isDifficultyLocked = false;

            switch ((int)CurrentMinigameDifficulty)
            {
                case 5:
                    ArrowPositionController._scoreWithBackgroundValueText.text = _minigameDataReceived.FifthDifficultyData.requiredScore.ToString();
                    ArrowPositionController._scoreWithoutBackgroundValueText.text = _minigameDataReceived.FifthDifficultyData.requiredScore.ToString();
                    CurrentMinigameDifficulty = MinigameDifficulty.Pro;

                    break;

                case 4:
                    ArrowPositionController._scoreWithBackgroundValueText.text = _minigameDataReceived.FourthDifficultyData.requiredScore.ToString();
                    ArrowPositionController._scoreWithoutBackgroundValueText.text = _minigameDataReceived.FourthDifficultyData.requiredScore.ToString();
                    CurrentMinigameDifficulty = MinigameDifficulty.Expert;

                    break;

                case 3:
                    ArrowPositionController._scoreWithBackgroundValueText.text = _minigameDataReceived.ThirdDifficultyData.requiredScore.ToString();
                    ArrowPositionController._scoreWithoutBackgroundValueText.text = _minigameDataReceived.ThirdDifficultyData.requiredScore.ToString();
                    CurrentMinigameDifficulty = MinigameDifficulty.Veteran;

                    break;

                case 2:
                    ArrowPositionController._scoreWithBackgroundValueText.text = _minigameDataReceived.SecondDifficultyData.requiredScore.ToString();
                    ArrowPositionController._scoreWithoutBackgroundValueText.text = _minigameDataReceived.SecondDifficultyData.requiredScore.ToString();
                    CurrentMinigameDifficulty = MinigameDifficulty.Regular;

                    break;

                case 1:
                    ArrowPositionController._scoreWithBackgroundValueText.text = _minigameDataReceived.FirstDifficultyData.requiredScore.ToString();
                    ArrowPositionController._scoreWithoutBackgroundValueText.text = _minigameDataReceived.FirstDifficultyData.requiredScore.ToString();
                    CurrentMinigameDifficulty = MinigameDifficulty.Beginner;

                    break;
            }

            Debug.Log("DifficultySelectionView.cs // HandleLockDifficultyEvent");

            if (isDifficultyLocked)
            {
                //ArrowPositionController.ActivateLockedScoreView(scoreText);
            }
            else
            {
                //ArrowPositionController.DeactivateLockedScoreView(scoreText);
            }

            _playButton.interactable = !isDifficultyLocked;

        }

        #region HANDLE FIRST BUTTON CLICK

        //-----------------------------------//
        private void HandleFirstButtonClick()
        //-----------------------------------//
        {
            //SoundManager.PlaySound(SharedGameSound.APP_UI_BUTTON_CLICK);

            //new DifficultySelectionChangeEvent(MinigameDifficulty.Beginner).TryInvokeShared(this);

            CurrentMinigameDifficulty = MinigameDifficulty.Beginner;
            _currentDifficultyData = _minigameDataReceived.FirstDifficultyData;
            UpdateDifficultyInformation();

            HandleLockDifficultyEvent();

            SecondButtonController.ContractButtonMinSize();
            ThirdButtonController.ContractButtonMinSize();
            FourthButtonController.ContractButtonMinSize();
            FifthButtonController.ContractButtonMinSize();

            _currentButtonSelected = _firstDifficultyButton;
            SwitchTextWithFade(_firstDifficultyText);
            ArrowPositionController.MoveArrowToPosition(0);
#if ROWING
            rowingTrackManager?.SetupRaceTrack(250, 2f, CurrentMinigameDifficulty);
#endif

#if FIT_FIGHTER
            if (rhythmGamemodeSystem != null)
            {
                FitFighterManager.selectedDifficulty = CurrentMinigameDifficulty;
                rhythmGamemodeSystem._challengeLevel = FitFighterManager.GetRhythmChallengeLevelConfigByDifficulty(CurrentMinigameDifficulty);
            }
#endif
        } // END HandleFirstButtonClick


        //------------------------------------//
        private void HandleSecondButtonClick()
        //-----------------------------------//
        {
            SoundManager.PlaySound(SharedGameSound.APP_UI_BUTTON_CLICK);

            //new DifficultySelectionChangeEvent(MinigameDifficulty.Regular).TryInvokeShared(this);

            CurrentMinigameDifficulty = MinigameDifficulty.Regular;
            _currentDifficultyData = _minigameDataReceived.SecondDifficultyData;
            UpdateDifficultyInformation();

            HandleLockDifficultyEvent();

            if (_currentButtonSelected == _firstDifficultyButton)
            {
                SecondButtonController.ExpandButtonMinSize();
            }
            else
            {
                ThirdButtonController.ContractButtonMinSize();
                FourthButtonController.ContractButtonMinSize();
                FifthButtonController.ContractButtonMinSize();
            }

            _currentButtonSelected = _secondDifficultyButton;
            SwitchTextWithFade(_secondDifficultyText);
            ArrowPositionController.MoveArrowToPosition(1);
#if ROWING
            rowingTrackManager?.SetupRaceTrack(500, 2.5f, CurrentMinigameDifficulty);
#endif

#if FIT_FIGHTER
            if (rhythmGamemodeSystem != null)
            {
                FitFighterManager.selectedDifficulty = CurrentMinigameDifficulty;
                rhythmGamemodeSystem._challengeLevel = FitFighterManager.GetRhythmChallengeLevelConfigByDifficulty(CurrentMinigameDifficulty);
            }
#endif
        } // END HandleSecondButtonClick



        //----------------------------------//
        private void HandleThirdButtonClick()
        //---------------------------------//
        {
            SoundManager.PlaySound(SharedGameSound.APP_UI_BUTTON_CLICK);

            //new DifficultySelectionChangeEvent(MinigameDifficulty.Veteran).TryInvokeShared(this);

            CurrentMinigameDifficulty = MinigameDifficulty.Veteran;
            _currentDifficultyData = _minigameDataReceived.ThirdDifficultyData;
            UpdateDifficultyInformation();

            HandleLockDifficultyEvent();

            if (_currentButtonSelected == _firstDifficultyButton || _currentButtonSelected == _secondDifficultyButton)
            {
                SecondButtonController.ExpandButtonMinSize();
                ThirdButtonController.ExpandButtonMinSize();
            }
            else
            {
                FourthButtonController.ContractButtonMinSize();
                FifthButtonController.ContractButtonMinSize();
            }

            _currentButtonSelected = _thirdDifficultyButton;
            SwitchTextWithFade(_thirdDifficultyText);
            ArrowPositionController.MoveArrowToPosition(2);
#if ROWING
            rowingTrackManager?.SetupRaceTrack(750, 3f, CurrentMinigameDifficulty);
#endif

#if FIT_FIGHTER
            if (rhythmGamemodeSystem != null)
            {
                FitFighterManager.selectedDifficulty = CurrentMinigameDifficulty;
                rhythmGamemodeSystem._challengeLevel = FitFighterManager.GetRhythmChallengeLevelConfigByDifficulty(CurrentMinigameDifficulty);
            }
#endif
        } // END HandleThirdButtonClick



        //------------------------------------//
        private void HandleFourthButtonClick()
        //------------------------------------//
        {
            SoundManager.PlaySound(SharedGameSound.APP_UI_BUTTON_CLICK);

            //new DifficultySelectionChangeEvent(MinigameDifficulty.Expert).TryInvokeShared(this);

            CurrentMinigameDifficulty = MinigameDifficulty.Expert;
            _currentDifficultyData = _minigameDataReceived.FourthDifficultyData;
            UpdateDifficultyInformation();

            HandleLockDifficultyEvent();

            if (_currentButtonSelected == _fifthDifficultyButton)
            {
                FifthButtonController.ContractButtonMinSize();
            }
            else
            {
                SecondButtonController.ExpandButtonMinSize();
                ThirdButtonController.ExpandButtonMinSize();
                FourthButtonController.ExpandButtonMinSize();
            }

            _currentButtonSelected = _fourthDifficultyButton;
            SwitchTextWithFade(_fourthDifficultyText);
            ArrowPositionController.MoveArrowToPosition(3);
#if ROWING
            rowingTrackManager?.SetupRaceTrack(1000, 3f, CurrentMinigameDifficulty);
#endif

#if FIT_FIGHTER
            if (rhythmGamemodeSystem != null)
            {
                FitFighterManager.selectedDifficulty = CurrentMinigameDifficulty;
                rhythmGamemodeSystem._challengeLevel = FitFighterManager.GetRhythmChallengeLevelConfigByDifficulty(CurrentMinigameDifficulty);
            }
#endif
        } // END HandleFourthButtonClick


        //----------------------------------//
        private void HandleFifthButtonClick()
        //----------------------------------//
        {
            SoundManager.PlaySound(SharedGameSound.APP_UI_BUTTON_CLICK);

            //new DifficultySelectionChangeEvent(MinigameDifficulty.Pro).TryInvokeShared(this);

            CurrentMinigameDifficulty = MinigameDifficulty.Pro;
            _currentDifficultyData = _minigameDataReceived.FifthDifficultyData;
            UpdateDifficultyInformation();

            HandleLockDifficultyEvent();

            SecondButtonController.ExpandButtonMinSize();
            ThirdButtonController.ExpandButtonMinSize();
            FourthButtonController.ExpandButtonMinSize();
            FifthButtonController.ExpandButtonMinSize();

            _currentButtonSelected = _fifthDifficultyButton;
            SwitchTextWithFade(_fifthDifficultyText);
            ArrowPositionController.MoveArrowToPosition(4);
#if ROWING
            rowingTrackManager?.SetupRaceTrack(1500, 3f, CurrentMinigameDifficulty);
#endif

#if FIT_FIGHTER
            if (rhythmGamemodeSystem != null)
            {
                FitFighterManager.selectedDifficulty = CurrentMinigameDifficulty;
                rhythmGamemodeSystem._challengeLevel = FitFighterManager.GetRhythmChallengeLevelConfigByDifficulty(CurrentMinigameDifficulty);
            }
#endif
        } // END HandleFifthButtonClick

        #endregion


        private void SwitchTextWithFade(string nextText)
        {
            _difficultyText.DOFade(0, TextFadeDuration);
            _difficultyText.text = nextText;
            _difficultyText.DOFade(1, TextFadeDuration);
        }

        private void UpdateDifficultyInformation()
        {
            _firstLineInformationValue.text = _currentDifficultyData.FirstLineParameter;
            _secondLineInformationValue.text = _currentDifficultyData.SecondLineParameter;
            _knobLevelSuggestionText.text = _currentDifficultyData.KnobLevelSuggestion;
        }


        private void HandlePlayButtonClick()
        {
#if ROWING
            SoundManager.PlaySound(SharedGameSound.APP_UI_BUTTON_CLICK);
            rowingTrackManager = FindObjectOfType<RowingTrackManager>();

            CanvasController controller = FindObjectOfType<CanvasController>();
            controller?.CheckMaxProConnection();

            if (MaxProConnectionCanvas.isConnected == true && fitFighterManager == null)
            {
                ForceHide();
            }
#endif
        }


        private void HandleLockedHandlePopUpContinueClick()
        {

        }

        private void OpenPopUpWithFade(CanvasGroup canvasGroup, Transform holder)
        {
            canvasGroup.alpha = 0;
            holder.localScale = new Vector3(PopUpFadeScaleIn, PopUpFadeScaleIn, PopUpFadeScaleIn);

            canvasGroup.gameObject.SetActive(true);

            canvasGroup.DOFade(1, PopUpFadeDuration).SetEase(Ease.OutExpo);
            holder.DOScale(1, PopUpFadeDuration).SetEase(Ease.OutExpo);
        }

        /*
        private void HandleMaxProInputReceived(MaxProController sender, GameEventRequestUpdateMaxProCommand command)
        {
            _currentLeftKnobLevel = (int)Mathf.Ceil(command.LeftKnobPosition / 10f);
            _currentRightKnobLevel = (int)Mathf.Ceil(command.RightKnobPosition / 10f);
        }
        */

        public void ForceShow()
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public void ForceHide()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        private void OnDestroy()
        {
            _firstDifficultyButton.onClick.RemoveListener(HandleFirstButtonClick);
            _secondDifficultyButton.onClick.RemoveListener(HandleSecondButtonClick);
            _thirdDifficultyButton.onClick.RemoveListener(HandleThirdButtonClick);
            _fourthDifficultyButton.onClick.RemoveListener(HandleFourthButtonClick);
            _fifthDifficultyButton.onClick.RemoveListener(HandleFifthButtonClick);

            _playButton.onClick.RemoveListener(HandlePlayButtonClick);
            _popUpContinueButton.onClick.RemoveListener(HandleLockedHandlePopUpContinueClick);
        }
    }
}