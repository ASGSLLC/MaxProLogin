using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

namespace maxprofitness.login
{
    public class WorkCalculatorController : MonoBehaviour
    {
        #region VARIABLES


        public delegate void CanoeMovementUpdateHandler(float newMovement, ActionSide side);
        public event CanoeMovementUpdateHandler OnCanoeMovementUpdated;
        public event Action OnEndMovement;

        [Tooltip("The offset will set the range based on the calibrated rest position")]
        [SerializeField] private float _restPositionOffset;

        private readonly List<Repetition> _repetitions = new List<Repetition>();

        private float _lastRightDistance;
        private float _lastLeftDistance;
        private float _currentRightDistance;
        private float _currentLeftDistance;
        private float _repInitialDistance;
        private float _repFinalDistance;
        private float _currentRightResistance = 10;
        private float _currentLeftResistance = 10;

        private float _currentRightVelocity;
        private float _maxRightVelocity;
        private int _rightCountForEndingMovement;
        private float _currentLeftVelocity;
        private float _maxLeftVelocity;
        private int _leftCountForEndingMovement;
        private const int CountsToConfirmEndingMovement = 2;

        private bool _isRightSideReset = true;
        private bool _isLeftSideReset = true;
        private bool _canComputeInput;

        [SerializeField] private PowerCalculator _rightPowerCalculator;
        [SerializeField] private PowerCalculator _leftPowerCalculator;

        public PowerCalculator LeftPowerCalculator => _leftPowerCalculator;
        public PowerCalculator RightPowerCalculator => _rightPowerCalculator;

#if FIT_FIGHTER || CANOE
    [Header("Listening To")]
    [SerializeField] private BoolEventChannelSO _finishedIntroductionChannel;
    [Space(10)]

    private HandleState _leftHandleState;
    private HandleState _rightHandleState;

    public List<int> AverageWorkByRepetition => _repetitions.Select(rep => rep.Work).ToList().CondenseList();
    public List<int> PowerList => _repetitions.Select(rep => rep.Power).ToList().CondenseList();
#endif
        public int CurrentRightKnob => (int)_currentRightResistance / 10;
        public int CurrentLeftKnob => (int)_currentLeftResistance / 10;

        public List<Repetition> Repetitions => _repetitions;
        private float MinResetThreshold => ((GameManager.leftDownPosition + GameManager.rightDownPosition) / 2) + _restPositionOffset;
        private float MaxResetThreshold => ((GameManager.leftDownPosition + GameManager.rightDownPosition) / 2) - _restPositionOffset;


        #endregion


        #region MONOBEHAVIOURS


        //--------------------//
        private void OnEnable()
        //--------------------//
        {
            Initialize();

            GameManager.OnInputUpdated += HandleInputReceived;
#if CANOE || FIT_FIGHTER
        AppEvents.RepetitionExecutedEvent += HandleRepetitionExecuted;

        if (_finishedIntroductionChannel != null)
        {
            _finishedIntroductionChannel.OnEventRaised += (value) => _canComputeInput = value;
            _finishedIntroductionChannel.RaiseEvent(true);
            _canComputeInput = true;
        }
#endif
        } // END OnEnable


        //----------------------//
        private void OnDisable()
        //----------------------//
        {
            GameManager.OnInputUpdated -= HandleInputReceived;
#if CANOE || FIT_FIGHTER
        AppEvents.RepetitionExecutedEvent -= HandleRepetitionExecuted;

        if (_finishedIntroductionChannel != null)
        {
            _finishedIntroductionChannel.OnEventRaised -= (value) => _canComputeInput = value;
        }
#endif
        } // END OnDisable


        //----------------------//
        private void OnDestroy()
        //----------------------//
        {
            GameManager.OnInputUpdated -= HandleInputReceived;

        } // END OnDestroy


        #endregion


        #region INITIALIZE


