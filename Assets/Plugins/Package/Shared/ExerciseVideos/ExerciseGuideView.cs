//using App.Scripts.Application.ExerciseVideoSystem;
//using DanielLochner.Assets.SimpleScrollSnap;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//using DanielLochner.Assets.SimpleScrollSnap;
namespace maxprofitness.shared
{
    public class ExerciseGuideView : MonoBehaviour
    {
        #region VARIABLES
        public Action<string> OnChangeVideoURL;

        [SerializeField] private SimpleScrollSnap _scrollSnap;
        [SerializeField] private RectTransform _scrollableExercisesContainer;
        [SerializeField] private GameObject _scrollableExercisePrefab;
        [SerializeField] private TMP_Text _exerciseDescription;
        [SerializeField] private TMP_Text _exercisePlacement;
        [SerializeField] private TMP_Text _exerciseBodyArea;
        [SerializeField] private TMP_Text _exerciseGoal;
        [SerializeField] private TMP_Text _exerciseAccessory;

        public List<MinigameExerciseData> ExerciseDataList;

        private MinigameExerciseData _selectedExercise;
        private List<MinigameExerciseData> _exercisesAvailableOnMinigame;

        public MinigameExerciseData SelectedExercise => _selectedExercise;
        #endregion

        #region MONOBEHAVIOURS
        //--------------------//
        private void OnEnable()
        //--------------------//
        {
            LoadExerciseData();

            AddEventListeners();

        } // END OnEnable

        //---------------------//
        private void OnDisable()
        //---------------------//
        {
            RemoveEventListeners();

        } // END OnDisable
        #endregion

        #region ADD / REMOVE LISTENERS
        //------------------------------//
        private void AddEventListeners()
        //----------------------------//
        {
            _scrollSnap.onPanelChanged.AddListener(UpdateExerciseUI);

        } // END AddEventListeners

        //---------------------------------//
        private void RemoveEventListeners()
        //--------------------------------//
        {
            _scrollSnap.onPanelChanged.RemoveAllListeners();

        } // END RemoveEventListeners
        #endregion

        #region LOAD EXERCISE DATA
        //-----------------------------//
        private void LoadExerciseData()
        //----------------------------//
        {
            //IExerciseVideoService exerciseVideoService = ServiceLocator.Shared.Get<IExerciseVideoService>();
            // _exercisesAvailableOnMinigame = exerciseVideoService?.GetMinigameExercise();

            _exercisesAvailableOnMinigame = ExerciseDataList;
            InitializeExerciseScroll(_exercisesAvailableOnMinigame);

        } // END LoadExerciseData
        #endregion

        #region UPDATE EXERCISE UI
        //-----------------------------//
        public void UpdateExerciseUI()
        //-----------------------------//
        {
            //_selectedExercise = _scrollSnap.GetCurrentPanel().GetComponent<ScrollableExercise>().ExerciseData;
            OnChangeVideoURL?.Invoke(_selectedExercise.VideoUrl);

            _exerciseDescription.SetText(_selectedExercise.Description);
            _exercisePlacement.SetText(GetPlacementName(_selectedExercise.Placement));
            _exerciseBodyArea.SetText(GetBodyAreaName(_selectedExercise.BodyArea));
            _exerciseGoal.SetText(GetGoalName(_selectedExercise.Goal));
            _exerciseAccessory.SetText(GetAccessoryName(_selectedExercise.Accessory));

        } // END UpdateExerciseUI
        #endregion

        #region INITIALIZE EXERCISE SCROLL
        //---------------------------------------------------------------------------------//
        private void InitializeExerciseScroll(List<MinigameExerciseData> availableExercises)
        //--------------------------------------------------------------------------------//
        {
            int numberOfExercisesActive = _scrollableExercisesContainer.childCount;

            for (int i = 0; i < numberOfExercisesActive; i++)
            {
                DestroyImmediate(_scrollableExercisesContainer.GetChild(0).gameObject);
            }

            for (int i = 0; i < availableExercises.Count; i++)
            {
                _scrollSnap.AddToBack(_scrollableExercisePrefab);
                //_scrollSnap.Panels[i].GetComponent<ScrollableExercise>().Initialize(_exercisesAvailableOnMinigame[i]);
            }

            UpdateExerciseUI();

        } // END InitializeExerciseScroll
        #endregion

        #region EXERCISE PLACEMENT/BODY AREA/GOAL/ACCESSORY NAME
        //---------------------------------------------------------//
        private string GetPlacementName(ExercisePlacement placement)
        //---------------------------------------------------------//
        {
            return placement switch
            {
                ExercisePlacement.Floor => "Floor",
                ExercisePlacement.UpperTrack => "Upper Track",
                ExercisePlacement.LowerTrack => "Lower Track",
                ExercisePlacement.Bench => "Bench",
                _ => throw new ArgumentOutOfRangeException(nameof(placement), placement, null),
            };

        } // END GetPlacementName


        //------------------------------------------------------//
        private string GetBodyAreaName(ExerciseBodyArea bodyArea)
        //------------------------------------------------------//
        {
            return bodyArea switch
            {
                ExerciseBodyArea.UpperBody => "Upper Body",
                ExerciseBodyArea.LowerBody => "Lower Body",
                ExerciseBodyArea.FullBody => "Full Body",
                ExerciseBodyArea.Core => "Core",
                _ => throw new ArgumentOutOfRangeException(nameof(bodyArea), bodyArea, null),
            };

        } // END GetBodyAreaName


        //-------------------------------------------//
        private string GetGoalName(ExerciseGoal goal)
        //-------------------------------------------//
        {
            return goal switch
            {
                ExerciseGoal.Burn => "Burn",
                ExerciseGoal.Build => "Build",
                ExerciseGoal.Tone => "Tone",
                _ => throw new ArgumentOutOfRangeException(nameof(goal), goal, null),
            };

        } // END GetGoalName


        //----------------------------------------------------------//
        private string GetAccessoryName(ExerciseAccessory accessory)
        //---------------------------------------------------------//
        {
            return accessory switch
            {
                ExerciseAccessory.LongShortBar => "Long/Short Bar",
                ExerciseAccessory.Handles => "Handles",
                ExerciseAccessory.SuspensionHandles => "Suspension Handles",
                ExerciseAccessory.Straps => "Straps",
                ExerciseAccessory.JumpBelt => "Jump Belt",
                ExerciseAccessory.Bench => "Bench",
                _ => throw new ArgumentOutOfRangeException(nameof(accessory), accessory, null),
            };

        } // END GetAccessory


        #endregion


    } // END ExerciseGuideView.cs
}