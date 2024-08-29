using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using maxprofitness.login;

public class MainMenuSceneManager : CanvasGroupUIBase
{
    #region ENUMS


    public enum ActivePage
    {
        none,
        homePage,
        leaderBoardPage,
        airRunnerListPage
    }


    #endregion


    #region VARIABLES


    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject leaderboardCanvas;

    [Header("LEADERBOARD")]
    [SerializeField] private CanvasGroup mainListCanvasGroup;
    [SerializeField] private CanvasGroup localLeaderboardsCanvasGroup;
    [SerializeField] private CanvasGroup airRunnerListCanvasGroup;
    [Space]
    [SerializeField] private CanvasGroup nextPageCanvasGroup;
    [SerializeField] private GameObject exitButton;

    [SerializeField] private ActivePage activePage = ActivePage.homePage;
    [SerializeField] private AudioSource menuButtonSFX;

    private ProfilePage profilePage;

    private LocalLeaderboard localLeaderboard;
    private GameSelectionCanvas gameSelectionCanvas;

    #endregion


    #region AWAKE
    //----------------------//
    protected override void Awake()
    //----------------------//
    {
        base.Awake();

        profilePage = FindObjectOfType<ProfilePage>();
        
        localLeaderboard = FindObjectOfType<LocalLeaderboard>();
        gameSelectionCanvas = FindObjectOfType<GameSelectionCanvas>();

        if (gameSelectionCanvas != null)
            gameSelectionCanvas.Hide();

    }//END Awake
    #endregion


    #region SHOW LEADERBOARD
    //-----------------------------//
    public void ShowLeaderboard()
    //-----------------------------//
    {
        PlayMenuSFX();
        mainCanvas.SetActive(false);
        leaderboardCanvas.GetComponent<CanvasGroup>().alpha = 1;
        leaderboardCanvas.GetComponent<CanvasGroup>().interactable = true;
        leaderboardCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;

    }//END ShowLeaderboard
    #endregion


    #region LEADER BOARD LOGIC

    #region LOAD APPROPRIATE LEADERBOARD
    //-------------------------------//
    public void LoadLeaderBoard(int gameId)
    //-------------------------------//
    {
        PlayMenuSFX();
        //Select the name based off of game id
        string gameName = "";
        switch (gameId)
        {
            case 1:
                {
                    gameName = "Test";
                    break;
                }
            case 3:
                {
                    gameName = "Air Runner - Hills";
                    break;
                }
            case 4:
                {
                    gameName = "Air Runner - Dark Sky";
                    break;
                }
            case 5:
                {
                    gameName = "Air Runner - Desert";
                    break;
                }
            case 6:
                {
                    gameName = "Air Runner - Forest";
                    break;
                }
            case 7:
                {
                    gameName = "Rowing Canoe";
                    break;
                }
            case 8:
                {
                    gameName = "Fit Fighter";
                    break;
                }
        }

        //Get the current active canvas group to hide
        CanvasGroup activeCanvasGroup = null;
        if (activePage != ActivePage.airRunnerListPage)
        {
            activeCanvasGroup = DetermineActivePage();
        }
        else
        {
            activeCanvasGroup = airRunnerListCanvasGroup;
        }

        exitButton.SetActive(false);
        activeCanvasGroup.interactable = false;
        activeCanvasGroup.blocksRaycasts = false;
        activeCanvasGroup.DOFade(0f, 0f).OnComplete(() =>
        {
            //Load the appropriate leaderboard
            localLeaderboard.LoadLeaderBoard(gameName, gameId);

            //Make sure we can return to the appropriate page based off the currently active one
            if (activePage != ActivePage.homePage)
            {
                activePage = ActivePage.leaderBoardPage;
            }
            else
            {
                activePage = ActivePage.airRunnerListPage;
            }

            localLeaderboardsCanvasGroup.DOFade(1f, 0f).OnComplete(() =>
            {
                localLeaderboardsCanvasGroup.interactable = true;
                localLeaderboardsCanvasGroup.blocksRaycasts = true;
            });
        });

    }//END LoadLeaderBoard
    #endregion


    #region CLEAR LEADERBOARD STATS


    //--------------------------------//
    public void ClearLeaderboardStats()
    //---------------------------------//
    {
        localLeaderboard.ClearLeaderboards();

    } // END ClearLeaderboardStats
    
    
    #endregion


    #region LOAD STATS


    //---------------------------------------//
    public void LoadRowingStatsFromFirebase()
    //---------------------------------------//
    {
        UserDataManager.Instance.GetRowingLeaderboardEntries();
    }
    

