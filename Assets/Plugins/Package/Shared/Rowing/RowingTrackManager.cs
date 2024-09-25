using System;
using System.Collections;
using UnityEngine;
using System.IO;

#if MAXPRO_LOGIN && FIREBASE
//using Firebase.Firestore;
//using Firebase.Database;
#endif

#if UNITY_EDITOR
using UnityEditor.PackageManager;
#endif

#if MAXPRO_LOGIN
using maxprofitness.login;
#endif

namespace maxprofitness.shared
{
    public class RowingTrackManager : MonoBehaviour
    {
        #region VARIABLES


        public event Action<MatchResult> OnGameFinished;        

        public MatchResult MatchResult => _matchResult;
        public MinigameDifficulty MinigameDifficulty => _selectedMinigameDifficulty;

        [SerializeField] private StartMinigameChannel _startMinigameChannel;
        [SerializeField] private EndMinigameChannel _endMinigameChannel;
        [SerializeField] private WorkCalculatorController _workCalculatorController;

        private MinigameDifficulty _selectedMinigameDifficulty;
        private MatchResult _matchResult;
        private UserDataManager userDataManager;
        private MinigameMode _gameMode;
        
        public event Action<float> OnElapsedTimeUpdated;
        public int finalScore;

        [SerializeField] private RowingCanoeGameManager rowingCanoeGameManager;
        [SerializeField] private RowingCanoeRacePlayerController _playerController;

        [SerializeField] private OpponentController _opponentController;
        [SerializeField] private CanvasController _canvasController;

        [Header("Camera")]
        [SerializeField] private Animation _cameraAnimation;

        [Header("Properties")]
        [SerializeField] private float _prepareTime;

        [Tooltip("This will multiply the distance required to beat the level by X amount and divide the displayed distance by X amount.")]
        [SerializeField] private int _scaleFactor = 10;

        [SerializeField] private GameObject _finishFlagPrefab;
        [SerializeField] private Vector3 _finishFlagPositionOffset;
        [SerializeField] private Vector3 _finishFlagRotationOffset;

        [Header("Debug settings")] [Tooltip("Enable this for testing on the scene")]
        [SerializeField] private bool _debugMode;

        private bool _opponentFinishedFirst;
        private float _elapsedTime;
        private float _playerPaceMinutes;
        private float _playerPaceSeconds;

        private const float SplitDistance = 500f;

        public float trackLength;

        public OpponentController OpponentController => _opponentController;
        public float OpponentSpeed => OpponentController.DefaultSpeed;

        public CanvasController canvasController;
        public CanoeController canoeController;

        public bool OpponentFinishedFirst => _opponentFinishedFirst;

        public bool hasPlayerFinishedFirst;
        public bool hasStarted = false;
        private static string rowingMetricsFilePath = "/RowingMetricsFile.json";
        

#endregion


#region ON AWAKE


        //-------------------//
        public void Init()
        //-------------------//
        {
            //Debug.Log("RowingTrackManager.cs // OnAwake() //");

            canoeController = FindObjectOfType<CanoeController>();
            canvasController = FindObjectOfType<CanvasController>();
            userDataManager = FindObjectOfType<UserDataManager>();

            hasStarted = false;

        } // END OnAwake


#endregion


#region MONOBEHAVIOURS


        //------------------//
        private void Start()
        //------------------//
        {
            StartTrack();   

        } // END Start



        //------------------//
        private void Update()
        //------------------//
        {
            CheckIfOpponentFinished();
            CheckIfPlayerFinished();

        } // END Update


#endregion


#region CHECK IF OPPONONENT / PLAYER FINISHED


        //------------------------------------//
        private void CheckIfOpponentFinished()
        //------------------------------------//
        {

            if (!(_opponentController.DistanceCovered >= trackLength))
            {
                if(rowingCanoeGameManager.hasStarted == false)
                {
                    _opponentFinishedFirst = false;
                    _opponentController.ToggleMovement(false);
                }
                else
                {
                    _opponentFinishedFirst = false;
                    _opponentController.ToggleMovement(true);
                }
                
                return;
            }
            else
            {
                _opponentFinishedFirst = true;
                _opponentController.ToggleMovement(false);
            }

        } // END CheckIfOpponentFinished


        //------------------------------------//
        private void CheckIfPlayerFinished()
        //------------------------------------//
        {
            if (!(_playerController.DistanceCovered >= trackLength) && _opponentFinishedFirst == false)
            {
                hasPlayerFinishedFirst = false;
                return;
            }
            else
            {
                _opponentFinishedFirst = true;
                _opponentController.ToggleMovement(false);
            }

        } // END CheckIfPlayerFinished


#endregion


#region START TRACK


