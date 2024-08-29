using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using maxprofitness.login;

public class SettingsPage : CanvasGroupUIBase
{
    #region VARIABLES


    [SerializeField] private CanvasGroupUIBase[] settingsPanels;


    #endregion

    #region Methods


    //----------------------------------//
    protected override void Awake()
    //----------------------------------//
    {
        base.Awake();

        ForceHide();

    } // END Awake


    //----------------------------------//
    public void ShowPanel(int _panelID)
    //----------------------------------//
    {
        settingsPanels[_panelID].ForceShow();

    } // END ShowPanel


    //----------------------------------//
    public void HidePanel(int _panelID)
    //----------------------------------//
    {
        settingsPanels[_panelID].ForceHide();

    } // END HidePanel


    //----------------------------------//
    public void ExitButtonPressed()
    //----------------------------------//
    {
        ForceHide();

    } // END ExitButtonPressed


    private void OnDestroy()
    {
        Destroy(canvasGroup);
    }

    #endregion

} // END Class