    //-------------------------------------------//
    public void LoadFitFighterStatsFromFirebase()
    //-------------------------------------------//
    {
        UserDataManager.Instance.GetFitFighterLeaderboardEntries();
    }


    //-------------------------------------------//
    public void LoadHillsStatsFromFirebase()
    //-------------------------------------------//
    {
        UserDataManager.Instance.GetHillsLeaderboardEntries();
    }


    //-------------------------------------------//
    public void LoadDarkSkyStatsFromFirebase()
    //-------------------------------------------//
    {
        UserDataManager.Instance.GetDarkSkyLeaderboardEntries();
    }


    //-------------------------------------------//
    public void LoadDesertStatsFromFirebase()
    //-------------------------------------------//
    {
        UserDataManager.Instance.GetDesertLeaderboardEntries();
    }


    //-------------------------------------------//
    public void LoadForestStatsFromFirebase()
    //-------------------------------------------//
    {
        UserDataManager.Instance.GetForestLeaderboardEntries();
    }


    #endregion


    #region LOAD AIR RUNNER LIST

    //-------------------------------//
    public void LoadAirRunnerPage()
    //-------------------------------//
    {
        PlayMenuSFX();
        activePage = ActivePage.none;
        exitButton.SetActive(false);

        mainListCanvasGroup.interactable = false;
        mainListCanvasGroup.blocksRaycasts = false;
        mainListCanvasGroup.DOFade(0f, 0f).OnComplete(() =>
        {
            airRunnerListCanvasGroup.DOFade(1f, 0f).OnComplete(() =>
            {
                airRunnerListCanvasGroup.interactable = true;
                airRunnerListCanvasGroup.blocksRaycasts = true;
            });
        });

    }//END LoadAirRunnerPage

    #endregion


    #region BACK / EXIT

    //-----------------//
    public void Back()
    //-----------------//
    {
        PlayMenuSFX();
        CanvasGroup activeCanvasGroup = DetermineActivePage();

        activeCanvasGroup.interactable = false;
        activeCanvasGroup.blocksRaycasts = false;
        activeCanvasGroup.DOFade(0f, 0f).OnComplete(() =>
        {
            nextPageCanvasGroup.DOFade(1f, 0f).OnComplete(() =>
            {
                nextPageCanvasGroup.interactable = true;
                nextPageCanvasGroup.blocksRaycasts = true;

                //Check to see what page we're returning to
                if (activePage != ActivePage.airRunnerListPage)
                {
                    exitButton.SetActive(true);
                }
                else
                {
                    activePage = ActivePage.none;
                }
            });
        });

    }//END Back

    //-----------------//
    public void ExitLeaderboards()
    //-----------------//
    {
        PlayMenuSFX();
        mainCanvas.SetActive(true);
        leaderboardCanvas.GetComponent<CanvasGroup>().alpha = 0;
        leaderboardCanvas.GetComponent<CanvasGroup>().interactable = false;
        leaderboardCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;

    }//END Exit

    #endregion


    #region DETERMINE ACTIVE PAGE
    //----------------------------------//
    /// <summary>
    /// Determine which canvas group to interact with
    /// </summary>
    /// <returns></returns>
    private CanvasGroup DetermineActivePage()
    //----------------------------------//
    {
        switch (activePage)
        {
            case ActivePage.homePage:
                activePage = ActivePage.leaderBoardPage;
                nextPageCanvasGroup = mainListCanvasGroup;
                return mainListCanvasGroup;

            case ActivePage.airRunnerListPage:
                activePage = ActivePage.airRunnerListPage;
                nextPageCanvasGroup = airRunnerListCanvasGroup;
                return localLeaderboardsCanvasGroup;

            case ActivePage.leaderBoardPage:
                activePage = ActivePage.homePage;
                nextPageCanvasGroup = mainListCanvasGroup;
                return localLeaderboardsCanvasGroup;

            case ActivePage.none:
                activePage = ActivePage.homePage;
                nextPageCanvasGroup = mainListCanvasGroup;
                return airRunnerListCanvasGroup;
        }

        return mainListCanvasGroup;

    }//END DetermineActivePage
    #endregion


    #endregion


    #region LOAD MAIN UI
    //--------------------------//
    public void LoadMainHubUI()
    //--------------------------//
    {
        PlayMenuSFX();
        Hide();
        gameSelectionCanvas.Show();

    }//END LoadScene
    #endregion


    //----------------------------------//
    public void ProfilePagePressed()
    //----------------------------------//
    {
        profilePage.ForceShow();

    } // END ShowProfilePage

    #region PLAY MENU SFX
    //-----------------------//
    public void PlayMenuSFX()
    //----------------------//
    {
        menuButtonSFX.Play();

    } // END PlayMenuSFX
    #endregion

} // END Class