        //-----------------------//
        private void StartTrack()
        //----------------------//
        {
            //Debug.Log("RowingTrackManager.cs // StartTrack() // ");

            if(userDataManager.rowingTrackManager == null)
            {
                userDataManager.rowingTrackManager = this;
            }

            rowingCanoeGameManager.StartGame();

        } // END StartTrack


#endregion


#region GAMEPLAY LOOP COROUTINE


        //-----------------------------------------//
        public IEnumerator GameplayLoopCoroutine()
        //-----------------------------------------//
        {
            while (_playerController.DistanceCovered < trackLength && hasStarted == true)
            {
                UpdateDistances();
                UpdateCurrentSpeed();
                UpdateElapsedTime();

                yield return null;
            }

        } // END GameplayLoopCoroutine


#endregion


#region SETUP RACE TRACK


        //--------------------------//
        public void SetupRaceTrack(float _trackLength, float _opponentSpeed, MinigameDifficulty _difficulty)
        //--------------------------//
        {
            //Debug.Log("RowingTrackManager.cs // SetupRaceTrack() // ");

            _selectedMinigameDifficulty = _difficulty;

            SetTrackLength(_trackLength);
            SpawnFinishFlag();

            _opponentFinishedFirst = false;
            _opponentController.SetDefaultSpeed(_opponentSpeed);
            _opponentController.SetOpponentSpeedToDefault();

            StartTrack();

        } // END SetupRaceTrack


        #endregion


        #region SET TRACK LENGTH


        //---------------------------------------//
        private void SetTrackLength(float _length)
        //---------------------------------------//
        {
            Debug.Log("RowingTrackManager.cs // SetTrackLength() // ");

            trackLength = _length * _scaleFactor;

        } // END SetTrackLength


        #endregion



        #region SPAWN FINISH FLAG


        //----------------------------//
        private void SpawnFinishFlag()
        //----------------------------//
        {
            Vector3 spawnPosition = _finishFlagPositionOffset + new Vector3(0, 0, -trackLength);
            _finishFlagPrefab.transform.position = spawnPosition;
            _finishFlagPrefab.SetActive(true);

        } // END SpawnFinishFlag


        #endregion


        #region PLAY INITIAL CAMERA ANIMATION


        //---------------------------------------//
        public bool PlayInitialCameraAnimation()
        //---------------------------------------//
        {
            Debug.Log("RowingTrackManager.cs // PlayInitialCameraAnimation() // ");
            _cameraAnimation.Play();

            return true;
            
        } // END PlayInitialCameraAnimation


        #endregion


        #region FINISH TRACK


        //------------------------//
        private void FinishTrack()
        //------------------------//
        {
            GameManager.isGamePlaying = false;
            GameManager.isRecieveingInput = false;

            CanoeEvents.RowingRaceEndedEvent?.Invoke();

            _playerController.SetPlayerSpeedAfterFinish();
            rowingCanoeGameManager.FinishGame();
            rowingCanoeGameManager.hasStarted = false;

            CalculateScore();
            CalculatePlayerPace();

            ProcessTimelines();
            ProcessMatchResult();

        } // END FinishTrack


#endregion


#region CALCULATE SCORE


        //---------------------------//
        private void CalculateScore()
        //--------------------------//
        {
            //Debug.Log("Calculated Score: ");

            float timeThreshold = (trackLength / OpponentSpeed) * 2;
            float timeScore = (timeThreshold / _elapsedTime) * trackLength;
            float opponentTime = trackLength / (_opponentController.DefaultSpeed / 10);

            finalScore = _opponentFinishedFirst ? (int)trackLength : (int)timeScore;

            _canvasController.SetFinalScoreRaceMode(finalScore, _elapsedTime, opponentTime);

        } // END CalculateScore


        #endregion


        #region FINISH GAME


        //----------------------//
        public void FinishGame()
        //---------------------//
        {
            FinishTrack();

        } // END FinishGame


#endregion


#region UI HUD UPDATES


        //----------------------------//
        private void UpdateDistances()
        //----------------------------//
        {
            _opponentController.ScaledDistanceCovered = _opponentController.DistanceCovered / _scaleFactor;
            _playerController.ScaledDistanceCovered = _playerController.DistanceCovered / _scaleFactor;

        } // END UpdateDistances


        //-------------------------------//
        private void UpdateCurrentSpeed()
        //-------------------------------//
        {
            _canvasController.UpdateCurrentSpeed(_playerController.Speed);

        } // END UpdateCurrentSpeed


        //------------------------------//
        private void UpdateElapsedTime()
        //-----------------------------//
        {
            _elapsedTime += Time.deltaTime;
            OnElapsedTimeUpdated?.Invoke(_elapsedTime);

        } // END UpdateElapsedTime


#endregion


#region PROCESS TIMELINES


