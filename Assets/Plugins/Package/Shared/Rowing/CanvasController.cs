using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

#if MAXPRO_LOGIN
using MaxProFitness.Sdk;
using maxprofitness.login;
#endif

namespace maxprofitness.shared
{
    public class CanvasController : MonoBehaviour
    {

        #region VARIABLES


        [Header("Broadcasting On")]
        [SerializeField] private BoolEventChannelSO _finishedIntroductionChannel;
        [SerializeField] private RowingCanoeGameManager _rowingCanoeGameManager;
        [SerializeField] private RowingResultsView _rowingResultsView;
        [SerializeField] private IntroCountdownController _introCountdownController;
        [SerializeField] private RowingSpeedView _gameplayView;
        [SerializeField] private RowingTrackManager rowingTrackManager;

#if MAXPRO_LOGIN
        [SerializeField] private DifficultySelectionCanvas difficultyCanvas;
        [SerializeField] private IntroAnimHelper introAnimHelper;

        private MaxProConnectionCanvas connectionCanvas;
#endif

#endregion

        #region MONOBEHAVIOURS


        private void Awake()
        {
            _finishedIntroductionChannel.RaiseEvent(true);

            _rowingCanoeGameManager.OnGameStarted += ActivateGameplayCanvas;
            _rowingCanoeGameManager.OnGameFinish += ActivatePostRowingCanoeGameCanvas;

#if MAXPRO_LOGIN
            connectionCanvas = FindObjectOfType<MaxProConnectionCanvas>();
            GameManager.OnConnectionStateChanged += OnConnectionStateChanged;
#endif
            rowingTrackManager = FindObjectOfType<RowingTrackManager>();

            HideDifficultyCanvas();
        }


        //-----------------//
        private void Start()
        //-----------------//
        {
            HideDifficultyCanvas();
#if MAXPRO_LOGIN
            introAnimHelper = FindObjectOfType<IntroAnimHelper>();
#endif
        }


        //----------------------//
        private void OnDestroy()
        //---------------------//
        {
            _rowingCanoeGameManager.OnGameStarted -= ActivateGameplayCanvas;
            _rowingCanoeGameManager.OnGameFinish -= ActivatePostRowingCanoeGameCanvas;
#if MAXPRO_LOGIN
            GameManager.OnConnectionStateChanged -= OnConnectionStateChanged;
#endif
        } // END Destroy


#endregion

        public void CheckMaxProConnection()
        {
#if MAXPRO_LOGIN
            // if we are not connected
            if (MaxProConnectionCanvas.isConnected == false)
            {
                //Debug.Log("MaxProConnectinoCanvas//Start// Show");
                //connectionCanvas.Show();
                CanvasGroupUIBase _connectionCanvasGroup = connectionCanvas.GetComponent<CanvasGroupUIBase>();
                _connectionCanvasGroup.ForceShow();
            }
            else // If we are already connected
            {
                introAnimHelper = FindObjectOfType<IntroAnimHelper>();

                if (introAnimHelper != null)
                {
                    PlayInitialCameraAnim();
                }
                else
                {
                    // Fit Fighter intro here
                }
            }
#endif
        }

        
        public void StartRowingGame()
        {
            
#if MAXPRO_LOGIN
            introAnimHelper = FindObjectOfType<IntroAnimHelper>();
            if (introAnimHelper != null)
            {
                Debug.Log("MaxProConnectionCanvas.cs // OnConnected() // Played CameraAnimIntro");

                PlayInitialCameraAnim();
            }
#endif
        }

#if MAXPRO_LOGIN
        // Previouslu OnConnected in Connection canvas 
        private void OnConnectionStateChanged(MaxProControllerState state)
        {
            
        } // END OnConnectionStateChanged
    
#endif
        #region DELAY INITAL CAMERA ANIM


        /// <summary>
        /// Start the Camera Animation.  It has an animation event that starts the Countdown
        /// </summary>
        //-----------------------------------//
        private void PlayInitialCameraAnim()
        //-----------------------------------//
        {
#if MAXPRO_LOGIN
            introAnimHelper = FindObjectOfType<IntroAnimHelper>();

            if(introAnimHelper != null)
            {
                introAnimHelper.GetComponent<Animation>().Play();
            }
#endif
        } // END DelayInitialCameraAnim


        #endregion

        //--------------------------------//
        public void HideDifficultyCanvas()
        //-------------------------------//
        {
            
        }

        public void SetFinalScoreObstacleMode(int score, float time, float speed)
        {
            _rowingResultsView.SetFinalScoreObstacleMode(score, time, speed);
        }

        public void SetFinalScoreRaceMode(int score, float playerTime, float opponentTime)
        {
            _rowingResultsView.SetFinalScoreRaceMode(score, playerTime, opponentTime);
        }

        private void ActivateGameplayCanvas()
        {
#if MAXPRO_LOGIN
            CanvasGroupUIBase _rowingResultsCanvas = _rowingResultsView?.GetComponent<CanvasGroupUIBase>();
            _rowingResultsCanvas.ForceHide();
#endif
        }

        private void ActivatePostRowingCanoeGameCanvas()
        {
#if MAXPRO_LOGIN
            CanvasGroupUIBase _rowingResultsCanvas = _rowingResultsView.GetComponent<CanvasGroupUIBase>();
            _rowingResultsCanvas.ForceShow();
#endif
        }

        public void ActiveLoadingScreen()
        {
#if MAXPRO_LOGIN
            CanvasGroupUIBase _rowingResultsCanvas = _rowingResultsView.GetComponent<CanvasGroupUIBase>();
            _rowingResultsCanvas.ForceHide();
#endif
        }

        public void UpdateCurrentSpeed(float speed)
        {
            _gameplayView.UpdateCurrentSpeed(speed);
        }

        public IEnumerator GameplayCountDownCoroutine(float time)
        {
            Debug.Log("CanvasController.cs // GameplayCountDownCoroutine() // Started Countdown");

            while (time > 0)
            {
                time -= Time.deltaTime;

                yield return null;
            }

            Debug.Log("CanvasController.cs // GameplayCountDownCoroutine() // Finished Countdown");

            _finishedIntroductionChannel.RaiseEvent(true);
        }


        }
}
