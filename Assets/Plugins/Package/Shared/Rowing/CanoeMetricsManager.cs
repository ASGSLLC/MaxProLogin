using System;
using UnityEngine;
using UnityEngine.Serialization;
using maxprofitness.login;

namespace maxprofitness.shared
{
    public class CanoeMetricsManager : MonoBehaviour
    {
        [SerializeField] private RowingTrackManager _rowingTrackManager;
        [SerializeField] private ResultsRoutineController _resultsRoutineController;
        [SerializeField] private RowingCanoeMatchMetricsScreenController _rowingCanoeMatchMetricsScreenController;


        //------------------//
        private void Start()
        //-----------------//
        {
            _rowingTrackManager.OnGameFinished += OnGameFinished;

        } // END Start
        
        
        #region ON GAME FINISHED


        //--------------------------------------------------//
        private void OnGameFinished(MatchResult matchResult)
        //--------------------------------------------------//
        {
            _rowingCanoeMatchMetricsScreenController.SetMatchMetricsScreen(matchResult);

        } // END OnGameFinished


        #endregion


        #region ENABLE METRICS UI


        //--------------------------//
        public void EnableMetricsUI()
        //--------------------------//
        {
            _resultsRoutineController.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
            _resultsRoutineController.gameObject.GetComponent<CanvasGroup>().interactable = true;
            _resultsRoutineController.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;

        } // END EnableMetricsUI


        #endregion

    } // END CanoeMetricsManager.cs

} // END Namespace
