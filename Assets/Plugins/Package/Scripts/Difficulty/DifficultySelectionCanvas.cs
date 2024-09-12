//using MaxProFitness.App.UI;
//using MaxProFitness.SharedSound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using maxprofitness.login;

public class DifficultySelectionCanvas : CanvasControllerBase
{
    [Header("Back Button")]
    [SerializeField] private Button _backButton;

    [SerializeField] private CanvasGroup modeSelectCanvas;
    [SerializeField] private CanvasGroup difficultyCanvas;
    [SerializeField] private CanvasGroup exerciseCanvas;

    private ExerciseGuideView exerciseGuideView;

    private void Awake()
    {
        exerciseGuideView = FindObjectOfType<ExerciseGuideView>();
    }

    #region BACK TO MODE SELECT CANVAS
    //--------------------------//
    public void BackToExerciseCanvas()
    //--------------------------//
    {
        ToggleCanvasGroup(exerciseCanvas, true);
        ToggleCanvasGroup(difficultyCanvas, false);

        exerciseGuideView.UpdateExerciseUI();

    } // END BackToExerciseCanvas
    #endregion

    #region TOGGLE CANVAS GROUP


    //-----------------------------------------------------//
    public void ToggleCanvasGroup(CanvasGroup _canvasGroup, bool _isActive)
    //----------------------------------------------------//
    {
        if (_isActive == true)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = _isActive;
            _canvasGroup.interactable = _isActive;
        }
        else if (_isActive == false)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = _isActive;
            _canvasGroup.interactable = _isActive;
        }

    } // END ToggleCanvasGroup


    #endregion


} // END DifficultySelectionCanvas.cs
