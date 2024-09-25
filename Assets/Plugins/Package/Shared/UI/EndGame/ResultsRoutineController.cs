using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using maxprofitness.login;

namespace maxprofitness.shared
{
    public class ResultsRoutineController : MonoBehaviour
    {
        #region VARIABLES


        [SerializeField] private Button _continueButton;

        [SerializeField] private RowingCanoeMatchMetricsScreenController _rowingMetrics;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private NameInput nameInputUI;


        #endregion

        //----------------------------------//
        public void OnClick_ShowMainMenu()
        //---------------------------------//
        {
            //SceneManager.LoadSceneAsync(0);
            
            canvasGroup.alpha = 0.0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            nameInputUI.GetGameResults(_rowingMetrics.endScore, 6, true);

        } // END OnClick_ShowMainMenu

    }
}
