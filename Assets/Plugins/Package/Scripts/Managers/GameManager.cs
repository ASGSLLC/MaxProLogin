//using MaxProFitness.Integrations;
using MaxProFitness.Sdk;
using System;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using maxprofitness.login;
using UnityEngine.Android;
using UnityEngine.Scripting;

    public class GameManager : MonoBehaviour
    {
        #region VARIABLES
        private static GameManager m_Instance;

        /// <summary>
        /// Access singleton instance through this propriety.
        /// </summary>
        public static GameManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    // Search for existing instance.
                    m_Instance = (GameManager)FindObjectOfType(typeof(GameManager));

                    // Create new instance if one doesn't already exist.
                    if (m_Instance == null)
                    {
                        // Need to create a new GameObject to attach the singleton to.
                        var singletonObject = new GameObject();
                        m_Instance = singletonObject.AddComponent<GameManager>();
                        singletonObject.name = typeof(GameManager).ToString() + " (Singleton)";
                    }

                    // Make instance persistent.
                    DontDestroyOnLoad(m_Instance.gameObject);
                }

                return m_Instance;

            }
        }

        public static Action<MaxProControllerState> OnConnectionStateChanged;
        public static Action OnGameStarted;
        public static Action OnDestroyed;
        public static Action<bool> OnGameEnded;

        public static Action<float, float> OnInputUpdated;

        private bool isGameModeInputOn = false;

        public static bool isGamePlaying = false;

        public MaxProController maxProController;

        [SerializeField]
        public class GameInput
        {
            public int LeftKnobPosition;
            public int LeftDistance;
            public int RightKnobPosition;
            public int RightDistance;
        }

        [SerializeField]
        public class RepInput
        {
            public int EventType;
            public int Model;
            public int Error;
            public int BatteryPercent;

            public int LeftKnobPosition;
            public int LeftDistance;
            public int LeftTime;
            public int LeftVelocity;
            public int LeftAcceleration;
            public int LeftRepsCount;

            public int RightKnobPosition;
            public int RightDistance;
            public int RightTime;
            public int RightVelocity;
            public int RightAcceleration;
            public int RightRepsCount;
        }

        public static GameInput gameInput;

        public static RepInput repInput;

        public static Action<RepInput> repComplete;

        public static float leftDownPosition = 480;
        public static float rightDownPosition = 480;

        public static float leftUpPosition = 1400;
        public static float rightUpPosition = 1400;

        public static float leftPull;
        public static float rightPull;
#if AIR_RUNNER
    public static LevelScriptableObject selectedLevel;
#endif
        // public static bool isRecieveingInput = true;
        public static bool isRecieveingInput = true;


        public static Action<int, int> OnNobChanged;

        public enum Difficulty
        {
            Easy,
            Middle,
            Hard,
            Max
        };

        public enum InputDirection
        {
            None,
            Left,
            Right,
            Up
        };

        public static InputDirection inputDirection = InputDirection.None;

        public static Difficulty difficulty = Difficulty.Easy;

        public int currentLNob;
        private int lastLNob;

        public int currentRNob;
        private int lastRNob;

        public int currentAvgNob;
        private int lastAvgNob;

        public static bool isBothUp { get { return (isLeftUp && isRightUp); } }
        public static bool isLeftUp = false;
        public static bool isRightUp = false;
        public static float avrgPull;
#if AIR_RUNNER
    private CheckpointCompass checkpointCompass;
#endif
        public static Action<InputDirection> OnRepUp;
        public static Action<InputDirection> OnRepDown;

        /*
            public static Action<Checkpoint.CheckpointType> OnRepUp;
            public static Action<Checkpoint.CheckpointType> OnRepDown;
            */

        private float leftStart;
        private float rightStart;

        private float leftEnd;
        private float rightEnd;

        public class ControlVarsGameMode
        {
            public int LTime;
            public float LDist;
            public float LVel;
            public float LAcc;
            public int LNum;

            public int RTime;
            public float RDist;
            public float RVel;
            public float RAcc;
            public int RNum;

            public bool MakingRepL;
            public bool MakingRepR;

            public int intervalForTimeL;
            public int intervalForTimeR;
            public int timeElapsedL;
            public int timeElapsedR;

        }

        private ControlVarsGameMode previous = new ControlVarsGameMode();
        private ControlVarsGameMode current = new ControlVarsGameMode();

        private float lDiffernce;
        private float rDiffernce;

        private float left;
        private float right;

        public bool isUpdatingUp = false;
        public bool isUpdatingRight = false;
        public bool isUpdatingLeft = false;

        public bool isDebugActive;

        [Range(0.0f, 1.0f)]
        public float leftPullDebug;

        [Range(0.0f, 1.0f)]
        public float rightPullDebug;

        [Range(0.0f, 1.0f)]
        public float pullBothDebug;

        private float previousPullBothDebug;
        #endregion

        #region STARTUP LOGIC
        //----------------------------------------------------//
        private void Awake()
        //----------------------------------------------------//
        {
            //Debug.Log("GameManager//Awake//");

            if (Instance != this)
                Destroy(this.gameObject);

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            maxProController = gameObject.AddComponent<MaxProController>();

            maxProController.DefaultConnectionHandler = new BluetoothLowEnergyHardwareInterfaceConnectionHandler();

            maxProController.OnStateChanged += HandleMaxProControllerStateChanged;
            maxProController.OnMaxProCommandReceived += HandleMaxProControllerMaxProCommandReceived;

#if UNITY_EDITOR
            InvokeRepeating("UpdateDebug", 0, 0.04f);
#endif

        } // END Awake
        #endregion

