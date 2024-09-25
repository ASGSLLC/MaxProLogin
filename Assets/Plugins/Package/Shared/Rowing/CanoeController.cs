using static GameManager;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace maxprofitness.shared
{
    public class CanoeController : MonoBehaviour
    {
        #region VARIABLES


        [SerializeField] private bool isDebug;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Animator canoeRowerAnim;
        [SerializeField] private Animator canoeAnim;
        [SerializeField] private float speed;
#if MAXPRO_LOGIN
        private GameManager gameManager;
#endif
        private RowingCanoeRacePlayerController rowingCanoeRacePlayerController;
        private bool isRepUp;
        private static readonly int Speed = Animator.StringToHash("Speed");

        [SerializeField] private float lPullThreshold;
        [SerializeField] private float rPullThreshold;


#endregion


        #region INIT


        //----------------//
        public void Init()
        //---------------//
        {
            rb = GetComponentInChildren<Rigidbody>();

            if (isDebug == true)
            {
                speed = 50;
            }
            else
            {
                speed = 35;
            }
            if (rowingCanoeRacePlayerController == null)
            {
                rowingCanoeRacePlayerController = FindObjectOfType<RowingCanoeRacePlayerController>();
            }
#if MAXPRO_FITNESS
            gameManager = FindObjectOfType<GameManager>();
#endif

        } // END Init


#endregion


        #region MONOBEHAVIOURS


        //-------------------//
        private void Awake()
        //------------------//
        {
            Init();

        } // END Awake


        //-------------------//
        private void Update()
        //-------------------//
        {
            if (isDebug == true)
            {
                DebugMoveCanoe();
            }
            else
            {
                MoveCanoe();
            }

        } // END Update


        #endregion


        #region MOVE CANOE


        //----------------------//
        private void MoveCanoe()
        //---------------------//
        {
            //Debug.Log("CanoeController.cs // DebugMoveCanoe() // LeftPullDebug Value: " + leftPull);
            //Debug.Log("CanoeController.cs // DebugMoveCanoe() // RightPullDebug Value: " + rightPull);
#if MAXPRO_LOGIN
            if (leftPull > lPullThreshold && rightPull > rPullThreshold && isRepUp == false)
            {
                // This while loop applys force for a frame so it doesnt keep applying force in Update() every frame
                while (leftPull > lPullThreshold && rightPull > rPullThreshold && isRepUp == false)
                {
                    ForwardPaddle();
                    GameManager.isLeftUp = true;
                    GameManager.isRightUp = true;
                    isRepUp = true;
                    break;
                }
            }

            // Resets the ability to push the Canoe
            if (leftPull < lPullThreshold && rightPull < rPullThreshold && isRepUp == true)
            {
                GameManager.isLeftUp = false;
                GameManager.isRightUp = false;
                isRepUp = false;
            }
#endif
        } // END MoveCanoe


        //----------------------//
        private void DebugMoveCanoe()
        //---------------------//
        {
            //Debug.Log("CanoeController.cs // DebugMoveCanoe() // LeftPullDebug Value: " + gameManager.leftPullDebug);
            //Debug.Log("CanoeController.cs // DebugMoveCanoe() // RightPullDebug Value: " + gameManager.rightPullDebug);
#if MAXPRO_LOGIN
            if (gameManager.leftPullDebug > lPullThreshold && gameManager.rightPullDebug > rPullThreshold && isRepUp == false)
            {
                // This while loop applys force for a frame so it doesnt keep applying force in Update() every frame
                while (gameManager.leftPullDebug > lPullThreshold && gameManager.rightPullDebug > rPullThreshold && isRepUp == false)
                {
                    ForwardPaddle();
                    GameManager.isLeftUp = true;
                    GameManager.isRightUp = true;
                    isRepUp = true;

                    break;
                }
            }

            // Resets the ability to push the Canoe
            if (gameManager.leftPullDebug < lPullThreshold && gameManager.rightPullDebug < rPullThreshold && isRepUp == true)
            {
                GameManager.isLeftUp = false;
                GameManager.isRightUp = false;
                isRepUp = false;
            }
#endif
        } // END DebugMoveCanoe


#endregion


        #region FORWARD PADDLE


        //-------------------------//
        private void ForwardPaddle()
        //-------------------------//
        {
            TrackMaxSpeedMetric();

            rb.AddForceAtPosition((transform.forward) * -2 * speed, transform.position, ForceMode.Impulse);
            rb.drag = .5f;

        } // END ForwardPaddle


        #endregion


        #region METRIC UPDATES


        //--------------------------------//
        private void TrackMaxSpeedMetric()
        //-------------------------------//
        {
            if (rowingCanoeRacePlayerController == null)
            {
                rowingCanoeRacePlayerController = FindObjectOfType<RowingCanoeRacePlayerController>();
            }

            // Updates the Max Speed
            if (rowingCanoeRacePlayerController != null)
            {
#if MAXPRO_LOGIN
                rowingCanoeRacePlayerController.HandleMovementUpdated(Speed, ActionSide.BOTH);
#endif
            }

        } // END TrackMaxSpeedMetric


#endregion



    } // END CanoeController.cs
}