        //---------------------//
        public void Initialize()
        //---------------------//
        {
            _rightPowerCalculator.Initialize();
            _leftPowerCalculator.Initialize();

        } // END Initialize


        #endregion


        #region INPUT


        //------------------------------------------------------//
        private void HandleInputReceived(float left, float right)
        //------------------------------------------------------//
        {
            //Debug.Log("WorkCalculatorController//HandleInputReceived//");

            UpdateHandleState();
            OnInputReceived();

        } // END HandleInputReceived


        //---------------------------//
        public void OnInputReceived()
        //---------------------------//
        {
            UpdateVelocities();
            UpdateWorkPeakAndAverage();

        } // END OnInputReceived


        #endregion


        #region GET WORK


        //-------------------------//
        public int GetAverageWork()
        //------------------------//
        {
            return (int)_leftPowerCalculator.GetAverageWork(_currentLeftResistance) + (int)_rightPowerCalculator.GetAverageWork(_currentRightResistance);

        } // END GetAverageWork


        //-----------------------//
        public int GetTotalWork()
        //----------------------//
        {
            return _leftPowerCalculator.TotalWork + _rightPowerCalculator.TotalWork;

        } // END GetTotalWork


        //----------------------//
        public int GetWorkPeak()
        //---------------------//
        {
            return _leftPowerCalculator.WorkPeak >= _rightPowerCalculator.WorkPeak ? _leftPowerCalculator.WorkPeak : _rightPowerCalculator.WorkPeak;

        } // END GetWorkPeak


        //------------------------------------------//
        public float GetAverageWork(ActionSide side)
        //-----------------------------------------//
        {
#if !CANOE || !FIT_FIGHTER
            float averageWork = 0;
#elif CANOE || FIT_FIGHTER
        float averageWork;

        switch (side)
        {
            case ActionSide.LEFT:
                averageWork = _leftPowerCalculator.GetAverageWork(_currentLeftResistance);
                break;
            case ActionSide.RIGHT:
                averageWork = _rightPowerCalculator.GetAverageWork(_currentRightResistance);
                break;
            case ActionSide.BOTH:
            default:
                averageWork = _leftPowerCalculator.GetAverageWork(_currentLeftResistance);
                averageWork += _rightPowerCalculator.GetAverageWork(_currentRightResistance);
                break;
        }
#endif
            return averageWork;

        } // END GetAverageWork


        //-----------------------------------//
        public float GetWork(ActionSide side)
        //----------------------------------//
        {
#if !CANOE || !FIT_FIGHTER
            float work = 0;
#elif CANOE || FIT_FIGHTER

        float work;

        switch (side)
        {
            case ActionSide.LEFT:
                work = _leftPowerCalculator.GetWork(_currentLeftResistance);
                break;

            case ActionSide.RIGHT:
                work = _rightPowerCalculator.GetWork(_currentRightResistance);
                break;

            case ActionSide.BOTH:
                work = _leftPowerCalculator.GetWork(_currentLeftResistance);
                work += _rightPowerCalculator.GetWork(_currentRightResistance);
                break;

            default:
                work = _leftPowerCalculator.GetWork(_currentLeftResistance);
                work += _rightPowerCalculator.GetWork(_currentRightResistance);
                break;
        }
#endif
            return work;

        } // END GetWork


        #endregion


        #region GET CALORIES BURNED