        //-----------------------------//
        private void ProcessTimelines()
        //-----------------------------//
        {
            if (!OpponentFinishedFirst)
            {
                CanoeEvents.WinRaceTimelineEvent?.Invoke();
            }
            else
            {
                CanoeEvents.LoseRaceTimelineEvent?.Invoke();
            }

        } // END ProcessTimelines


#endregion


        #region SAVE/LOAD ROWING METRICS TO/FROM JSON


        //-----------------------------//
        public void SaveLocalRowingMetricsToJson(MatchResult _matchResult, RowingMetrics _rowingMetricsResult)
        //-----------------------------//
        {
            Debug.Log("RowingTrackManager.cs // SaveLocalRowingMetricsToJson() //");

            UserDataMeta profileData = UserDataManager.loadedData;

            if (profileData == null)
            {
                UserDataManager userDataManager = FindObjectOfType<UserDataManager>();

                if (userDataManager.localLeaderboard == null)
                {
                    userDataManager.localLeaderboard = FindObjectOfType<LocalLeaderboard>();
                }

                //Grab User Data
                userDataManager.InitializeUserData((snapShot) =>
                {
                    userDataManager.ReadUserDocument(snapShot);
                    
                    Debug.Log("UserProfilePage.cs // ReceiveProfileData() // Successfully got data");

                    GameMetrics.RowingCanoeGameMetrics _rowingMetrics = new GameMetrics.RowingCanoeGameMetrics();
                    _rowingMetrics.maxSpeed = _rowingMetricsResult.MaxSpeed;
                    _rowingMetrics.averageSpeed = _rowingMetricsResult.AverageSpeed;
                    _rowingMetrics.strokes = _rowingMetricsResult.Strokes;
                    _rowingMetrics.paceMinutes = _rowingMetricsResult.PaceMinutes;
                    _rowingMetrics.paceSeconds = _rowingMetricsResult.PaceSeconds;
                    _rowingMetrics.cadence = _rowingMetricsResult.Cadence;
                    _rowingMetrics.highScore = _matchResult.Score;
                    _rowingMetrics.averageWork = _matchResult.AverageWork;
                    _rowingMetrics.caloriesBurned = _matchResult.CaloriesBurned;
                    _rowingMetrics.repetitions = _matchResult.Repetitions;
                    _rowingMetrics.workByRepetitions = _matchResult.WorkByRepetitions.ToArray();
                    _rowingMetrics.powerList = _matchResult.PowerList.ToArray();
                    _rowingMetrics.peakWork = _matchResult.PeakWork;
                    _rowingMetrics.totalWork = _matchResult.TotalWork;
                    _rowingMetrics.date = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    _rowingMetrics.timeToFinish = _matchResult.TimeToFinish.Hours.ToString() + ":" + _matchResult.TimeToFinish.Minutes.ToString() + ":" + _matchResult.TimeToFinish.Seconds.ToString();
                    _rowingMetrics.leaderboardScore[0] = UserDataManager.loadedData.firstName;
                    _rowingMetrics.leaderboardScore[1] = _rowingMetrics.highScore.ToString();

                    string _json = JsonUtility.ToJson(_rowingMetrics, true);
                    File.WriteAllText(Application.persistentDataPath + rowingMetricsFilePath, _json);

                    Debug.Log(Application.persistentDataPath + rowingMetricsFilePath);

                    UserDataManager.Instance.RecieveRowingData();
                    UserDataManager.Instance.UpdateRowingLeaderboardEntries(_rowingMetrics.leaderboardScore[0].ToLower(), _rowingMetrics.leaderboardScore[1]);

                }, (error) =>
                {
                    Debug.Log("UserProfilePage.cs // ReceiveProfileData() // ERROR: Was not able to initialize user data");
                });
            }
            else
            {
                GameMetrics.RowingCanoeGameMetrics _rowingMetrics = new GameMetrics.RowingCanoeGameMetrics();
                _rowingMetrics.maxSpeed = _rowingMetricsResult.MaxSpeed;
                _rowingMetrics.averageSpeed = _rowingMetricsResult.AverageSpeed;
                _rowingMetrics.strokes = _rowingMetricsResult.Strokes;
                _rowingMetrics.paceMinutes = _rowingMetricsResult.PaceMinutes;
                _rowingMetrics.paceSeconds = _rowingMetricsResult.PaceSeconds;
                _rowingMetrics.cadence = _rowingMetricsResult.Cadence;
                _rowingMetrics.highScore = _matchResult.Score;
                _rowingMetrics.averageWork = _matchResult.AverageWork;
                _rowingMetrics.caloriesBurned = _matchResult.CaloriesBurned;
                _rowingMetrics.repetitions = _matchResult.Repetitions;
                _rowingMetrics.workByRepetitions = _matchResult.WorkByRepetitions.ToArray();
                _rowingMetrics.powerList = _matchResult.PowerList.ToArray();
                _rowingMetrics.peakWork = _matchResult.PeakWork;
                _rowingMetrics.totalWork = _matchResult.TotalWork;
                _rowingMetrics.date = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                _rowingMetrics.timeToFinish = _matchResult.TimeToFinish.Hours.ToString() + ":" + _matchResult.TimeToFinish.Minutes.ToString() + ":" + _matchResult.TimeToFinish.Seconds.ToString();
                _rowingMetrics.leaderboardScore[0] = UserDataManager.loadedData.firstName;
                _rowingMetrics.leaderboardScore[1] = _rowingMetrics.highScore.ToString();

                string _json = JsonUtility.ToJson(_rowingMetrics, true);
                File.WriteAllText(Application.persistentDataPath + rowingMetricsFilePath, _json);

                Debug.Log(Application.persistentDataPath + rowingMetricsFilePath);

                UserDataManager.Instance.RecieveRowingData();
                UserDataManager.Instance.UpdateRowingLeaderboardEntries(_rowingMetrics.leaderboardScore[0].ToLower(), _rowingMetrics.leaderboardScore[1]);
            }

        } // END SaveMetricsToJson


