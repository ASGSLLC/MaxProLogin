using System.Collections;
using UnityEngine;

namespace maxprofitness.shared
{
    public class OpponentController : MonoBehaviour
    {
        #region VARIABLES


        private const float LerpDuration = 2.0f;

        [Header("Opponent")]
        [SerializeField] private Transform _opponentTransform;
        [SerializeField] private Animator _opponentCharacterAnimator;
        [SerializeField] private Animator _opponentCanoeAnimator;
        [SerializeField] private float _defaultSpeed = 10;

        private RowingTrackManager trackManager;
        private static readonly int IsRowing = Animator.StringToHash("IsRowing");
        private static readonly int Speed = Animator.StringToHash("Speed");

        private Coroutine _adjustSpeedCoroutine;
        private float _speed;

        public bool canMove;
        public float DefaultSpeed => _defaultSpeed;
        public float ScaledDistanceCovered { get; set; }
        public float DistanceCovered { get; private set; }
        
        
        #endregion


        #region MONOBEHAVIOURS


        //------------------//
        public void Update()
        //------------------//
        {
            MoveOpponent();

        } // END Update


        //------------------------------------------//
        private void OnTriggerEnter(Collider _other)
        //------------------------------------------//
        {
            if (_other.transform.gameObject.GetComponentInChildren<CanoeController>() || _other.transform.gameObject.GetComponent<CanoeController>() || _other.transform.gameObject.GetComponentInParent<CanoeController>() != null)
            {
                Debug.Log("Player finished: " + _other.transform.gameObject.name);


            }

        } // END OnTriggerEnter
        
        
        #endregion


        #region ON START


        //--------------------//
        public void OnStart()
        //--------------------//
        {
            Initialize();

        } // END OnStart
        
        
        #endregion


        #region SET DEFAULT SPEED


        //---------------------------------------//
        public void SetDefaultSpeed(float speed)
        //---------------------------------------//
        {
            _defaultSpeed = speed;

        } // END SetDefaultSpeed


        #endregion


        #region SET OPPONENT SPEED TO DEFAULT


        //-------------------------------------//
        public void SetOpponentSpeedToDefault()
        //-------------------------------------//
        {
            _speed = _defaultSpeed;

        } // END SetOpponentSpeedToDefault


        #endregion


        #region TOGGLE MOVEMENT


        //-------------------------------------//
        public void ToggleMovement(bool state)
        //-------------------------------------//
        {
            canMove = state;

            _opponentCharacterAnimator.SetBool(IsRowing, canMove);
            _opponentCanoeAnimator.SetBool(IsRowing, canMove);

            SetOpponentAnimatorSpeed(_speed);

        } // END ToggleMovement


        #endregion


        #region UPDATE SPEED


        //----------------------------------------//
        public void UpdateSpeed(float targetSpeed)
        //----------------------------------------//
        {
            if (_adjustSpeedCoroutine != null)
            {
                StopCoroutine(_adjustSpeedCoroutine);
            }

            _adjustSpeedCoroutine = StartCoroutine(AdjustSpeedCoroutine(targetSpeed));

        } // END UpdateSpeed


        #endregion


        #region INITIALIZE


        //-----------------------//
        private void Initialize()
        //-----------------------//
        {
            if (_opponentTransform == null)
            {
                _opponentTransform = GetComponentInChildren<Transform>();
            }

        } // END Initialize


        #endregion


        #region MOVE OPPONENT


        //-------------------------//
        private void MoveOpponent()
        //-------------------------//
        {
            if (!canMove)
            {
                return;
            }

            float forwardSpeed = _speed * Time.deltaTime;
            DistanceCovered += Mathf.Abs(forwardSpeed);

            _opponentTransform.Translate(new Vector3(0, 0, forwardSpeed));

        } // END MoveOpponent


        #endregion


        #region SET OPPONENT ANIMATOR SPEED


        //------------------------------------------------//
        private void SetOpponentAnimatorSpeed(float speed)
        //-----------------------------------------------//
        {
            _opponentCharacterAnimator.SetFloat(Speed, speed);
            _opponentCanoeAnimator.SetFloat(Speed, speed);

        } // END SetOpponentAnimatorSpeed


        #endregion


        #region ADJUST SPEED COROUTINE


        //---------------------------------------------------------//
        private IEnumerator AdjustSpeedCoroutine(float targetSpeed)
        //---------------------------------------------------------//
        {
            float timeElapsed = 0;
            float currentSpeed = _speed;

            while (timeElapsed < LerpDuration)
            {
                _speed = Mathf.Lerp(currentSpeed, targetSpeed, timeElapsed / LerpDuration);
                timeElapsed += Time.deltaTime;

                yield return null;
            }

            _speed = targetSpeed;

            SetOpponentAnimatorSpeed(_speed);

        } // END AdjustSpeedCoroutine
        
        
        #endregion


    } // END OpponentController.cs

} // END Namespace