        //----------------------------//
        public int GetCaloriesBurned()
        //---------------------------//
        {
            float caloriesBurned = 0;

            Gender gender = Gender.Male;

            if (UserDataManager.loadedData != null)
            {
                gender = UserDataManager.loadedData.gender;
            }

            // float userWeight = _userInfo.Weight;
            //float userHeight = _userInfo.Height;
            // int userAge = _userInfo.Age;

            float userWeight = 70;
            float userHeight = 171;
            int userAge = 25;

            float exerciseMet = 4;

            foreach (Repetition repetition in _repetitions)
            {
                float power = repetition.Power;
                float repTime = repetition.TimeInSeconds;

                float powerMetFactor = power switch
                {
                    float n when (n <= 74) => -1,
                    float n when (n >= 75 && n <= 149) => 0,
                    float n when (n >= 150 && n <= 299) => 3,
                    _ => 4,
                };

                float bmrDays = gender switch
                {
                    Gender.Female => (10 * userWeight) + (6.25f * userHeight) - (5 * userAge) - 161,
                    _ => (10 * userWeight) + (6.25f * userHeight) - (5 * userAge) + 5,
                };

                float calcOriginalMet = exerciseMet + powerMetFactor;
                float md = bmrDays / 1440;
                float kl = md / 5;
                float lm = (kl / userWeight) * 1000;
                float metFixedMetcbi = calcOriginalMet * (3.5f / lm);
                float calsByMinute = (0.0175f * metFixedMetcbi) * userWeight;

                caloriesBurned += calsByMinute * (repTime / 60);
            }

            Debug.Log($"[{nameof(WorkCalculatorController)}] - Calories burned: {caloriesBurned}");

            return Mathf.RoundToInt(caloriesBurned);

        } // END GetCaloriesBurned


        #endregion


        #region GET INPUT VELOCITY


        //--------------------------------------------//
        public float GetInputVelocity(ActionSide side)
        //-------------------------------------------//
        {
#if CANOE || FIT_FIGHTER
        switch (side)
        {
            case ActionSide.LEFT:
                {
                    return _leftPowerCalculator.GetAverageVelocity();
                }

            case ActionSide.RIGHT:
                {
                    return _rightPowerCalculator.GetAverageVelocity();
                }
        }

        float inputVelocity = _leftPowerCalculator.GetAverageVelocity();
        inputVelocity += _rightPowerCalculator.GetAverageVelocity();
#elif !CANOE || !FIT_FIGHTER
            float inputVelocity = 0;
#endif
            return inputVelocity / 2f;

        } // END GetInputVelocity


        #endregion


        #region CLEAR DATA


        //------------------------------------//
        public void ClearData(ActionSide side)
        //------------------------------------//
        {
#if CANOE || FIT_FIGHTER
        switch (side)
        {
            case ActionSide.LEFT:
                _leftPowerCalculator.ClearData();
                break;
            case ActionSide.RIGHT:
                _rightPowerCalculator.ClearData();
                break;
            case ActionSide.BOTH:
            default:
                _leftPowerCalculator.ClearData();
                _rightPowerCalculator.ClearData();
                break;
        }
#endif
        } // END ClearData


        #endregion


        #region CHECK FOR ENDING MOVEMENT


        //----------------------------------//
        public void CheckForEndingMovement()
        //----------------------------------//
        {
#if CANOE || FIT_FIGHTER
        _currentLeftVelocity = _leftPowerCalculator.GetCurrentVelocity();
        _currentRightVelocity = _rightPowerCalculator.GetCurrentVelocity();
#endif
            if (_currentLeftVelocity >= _maxLeftVelocity)
            {
                _maxLeftVelocity = _currentLeftVelocity;
                _leftCountForEndingMovement = 0;
            }
            else
            {
                _leftCountForEndingMovement++;
                _maxLeftVelocity = _currentLeftVelocity;

                if (_leftCountForEndingMovement >= CountsToConfirmEndingMovement || _currentLeftVelocity < 0.1f)
                {
                    _leftCountForEndingMovement = 0;
                    _maxLeftVelocity = 0;
                    OnEndMovement?.Invoke();
                }
            }

            if (_currentRightVelocity >= _maxRightVelocity)
            {
                _maxRightVelocity = _currentRightVelocity;
                _rightCountForEndingMovement = 0;
            }
            else
            {
                _rightCountForEndingMovement++;
                _maxRightVelocity = _currentRightVelocity;

                if (_rightCountForEndingMovement < CountsToConfirmEndingMovement && !(_currentRightVelocity < 0.1f))
                {
                    return;
                }

                _rightCountForEndingMovement = 0;
                _maxRightVelocity = 0;
                OnEndMovement?.Invoke();
            }

        } // END CheckForEndingMovement


