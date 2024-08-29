using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using maxprofitness.login;

public class GameSelectionCanvas : CanvasGroupUIBase
{
    [SerializeField] int setGameIndex;

    private static int selectedLevel;
    private MainMenuSceneManager mainMenuSceneManager;

    #region AWAKE

    //----------------------//
    private void Awake()
    //----------------------//
    {
        mainMenuSceneManager = FindObjectOfType<MainMenuSceneManager>();

    }//END Awake

    #endregion

    #region START GAME

    //------------------------------//
    public void StartLoad()
    //-----------------------------//
    {
        AsyncOperation loadingScreen = SceneManager.LoadSceneAsync("LoadingScreen");
    }
    // END StartGame

    //------------------------------//
    public static int GetSelectedLevel()
    //------------------------------//
    {
        return selectedLevel;
    }
    // END GetSelectedLevel

    #endregion

    #region GAME SELECTED

    //------------------------------//
    public void SelectAirRunner()
    //-----------------------------//
    {
        //setGameIndex = 1;
        selectedLevel = 1;
        StartLoad();
    }
    // END SelectAirRunner



    //------------------------------//
    public void SelectRowing()
    //-----------------------------//
    {
        //setGameIndex = 6;
        selectedLevel = 2;
        StartLoad();
    }
    // END SelectRowing



    //------------------------------//
    public void SelectBoxing()
    //-----------------------------//
    {
        //setGameIndex = 7;
        selectedLevel = 3;
        StartLoad();
    }
    // END SelectBoxing

    #endregion

    #region QUIT

    //------------------------------//
    public void QuitGame()
    //-----------------------------//
    {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    // END QuitGame

    #endregion

    #region BACK

    //-------------------------------//
    public void BackToMainMenu()
    //-------------------------------//
    {
        Hide();
        mainMenuSceneManager.Show();

    }//END BackToMainMenu

    #endregion
}