        //-----------------------------------//
        public static GameMetrics.RowingCanoeGameMetrics GetRowingMetricsFromJson()
        //-----------------------------------//
        {
            string _json = File.ReadAllText(Application.persistentDataPath + rowingMetricsFilePath);
            GameMetrics.RowingCanoeGameMetrics _rowingMetrics = JsonUtility.FromJson<GameMetrics.RowingCanoeGameMetrics>(_json);

            return _rowingMetrics;

        } // END LoadRowingMetricsToJson


        //---------------------------------------//
        public void LoadRowingStatsFromFirebase()
        //---------------------------------------//
        {
            UserDataManager.Instance.GetRowingLeaderboardEntries();

        }


        #endregion


            #region PROCESS MATCH RESULTS


            //-------------------------------//
            private void ProcessMatchResult()
        //------------------------------//
        {

            Minigame actualMinigame = new Minigame
            {
                Difficulty = _selectedMinigameDifficulty,
                Type = MinigameType.RowingCanoe,
                Mode = _gameMode
            };

            RowingMetrics rowingMetrics = new RowingMetrics
            {
                MaxSpeed = _playerController.MetricsMaxSpeed,
                AverageSpeed = _playerController.MetricsAverageSpeed,
                Strokes = _workCalculatorController.Repetitions.Count,
                PaceMinutes = _playerPaceMinutes,
                PaceSeconds = _playerPaceSeconds,
                Cadence = Mathf.RoundToInt(_workCalculatorController.Repetitions.Count / (_elapsedTime / 60)),
            };

            MatchResult matchResult = new MatchResult
            {
                AverageWork = _workCalculatorController.GetAverageWork(),
                CaloriesBurned = _workCalculatorController.GetCaloriesBurned(),
                TimeToFinish = new TimeStruct
                {
                    Hours = 0,
                    Minutes = (int)(_elapsedTime / 60),
                    Seconds = (int)(_elapsedTime - (int)(_elapsedTime / 60) * 60),
                },
                Date = new DateStruct
                {
                    Day = DateTime.UtcNow.Day,
                    Month = DateTime.UtcNow.Month,
                    Year = DateTime.UtcNow.Year,
                },
                Minigame = actualMinigame,
                Score = finalScore,
                PeakWork = _workCalculatorController.GetWorkPeak(),
                TotalWork = _workCalculatorController.GetTotalWork(),
                WorkByRepetitions = _workCalculatorController.AverageWorkByRepetition,
                PowerList = _workCalculatorController.PowerList,
                RowingMetrics = rowingMetrics,
            };

            _matchResult = matchResult;
            
            SaveLocalRowingMetricsToJson(matchResult, rowingMetrics);

            OnGameFinished?.Invoke(matchResult);

        } // END ProcessMatchResults
        
        
#endregion


        #region CALCULATE PLAYER PACE


        //-------------------------------//
        private void CalculatePlayerPace()
        //------------------------------//
        {
            //Split (pace) = 500 * (Time in Seconds / Distance in Meters)

            float playerTime = _elapsedTime;
            float trackDistance = trackLength;

            float split = SplitDistance * (playerTime / trackDistance);

            _playerPaceMinutes = Mathf.FloorToInt(split / 60);
            _playerPaceSeconds = split % 60;

        } // END CalculatePlayerPace
        
        
#endregion


    } // END RowingTrackManager.cs

} // END Namespace