        #endregion


        #region UPDATE HANDLE STATE


        //------------------------------//
        private void UpdateHandleState()
        //------------------------------//
        {
            if (_canComputeInput)
            {
                CheckHandleDistanceAndReset();
            }
#if CANOE || FIT_FIGHTER
        PowerCalculator powerCalculator;

        if (_currentLeftDistance > _currentRightDistance)
        {
            powerCalculator = _leftPowerCalculator;
            CheckMovementState(powerCalculator, _lastLeftDistance, GameManager.gameInput.LeftDistance);
        }
        else
        {
            powerCalculator = _rightPowerCalculator;
            CheckMovementState(powerCalculator, _lastRightDistance, GameManager.gameInput.RightDistance);
        }

        _leftHandleState = MaxProHandleState.GetHandleState(_lastLeftDistance, GameManager.gameInput.LeftDistance);
        _rightHandleState = MaxProHandleState.GetHandleState(_lastRightDistance, GameManager.gameInput.RightDistance);

    }
#endif
#if CANOE || FIT_FIGHTER
    private void CheckMovementState(PowerCalculator powerCalculator, float oldDistance, float newDistance)
    {
        if (oldDistance < newDistance)
        {
            return;
        }

        powerCalculator.ClearData();
#endif
        } // END UpdateHandleState


        #endregion


        #region UPDATE VELOCITIES


        //-----------------------------//
        private void UpdateVelocities()
        //-----------------------------//
        {
            //Debug.Log("WorkCalculatorController//UpdateVelocities//");

            _currentRightResistance = GameManager.gameInput.RightKnobPosition;
            _currentLeftResistance = GameManager.gameInput.LeftKnobPosition;
            _currentRightDistance = GameManager.gameInput.RightDistance;
            _currentLeftDistance = GameManager.gameInput.LeftDistance;
#if CANOE || FIT_FIGHTER
        _rightPowerCalculator.GenerateVelocity(_lastRightDistance / 1000, _currentRightDistance / 1000);
        _leftPowerCalculator.GenerateVelocity(_lastLeftDistance / 1000, _currentLeftDistance / 1000);
#endif
            _lastRightDistance = GameManager.gameInput.RightDistance;
            _lastLeftDistance = GameManager.gameInput.LeftDistance;

        } // END UpdateVelocities


        #endregion


        #region UPDATE WORK PEAK AND AVERAGE


        //-------------------------------------//
        private void UpdateWorkPeakAndAverage()
        //-------------------------------------//
        {
#if CANOE || FIT_FIGHTER
        _leftPowerCalculator.UpdateTotalWorkAndWorkPeak(_currentLeftResistance);
        _rightPowerCalculator.UpdateTotalWorkAndWorkPeak(_currentRightResistance);
#endif
        } // END UpdateWorkPeakAndAverage


        #endregion


        #region HANDLE REPETITION EXECUTED


        //---------------------------------------------------//
        private void HandleRepetitionExecuted(Repetition rep)
        //---------------------------------------------------//
        {
            _repetitions.Add(rep);

        } // END HandleRepetitionExecuted


        #endregion


        #region CHECK HANDLE DISTANCE AND RESET


        //----------------------------------------//
        private void CheckHandleDistanceAndReset()
        //----------------------------------------//
        {
#if CANOE || FIT_FIGHTER
        if (_leftHandleState != HandleState.Releasing && _rightHandleState != HandleState.Releasing)
        {
            return;
        }
#endif
            if (_currentLeftDistance <= MinResetThreshold)
            {
                _isLeftSideReset = true;
            }

            if (_currentRightDistance <= MinResetThreshold)
            {
                _isRightSideReset = true;
            }

        } // END CheckHandleDistanceAndReset


        #endregion


    } // END WorkCalculatorController.cs
}