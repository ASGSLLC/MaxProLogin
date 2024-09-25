using System.Collections;
using System.Collections.Generic;
using System.Globalization;
//using _Project.RowingCanoe.Scripts.CanoeRefactor.Metrics;
using TMPro;
using Unity.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

#if MAXPRO_LOGIN
using DG.Tweening;

#endif
//using App.Scripts.UI.DifficultySelection;
//using _Project.App.Scripts.UI.TrainingRoutine.Results;
using maxprofitness.login;
namespace maxprofitness.shared
{
    public class RowingResultsView : MonoBehaviour
    {
        #region VARIABLES

        
        [SerializeField] private CanvasGroup _root;
        [SerializeField] private RectTransform _panel;
        [SerializeField] private RowingTrackManager _rowingTrackManager;
        
        
        [SerializeField] private TMP_Text _gameResultText;
        [SerializeField] private TMP_Text _gamemodeDifficultyText;
        [SerializeField] private TMP_Text _countDownToContinueText;
        [SerializeField] private TMP_Text _distanceText;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _yourTimeText;
        
        [SerializeField] private TMP_Text _newHighscoreText;
        [SerializeField] private int _currentHighscore;
        [SerializeField] private GameObject metricsButton;

        private const int CountdownTime = 45;
        private const string LoseText = "YOU LOSE";
        private const string WinText = "YOU WIN";

#if MAXPRO_LOGIN
        private MaxProMobileControlSelection _controlsSelection;
        [SerializeField] private CanoeMetricsManager _canoeMetricsManager;
        [SerializeField] private NameInput nameInputUI;
        [SerializeField] private RowingCanoeMatchMetricsScreenController _rowingMetrics;
        [SerializeField] private DifficultySelectionView difficultySelectView;
#endif

        #endregion


        #region MONOBEHAVIOURS


        //-----------------//
        private void Start()
        //-----------------//
        {
            Init();

        } // END Start


        #endregion


        #region INIT


        //----------------//
        private void Init()
        //----------------//
        {
#if MAXPRO_LOGIN
            _panel.DOAnchorPosX(1000, 0);
            _rowingTrackManager = FindObjectOfType<RowingTrackManager>();
            difficultySelectView = FindObjectOfType<DifficultySelectionView>();
            _controlsSelection = FindObjectOfType<MaxProMobileControlSelection>();
#endif

        } // END Init


#endregion


        #region SET FINAL SCORE RACE MODE


        //--------------------------------//
        public void SetFinalScoreRaceMode(int score, float yourTime, float opponentTime)
        //--------------------------------//
        {
#if MAXPRO_LOGIN
            if (_controlsSelection.isUsingMaxProControls == true)
            {
                metricsButton.SetActive(true);
            }
            else if (_controlsSelection.isUsingMaxProControls == false)
            {
                metricsButton.SetActive(false);
            }

            int playerMinutes = (int)(yourTime / 60);
            int playerSeconds = (int)(yourTime - playerMinutes * 60);

            int opponentMinutes = (int)(opponentTime / 60);
            int opponentSeconds = (int)(opponentTime - opponentMinutes * 60);
            string _gameModeType = "Racing";
            
            _gamemodeDifficultyText.text = _gameModeType + " -" + $" {difficultySelectView.CurrentMinigameDifficulty.ToString()}";
            string message = _rowingTrackManager.OpponentFinishedFirst ? LoseText : WinText ;

            _gameResultText.text = message;

            _distanceText.text = $"{_rowingTrackManager.trackLength}";
            _yourTimeText.text = $"{playerMinutes}:{playerSeconds:00}";
            //_scoreText.text = score.ToString(CultureInfo.InvariantCulture);
#endif
        } // END SetFinalScoreRaceMode


#endregion
        
        
        #region ON VIEW METRICS BUTTON PRESSED


        //--------------------------------------//
        public void OnViewMetricsButtonPressed()
        //--------------------------------------//
        {
            gameObject.SetActive(false);
#if MAXPRO_LOGIN
            _canoeMetricsManager.EnableMetricsUI();
#endif
        } // END OnViewMetricsButton
        
        
#endregion


        #region ON RETURN TO HUB


        //-------------------------//
        public void OnReturnToHub()
        //-------------------------//
        {
            //Debug.Log("RowingResultsView.cs // OnReturnToHub() // Returning to Hub Scene");
            SceneManager.LoadSceneAsync(0);

        } // END OnReturnToHub


        #endregion


        #region ON REMTACH BUTTON PRESSED
        
        
        //---------------------------------//
        public void OnRematchButtonPressed()
        //---------------------------------//
        {
            //Debug.Log("RowingResultsView.cs // OnRematchButtonPressed() // Reloading Scene for Rematch");
            //SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);


            _root.alpha = 0.0f;
            _root.interactable = false;
            _root.blocksRaycasts = false;
#if MAXPRO_LOGIN
            nameInputUI.GetGameResults(_rowingMetrics.endScore, 6, true);
#endif
        } // END OnRematchButtonPressed


#endregion


        #region ICOUNTDOWN TO ENABLE METRICS COROUTINE


        //-----------------------------------------------------//
        public IEnumerator ICountdownToEnableMetricsCoroutine()
        //-----------------------------------------------------//
        {
            for (int i = CountdownTime; i >= 0; i--)
            {
                _countDownToContinueText.text = i.ToString();
                yield return new WaitForSeconds(1);
            }

            OnViewMetricsButtonPressed();

        } // END ICountdownToEnableMetricsCoroutine
        
        
        #endregion


        #region OLD / WAITING TO DELETE


        //------------------------------------//
        public void SetFinalScoreObstacleMode(int score, float yourTime, float speed)
        //------------------------------------//
        {
            //_root.gameObject.SetActive(true);
            //_root.interactable = true;
            //_root.blocksRaycasts = true;

            //_root.DOFade(0, 0);
            //_root.DOFade(1, 2);

            //_panel.DOAnchorPosX(0, 1);

            //MinigameDifficulty minigameDifficulty = _rowingTrackManager.MinigameDifficulty;
            //MinigameMode gamemode = _canoeGameSystem.Gamemode;

            //_gamemodeDifficultyText.text = gamemode + " - " + minigameDifficulty;
            //_scoreText.text = score.ToString(CultureInfo.InvariantCulture);

            //int playerMinutes = (int)(yourTime / 60);
            //int playerSeconds = (int)(yourTime - playerMinutes * 60);

            //StartCoroutine(CountdownToEnableMetricsCoroutine());

        } // END SetFinalScoreObstacleMode


        #endregion


    } // END RowingResultsView

} // END Namespace
