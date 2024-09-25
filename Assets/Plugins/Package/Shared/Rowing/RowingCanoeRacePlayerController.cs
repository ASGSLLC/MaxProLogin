using UnityEngine;

namespace maxprofitness.shared
{
    public class RowingCanoeRacePlayerController : RowingCanoePlayerControllerBase
    {
        #region VARIABLES


        [SerializeField] private AnimationCurve _speedMultiplierByWeight;
        private int _knobLevelAverage;


        #endregion


        #region MONOBEHAVIOURS


        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        //-------------------//
        private void Update()
        //-------------------//
        {
            MovePlayer();

        } // END Update


        #endregion


        #region SET PLAYER SPEED AFTER FINISH


        //-------------------------------------//
        public void SetPlayerSpeedAfterFinish()
        //-------------------------------------//
        {
            _constantForce.force = -Vector3.forward * PlayerSpeedAfterFinish;
            _constantForce.enabled = true;

        } // END SetPlayerSpeedAfterFinish


        #endregion


        #region HANDLE MOVEMENT UPDATED


        //-------------------------------------------------------------------------//
        public override void HandleMovementUpdated(float newSpeed, ActionSide side)
        //-------------------------------------------------------------------------//
        {
            if (!CanMove)
            {
                return;
            }

            _speedMultiplier = _speedMultiplierByWeight.Evaluate(_knobLevelAverage);

            ProcessNewMovement(Mathf.Abs(newSpeed) * _speedMultiplier, side);

        } // END HandleMovementUpdated


        #endregion


        #region PROCESS NEW MOVEMENT

        //-------------------------------------------------------------------------//
        protected override void ProcessNewMovement(float newSpeed, ActionSide side)
        //-------------------------------------------------------------------------//
        {
            float forwardSpeed = newSpeed * Time.deltaTime;
            Vector3 playerMovementVector = new Vector3(0, 0, -forwardSpeed);

            CanoeEvents.PlayerMovementDirectionEvent?.Invoke(playerMovementVector);
            UpdateMovementMetrics(Speed);

        } // END ProcessNewMovement

#endregion

    } // END RowingCanoeRacePlayerController.cs

} // END Namespace
