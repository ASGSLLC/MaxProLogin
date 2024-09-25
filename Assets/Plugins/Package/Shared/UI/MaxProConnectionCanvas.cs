using MaxProFitness.Sdk;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System;
using UnityEngine.SceneManagement;
using maxprofitness.shared;
using maxprofitness.login;

public class MaxProConnectionCanvas : YScaleUIBase
{
    #region VARIABLES
    public static bool isConnected = false;

    [SerializeField] private CanvasGroupUIBase nextMenuCanvas;
    [SerializeField] private CanvasGroupUIBase errorConnectingCanvas;

    [SerializeField] private TMP_Text _stateLabel;

    private GameManager gameManager;

    private MaxProController maxProController;

    protected Canvas canvas;

    #endregion

    #region MONOBEHAVIOURS
    //------------------//
    private void Awake()
    //------------------//
    {
        GameManager.OnConnectionStateChanged += OnConnectionStateChanged;

        canvas = GetComponentInParent<Canvas>();
        canvas.sortingOrder = 101;
    } // END Awake

    //------------------//
    private void Start()
    //------------------//
    {
        maxProController = FindObjectOfType<MaxProController>();

        if (maxProController != null)
        {
            _stateLabel.text = maxProController.State.ToString();
        }

    } // END Start
    #endregion

    #region ON CONNECT PRESSED
    //----------------------------//
    public void OnConnectPressed()
    //----------------------------//
    {
        Debug.Log("MaxProConnectionCanvas//OnConnectPressed//");

#if UNITY_EDITOR
        GoToNextMenu();
#endif

#if !UNITY_EDITOR
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        if (gameManager != null)
        {
            Debug.Log("MaxProConnectionCanvas//OnConnectPressed//have gameManager");
            GoToNextMenu();
            gameManager.Connect();
        }
        else
        {
            Debug.Log("MaxProConnectionCanvas//OnConnectPressed//dont have gameManager");
            errorConnectingCanvas.ForceShow();
        }
#else
        GameManager.OnConnectionStateChanged?.Invoke(MaxProControllerState.Connected);
#endif

    } // END OnConnectPressed

    #endregion

    public void OnConnectionStateChanged(MaxProControllerState state)
    {
        Debug.Log("MaxProConnectionCanvas//OnConnectionStateChanged//");

        _stateLabel.text = state.ToString();

        if (state == MaxProControllerState.Connected)
        {
            isConnected = true;

            Hide();
        }
        else
        {
            isConnected = false;
            Debug.Log("MaxProConnectionCanvas//OnConnectionStateChanged//Max pro not connected");
        }

    }


    //------------------------------------//
    public void GoToNextMenu()
    //------------------------------------//
    {
        CanvasGroupUIBase _thisCanvas = this.gameObject.GetComponent<CanvasGroupUIBase>();
        _thisCanvas.ForceHide();
        nextMenuCanvas.ForceShow();
    }


    private void OnDestroy()
    {
        GameManager.OnConnectionStateChanged -= OnConnectionStateChanged;
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (transform.localScale.y != 0)
        {
            if (GUI.Button(new Rect(10, 70, 50, 30), "Connect"))
            {
                OnConnectPressed();
            }
        }

    }
#endif
}