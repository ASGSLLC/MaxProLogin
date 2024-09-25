using UnityEngine;
using UnityEngine.Serialization;
using maxprofitness.login;

namespace maxprofitness.shared
{
    public class RowingCanoeGameManager : MonoBehaviour
    {
        #region VARIABLES


        public event System.Action OnGameStarted;
        public event System.Action OnGameFinish;
        public event System.Action OnDeviceConnected;
        public bool hasStarted = false;


        #endregion


        #region MONOBEHAVIOURS


        //------------------//
        private void Awake()
        //------------------//
        {
            hasStarted = false;
        }


        //------------------//
        private void Start()
        //-----------------//
        {
            hasStarted = false;

        } // END Start


        #endregion


        #region START / FINISH GAME


        //---------------------//
        public void StartGame()
        //--------------------//
        {
            OnGameStarted?.Invoke();

        } // END StartGame



        //----------------------//
        public void FinishGame()
        //---------------------//
        {
            OnGameFinish?.Invoke();

        } // END FinishGame


        #endregion


    } // END RowingCaneGameManager


} // END Namespace