#if UNITY_EDITOR
        private void UpdateDebug()
        {
            if (GameManager.isGamePlaying == true && isDebugActive == true)
            {
                if (pullBothDebug != previousPullBothDebug)
                {
                    GameManager.Instance.leftPullDebug = pullBothDebug;
                    GameManager.Instance.rightPullDebug = pullBothDebug;
                }

                DebugUpdateInput(GameManager.Instance.leftPullDebug, GameManager.Instance.rightPullDebug);

                previousPullBothDebug = pullBothDebug;
            }
        }
#endif


        #region BUTTON PRESSES
        //----------------------------------------------------//
        public void Connect()
        //----------------------------------------------------//
        {
            Debug.Log("GameManager//OnConnectButtonPressed// ");

            maxProController.Initialize(
            () =>
            {
                //Debug.Log("GameManager//OnConnectButtonPressed// Initialize ");
            },
            (error) =>
            {
                Debug.Log("GameManager//OnConnectButtonPressed// error " + error);

            });


        } // END OnConnectButtonPressed

        //----------------------------------------------------//
        private void OnDisconnectButtonPressed()
        //----------------------------------------------------//
        {
            //Debug.Log("GameManager//HandleDisconnectButtonClick//");

            maxProController.Disconnect();

        } // END OnDisconnectButtonPressed

        //----------------------------------------------------//
        public void OnRecalibrtatePressed()
        //----------------------------------------------------//
        {
            /*_maxProController.SendAppCommand(new GameEventRequestAppCommand
            {
                IsGameModeOn = true,
            });*/

        } // END OnRecalibrtatePressed
        #endregion

        #region ON COMMAND RECIVED
        //----------------------------------------------------//
        private void HandleMaxProControllerMaxProCommandReceived(MaxProController sender, CommandType type, byte[] data)
        //----------------------------------------------------//
        {
            //Debug.Log("GameManager//HandleMaxProControllerMaxProCommandReceived// sender " + sender.name + " commandType " + type + " data " + data.Length);

            switch (type)
            {
                case CommandType.StatusRequest:
                    {
                        StatusRequestMaxProCommand command = new StatusRequestMaxProCommand();
                        command.Deserialize(data);
                        UpdateInput(type, command);

                        break;
                    }

                case CommandType.Event:
                    {
                        EventCommand command = new EventCommand();
                        command.Deserialize(data);
                        UpdateInput(type, command);

                        break;
                    }

                case CommandType.GameEventRequestUpdate:
                    {
                        GameEventRequestUpdateMaxProCommand command = new GameEventRequestUpdateMaxProCommand();
                        command.Deserialize(data);
                        UpdateInput(type, command);

                        break;
                    }
            }

        } // END HandleMaxProControllerMaxProCommandReceived

        //----------------------------------------------------//
        private void HandleMaxProControllerStateChanged(MaxProController sender)
        //----------------------------------------------------//
        {
            //Debug.Log("GameManager//HandleMaxProControllerStateChanged// " + maxProController.State.ToString());

            if (maxProController.State == MaxProControllerState.Connected)
            {
                isGameModeInputOn = true;

                maxProController.SendAppCommand(new GameEventRequestAppCommand
                {
                    IsGameModeOn = true,
                });
            }

            OnConnectionStateChanged?.Invoke(maxProController.State);

        } // END HandleMaxProControllerStateChanged

        //----------------------------------------------------//
        private void UpdateInput(CommandType type, object obj)
        //----------------------------------------------------//
        {
            //Debug.Log("SdkSample//UpdateInput// type " + type.ToString() + " obj " + obj.ToString());

            string json = JsonUtility.ToJson(obj, true).Replace("  ", "");

            //Debug.Log("GameManager//UpdateInput// json " + json);

            if (isGameModeInputOn)
            {
                gameInput = JsonUtility.FromJson<GameInput>(json);

                DetermineNobPosition();

                if (isGamePlaying == false)
                {
                    return;
                }

                DetermineCordPosition();

            }
            else
            {
                repInput = JsonUtility.FromJson<RepInput>(json);

                if (repInput != null)
                {
                    repComplete?.Invoke(repInput);
                }
            }


        } // END UpdateModeExclusiveCommandLabel

        //----------------------------------------------------//
        private void DebugUpdateInput(float left, float right)
        //----------------------------------------------------//
        {
            //Debug.Log("GameManager//DebugUpdateInput// left " + left);
            //Debug.Log("GameManager//DebugUpdateInput// right " + right);

            gameInput = new GameInput
            {
                LeftDistance = (int)(leftPullDebug * 1000),
                RightDistance = (int)(rightPullDebug * 1000),
                LeftKnobPosition = 10,
                RightKnobPosition = 10
            };

            //Debug.Log("GameManager//DebugUpdateInput// gameInput.LeftDistance " + gameInput.LeftDistance);
            //Debug.Log("GameManager//DebugUpdateInput// gameInput.RightDistance " + gameInput.RightDistance);

            DetermineNobPosition();

            if (isGamePlaying == false)
            {
                //Debug.Log("GameManager//DebugUpdateInput// isGamePlaying False return");

                return;
            }

            DetermineCordPosition();

        } // END UpdateModeExclusiveCommandLabel

        //----------------------------------------------------//
        private void DetermineNobPosition()
        //----------------------------------------------------//
        {
            //Debug.Log("GameManager//DetermineNobPosition//");

            currentLNob = gameInput.LeftKnobPosition;
            currentRNob = gameInput.RightKnobPosition;

            currentLNob = currentLNob / 10;
            currentRNob = currentRNob / 10;

            currentAvgNob = ((currentLNob + currentRNob) / 2);

            if (currentLNob != lastLNob ||
                currentRNob != lastRNob)
            {
                float avrgWeight = (float)(currentLNob + currentRNob) / 2;

                //Debug.Log("GameManager//UpdateInput// avrgWeight " + avrgWeight);

                avrgWeight = avrgWeight / 25;

                //Debug.Log("GameManager//UpdateInput// percentage " + avrgWeight);

                if (avrgWeight < 0.25f)
                {
                    difficulty = Difficulty.Easy;
                }
                else if (avrgWeight < 0.5f)
                {
                    difficulty = Difficulty.Middle;
                }
                else if (avrgWeight < 0.75f)
                {
                    difficulty = Difficulty.Hard;
                }
                else
                {
                    difficulty = Difficulty.Max;
                }

                //Debug.Log("GameManager//UpdateInput// difficulty " + difficulty.ToString());

                OnNobChanged?.Invoke(currentLNob, currentRNob);
            }

            lastLNob = currentLNob;
            lastRNob = currentRNob;
            lastAvgNob = currentAvgNob;
        }

        //----------------------------------------------------//
        private void DetermineCordPosition()
        //----------------------------------------------------//
        {
            //Debug.Log("DetermineCordPosition");
            current.LTime = previous.LTime;
            current.LDist = gameInput.LeftDistance;
            current.LVel = previous.LVel;
            current.LAcc = previous.LAcc;
            current.LNum = previous.LNum;
            current.RDist = gameInput.RightDistance;
            current.RTime = previous.RTime;
            current.RVel = previous.RVel;
            current.RAcc = previous.RAcc;
            current.RNum = previous.RNum;

            // L Dist decreasing 
            if (current.LDist < this.previous.LDist && this.previous.MakingRepL)
            {
                HandleLDecrease();
            }
            // L Dist increasing 
            else if (current.LDist > this.previous.LDist && this.previous.MakingRepL)
            {
                HandleLIncrease();
            }
            // L Dist started moving up 
            else if (current.LDist > this.previous.LDist && !this.previous.MakingRepL)
            {
                LRepStarted();
            }

            // R Dist decreasing 
            if (current.RDist < this.previous.RDist && this.previous.MakingRepR)
            {
                HandleRDecrease();
            }
            // R Dist increasing 
            else if (current.RDist > this.previous.RDist && this.previous.MakingRepR)
            {
                HandleRIncrease();
            }
            // R Dist started moving up 
            else if (current.RDist > this.previous.RDist && !this.previous.MakingRepR)
            {
                RRepStarted();
            }

            this.previous.RDist = current.RDist;

            if (current.RDist > 0)
            {
                //current.RDist = current.RDist / 1000;
            }

            this.previous.LDist = current.LDist;

            if (current.LDist > 0)
            {
                //current.LDist = current.LDist / 1000;
            }

            //Debug.Log(JsonUtility.ToJson(current));

            lDiffernce = GameManager.leftUpPosition - GameManager.leftDownPosition;
            rDiffernce = GameManager.rightUpPosition - GameManager.rightDownPosition;

            left = GameManager.gameInput.LeftDistance - GameManager.leftDownPosition;
            right = GameManager.gameInput.RightDistance - GameManager.rightDownPosition;

            if (lDiffernce <= 0 || rDiffernce <= 0 || left <= 0 || right <= 0)
            {
                //SetToDefault();
            }

            lDiffernce = Mathf.Clamp(lDiffernce, 0.001f, 3000);
            rDiffernce = Mathf.Clamp(rDiffernce, 0.001f, 3000);

            left = Mathf.Clamp(left, 0.001f, 3000);
            right = Mathf.Clamp(right, 0.001f, 3000);

            leftPull = Mathf.Clamp(left / lDiffernce, 0.001f, 1);
            rightPull = Mathf.Clamp(right / rDiffernce, 0.001f, 1);

            avrgPull = (leftPull + rightPull) / 2;

            OnInputUpdated?.Invoke(leftPull, rightPull);

            if (isBothUp == false && avrgPull > 0.65)
            {
                //Debug.Log("GameManager//DetermineCordPosition// up start ");

                isLeftUp = true;
                isRightUp = true;

                OnRepUp?.Invoke(InputDirection.Up);
            }
            else if (isBothUp == true && avrgPull < 0.3)
            {
                //Debug.Log("GameManager//DetermineCordPosition// end up");

                isLeftUp = false;
                isRightUp = false;

                OnRepDown?.Invoke(InputDirection.Up);
            }
            else if (isLeftUp == false && leftPull > 0.8)
            {
                //Debug.Log("GameManager//DetermineCordPosition// start left");

                isLeftUp = true;

                OnRepUp?.Invoke(InputDirection.Left);
            }
            else if (isLeftUp == true && leftPull < 0.3)
            {
                //Debug.Log("GameManager//DetermineCordPosition// end left");

                isLeftUp = false;

                OnRepDown?.Invoke(InputDirection.Left);
            }
            else if (isRightUp == false && rightPull > 0.8)
            {
                //Debug.Log("ShipController//Right// start right");

                isRightUp = true;

                OnRepUp?.Invoke(InputDirection.Right);
            }
            else if (isRightUp == true && rightPull < 0.3)
            {
                isRightUp = false;

                OnRepDown?.Invoke(InputDirection.Right);

            }

        } // END DetermineCordPosition

        //----------------------------------------------------//
        private void LRepStarted()
        //----------------------------------------------------//
        {
            this.previous.MakingRepL = true;

            this.previous.timeElapsedL = 0;

            leftStart = current.LDist;

            //Debug.Log("STARING L REP " + leftStart);

            leftEnd = 0;

            GameManager.leftDownPosition = leftStart;

        }

        //----------------------------------------------------//
        private void HandleLIncrease()
        //----------------------------------------------------//
        {
            //Debug.Log("DURING L REP");

            //CALC TIME IN MILISECONDS
            this.previous.timeElapsedL += 40;
            int timeElapsedInMiliSeconds = this.previous.timeElapsedL; //ms

            //GET DIST IN MILIMETERS
            float finalDistRepL = current.LDist;

            //SET TIME MILI
            this.previous.LTime = timeElapsedInMiliSeconds;
            current.LTime = this.previous.LTime;

            //CALC VEL d/t = milimeters/miliseconds*1000 to be equal to normal mode
            this.previous.LVel = (finalDistRepL / this.previous.LTime) * 1000;
            current.LVel = this.previous.LVel; // mm/ms

            //CALC AC (Vf - Vi) / t
            this.previous.LAcc = (this.previous.LVel - 0) / this.previous.LTime;
            current.LAcc = this.previous.LAcc;
        }

        //----------------------------------------------------//
        private void HandleLDecrease()
        //----------------------------------------------------//
        {
            leftEnd = current.LDist;

            lDiffernce = leftEnd - leftStart;

            // Debug.Log("L Peak " + current.LDist);
            // Debug.Log("leftStart " + leftStart);
            // Debug.Log("lDiffernce " + lDiffernce);

            if (lDiffernce > 300)
            {
                GameManager.leftUpPosition = leftEnd;
            }

            this.previous.MakingRepL = false;
            //CLCULATE REP MILISECONDS
            this.previous.timeElapsedL += 40;
            int timeElapsedInMiliSeconds = this.previous.timeElapsedL; //ms

            //GET DIST IN MILIMETERS
            int finalDistRepL = gameInput.LeftDistance;

            //Send time to data
            this.previous.LTime = timeElapsedInMiliSeconds;
            current.LTime = this.previous.LTime;

            //CALC VEL d/t = milimeters/miliseconds*1000 to be equal to normal mode
            this.previous.LVel = (finalDistRepL / this.previous.LTime) * 1000;
            current.LVel = this.previous.LVel; // mm/ms

            //CALC AC (Vf - Vi) / t
            this.previous.LAcc = (this.previous.LVel - 0) / this.previous.LTime;
            current.LAcc = this.previous.LAcc;

            //Set Reps
            this.previous.LNum++;
            current.LNum = this.previous.LNum;
        }

        //----------------------------------------------------//
        private void RRepStarted()
        //----------------------------------------------------//
        {
            this.previous.MakingRepR = true;

            this.previous.timeElapsedR = 0;

            rightStart = current.RDist;

            // Debug.Log("STARING R REP " + rightStart);

            rightEnd = 0;

            GameManager.rightDownPosition = rightStart;
        }

        //----------------------------------------------------//
        private void HandleRIncrease()
        //----------------------------------------------------//
        {
            //Debug.Log("HandleRIncrease");

            //CALC TIME IN MILISECONDS
            this.previous.timeElapsedR += 40;

            int timeElapsedInMiliSeconds = this.previous.timeElapsedR; //ms

            //GET DIST IN MILIMETERS
            float finalDistRepR = current.RDist;

            //SET TIME MILI
            this.previous.RTime = timeElapsedInMiliSeconds;

            current.RTime = this.previous.RTime;

            //CALC VEL d/t = milimeters/miliseconds*1000 to be equal to normal mode
            this.previous.RVel = (finalDistRepR / this.previous.RTime) * 1000;
            current.RVel = this.previous.RVel; // mm/ms

            //CALC AC (Vf - Vi) / t
            this.previous.RAcc = (this.previous.RVel - 0) / this.previous.RTime;
            current.RAcc = this.previous.RAcc;
        }

        //----------------------------------------------------//
        private void HandleRDecrease()
        //----------------------------------------------------//
        {
            //Debug.Log("HandleRDecrease");

            rightEnd = current.RDist;

            rDiffernce = rightEnd - rightStart;

            // Debug.Log("R Peak " + current.RDist);
            //  Debug.Log("rightStart " + rightStart);
            //  Debug.Log("rDiffernce " + rDiffernce);

            if (rDiffernce > 300)
            {
                GameManager.rightUpPosition = rightEnd;
            }

            this.previous.MakingRepR = false;

            //CLCULATE REP MILISECONDS
            this.previous.timeElapsedR += 40;
            int timeElapsedInMiliSeconds = this.previous.timeElapsedR;

            //GET DIST IN MILIMETERS
            float finalDistRepR = current.RDist;

            //Send time to data
            this.previous.RTime = timeElapsedInMiliSeconds;
            current.RTime = this.previous.RTime;

            //Calculate velocity
            this.previous.RVel = (finalDistRepR / this.previous.RTime) * 1000;
            current.RVel = this.previous.RVel; // mm/ms

            //Calculate acceleration
            this.previous.RAcc = (this.previous.RVel - 0) / this.previous.RTime;
            current.RAcc = this.previous.RAcc; // (Vf - Vi) / t = a

            //Set Reps
            this.previous.RNum++;
            current.RNum = this.previous.RNum;
        }
        #endregion


        //----------------------------------------------------//
        public static void EndGame(bool isRestart)
        //----------------------------------------------------//
        {
            GameManager.isGamePlaying = false;

            OnGameEnded?.Invoke(isRestart);

        } // END EndGame


        #region ON DESTROY
        //----------------------------------------------------//
        private void OnDestroy()
        //----------------------------------------------------//
        {
            isGamePlaying = false;

            if (maxProController != null)
            {
                maxProController.OnStateChanged -= HandleMaxProControllerStateChanged;
                maxProController.OnMaxProCommandReceived -= HandleMaxProControllerMaxProCommandReceived;
            }

            //CountdownTimer.OnComplete -= OnTimerComplete;

            OnDestroyed?.Invoke();

        } // END OnDestroy
        #endregion

    } // END Class