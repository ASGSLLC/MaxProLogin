//using MaxProFitness.Shared.Inputs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using maxprofitness.login;

namespace maxprofitness.shared
{
   public abstract class RowingCanoePlayerControllerBase : MonoBehaviour
   {
        #region VARIABLES


        [Header("References")]
        [SerializeField] private Transform _playerTransform;
        [SerializeField] protected Rigidbody _rigidbody;
        [SerializeField] protected ConstantForce _constantForce;

        [Header("Speed")]
        [SerializeField] private float _playerSpeedAfterFinish = 3f;

        protected float _speedMultiplier;
        public List<float> _metricsAverageSpeedEntries = new List<float>();

        public float MetricsAverageSpeed => _metricsAverageSpeedEntries.Average();
        public float ScaledDistanceCovered { get; set; }
        public float Speed => Mathf.Abs(_rigidbody.velocity.z);
        public bool CanMove { get; private set; }
        public float MetricsMaxSpeed { get; private set; }
        public float DistanceCovered { get; private set; }
        protected float PlayerSpeedAfterFinish => _playerSpeedAfterFinish;


        #endregion


        #region MONOBEHAVIOURS


        //------------------//
        private void Start()
        //-----------------//
        {
            Initialize();

        } // END Start


        #endregion


        #region ON UPDATE


        //----------------------------//
        public virtual void OnUpdate()
        //----------------------------//
        {
            MovePlayer();

        } // END OnUpdate
        
        
        #endregion


        #region TOGGLE MOVEMENT


        //--------------------------------------------//
        public virtual void ToggleMovement(bool state)
        //--------------------------------------------//
        {
            CanMove = state;
            CanoeEvents.CanoeAnimationSpeedEvent?.Invoke(state ? 1 : 0);

        } // END ToggleMovement


        #endregion


        #region HANDLE MOVEMENT UPDATED


        //----------------------------------------//
        public virtual void HandleMovementUpdated(float newSpeed, ActionSide side)
        //----------------------------------------//
        {
            if (!CanMove)
            {
                return;
            }

            newSpeed = Mathf.Abs(newSpeed * _speedMultiplier);
            ProcessNewMovement(newSpeed, side);

        } // END HandleMovementUpdated

        
#endregion


        #region INITALIZE


        //---------------------------------//
        protected virtual void Initialize()
        //--------------------------------//
        {
            StartCoroutine(SpeedAverageCoroutine());

        } // END Initialize


        #endregion


        #region MOVE PLAYER


        //----------------------------------//
        protected virtual void MovePlayer()
        //----------------------------------//
        {
            if (!CanMove)
            {
                return;
            }

            DistanceCovered = Mathf.Abs(_playerTransform.position.z);

            CanoeEvents.CanoeAnimationSpeedUpdateEvent?.Invoke(Speed);

        } // END MovePlayer


        #endregion


        #region PROCESS NEW MOVEMENT

#if MAXPRO_LOGIN
        //------------------------------------------------------------------------//
        protected virtual void ProcessNewMovement(float newSpeed, ActionSide side)
        //-----------------------------------------------------------------------//
        {
            UpdateMovementMetrics(Speed);

        } // END ProcessNewMovement
#endif


#endregion


        #region UPDATE MOVEMENT METRICS


        //--------------------------------------------------------//
        protected virtual void UpdateMovementMetrics(float speed)
        //-------------------------------------------------------//
        {
            if (speed > MetricsMaxSpeed)
            {
                MetricsMaxSpeed = speed;
            }
        } // END UpdateMovementMetrics


        #endregion


        #region SPEED AVERAGE COROUTINE


        //------------------------------------------------//
        protected virtual IEnumerator SpeedAverageCoroutine()
        //------------------------------------------------//
        {
            //Debug.Log("RowingCanoePlayerControllerBase.cs // SpeedAverageCoroutine() // Our speed is: " + Speed);
            if (Speed != 0)
            {
                _metricsAverageSpeedEntries.Add(Speed);
            }

            yield return new WaitForSeconds(1f);

            StartCoroutine(SpeedAverageCoroutine());

        } // END SpeedAverageCoroutine


        #endregion

    } // END RowingCanoePlayerControllerBase.cs

} // END Namespace