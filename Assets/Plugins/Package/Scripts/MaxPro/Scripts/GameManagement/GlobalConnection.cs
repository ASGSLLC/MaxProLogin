using MaxProFitness.Sdk;
using UnityEngine;
using UnityEngine.UI;
using maxprofitness.login;

public class GlobalConnection : MonoBehaviour
{
    #region VARIABLES

    [Header("UI ELEMENTS")]
    [SerializeField] private Image controllerStateUI;
    [Space]
    [SerializeField] private Sprite disconnectedUI;
    [SerializeField] private Sprite connectedUI;
    
    #endregion

    #region MONOBEHAVIORS

    //---------------------//
    private void Awake()
    //---------------------//
    {
        GameManager.OnConnectionStateChanged += OnConnnectionStateChanged;

    }//END Awake


    //---------------------//
    private void OnDestroy()
    //---------------------//
    {
        GameManager.OnConnectionStateChanged -= OnConnnectionStateChanged;

    }//END OnDestroy

    #endregion

    #region CONNECTION STATE CHANGED

    //---------------------------------------------------------------------//
    private void OnConnnectionStateChanged(MaxProControllerState state)
    //---------------------------------------------------------------------//
    {
        if (state == MaxProControllerState.Connected)
        {
           controllerStateUI.sprite = connectedUI;
        }
        else if(state == MaxProControllerState.Disconnected)
        {
            controllerStateUI.sprite = disconnectedUI;
        }

    }//END OnConnnectionStateChanged

    #endregion

}
