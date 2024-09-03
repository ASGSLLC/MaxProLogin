//using Den.Tools;
using Firebase.Firestore;
using Firebase.Database;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using maxprofitness.login;

//This Class Is Used To Make The Storage And Manipulation Of Two Variables Easier
[System.Serializable]
public class LocalPlayerInfo
{
    public string name;
    public int score;

    public LocalPlayerInfo(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}

public class LocalLeaderboard : MonoBehaviour {
    #region VARIABLES


    CanvasGroup localLeaderboardCanvasGroup;

    [SerializeField] LeaderboardEntry[] entries;
    [SerializeField] TextMeshProUGUI lbGameNameTMP;
    
    // Assigned in inspector depending on the scene the leaderboard is in
    [Tooltip("The current scene's game name")]
    public string gameName;

    [Tooltip("The current scene's game ID, typically the build index.")]
    [SerializeField] int currentGameID; // 2 = AirRunner Hills || 3 = AirRunner DarkSky || 4 = AirRunner DesertScene || 5 = AirRunner Forest || 6 = RowingCanoeRace || 7 = FitFighter Rhythym

    //List To Hold "PlayerInfo" Objects
    public List<LocalPlayerInfo> collectedStats = new List<LocalPlayerInfo>();
    public bool hasUpdatedOnce;
    public string firebaseStats;
    public string leaderboardEntryComparison;
    
    
    #endregion


    #region MONOBEHAVIOURS


    //----------------------------------//
    void Awake()
    //----------------------------------//
    {
        localLeaderboardCanvasGroup = GetComponent<CanvasGroup>();

        //LoadLeaderBoard("Test Game", 1);
        //SubmitButton("user", 10, 1);
    }
    // END Awake


    //------------------//
    private void Start()
    //------------------//
    {
        hasUpdatedOnce = false;

    } // END Start


    #endregion


    #region SUBMIT BUTTON


    //----------------------------------//
    public void SubmitButton(string _userName, int _score, int _gameID)
    //----------------------------------//
    {
        /*
        hasUpdatedOnce = false;
        currentGameID = _gameID;

        //Create Object Using Values From InputFields, This Is Done So That A Name And Score Can Easily Be Moved/Sorted At The Same Time
        LocalPlayerInfo _stats = new LocalPlayerInfo(_userName, _score);//Depending On How You Obtain The Score, It May Be Necessary To Parse To Integer
        if (!collectedStats.Contains(_stats))
        {
            //Add The New Player Info To The List
            //collectedStats.Add(_stats);
        }


        //Clear InputFields Now That The Object Has Been Created
        //userName = "";
        //score = 0;

        //entriesLoaded[currentGameID] = false;

        //Start Sorting Method To Place Object In Correct Index Of List
        //SortStats(false);
        */
    }
    // END SubmitButton


    #endregion


    #region INPUT/READ DATA


    //----------------------------------//
    public void InputData(string _userEntry, int _gameID)
    //----------------------------------//
    {
        hasUpdatedOnce = false;
        currentGameID = _gameID;

        string[] _userData = _userEntry.Split(',');
        string _userName = _userData[0];
        string _score = _userData[1];

        Debug.Log(_userData[0].ToString());
        Debug.Log(_userData[1].ToString());

        //Create Object Using Values From InputFields, This Is Done So That A Name And Score Can Easily Be Moved/Sorted At The Same Time
        LocalPlayerInfo _stats = new LocalPlayerInfo(_userName, Convert.ToInt32(_score));//Depending On How You Obtain The Score, It May Be Necessary To Parse To Integer
        if (!collectedStats.Contains(_stats))
        {
            //Add The New Player Info To The List
            collectedStats.Add(_stats);
        }

        _userData = null;
        _userName = ""; 
        _score = "";

        //Clear InputFields Now That The Object Has Been Created
        //userName = "";
        //score = 0;

        //entriesLoaded[currentGameID] = false;

        //Start Sorting Method To Place Object In Correct Index Of List
        SortStats(false);
    }
    // END SubmitButton


    //-------------------------------------//
    public void ReadData(string _userEntry)
    //------------------------------------//
    {
        if(_userEntry == "")
        {
            return;
        }

        string[] _userData = _userEntry.Split(',');
        string _userName = _userData[0];
        string _score = _userData[1];

        //Create Object Using Values From InputFields, This Is Done So That A Name And Score Can Easily Be Moved/Sorted At The Same Time
        LocalPlayerInfo _stats = new LocalPlayerInfo(_userName, Convert.ToInt32(_score));//Depending On How You Obtain The Score, It May Be Necessary To Parse To Integer
        if (!collectedStats.Contains(_stats))
        {
            //Add The New Player Info To The List
            collectedStats.Add(_stats);
        }

        _userData = null;
        _userName = "";
        _score = "";

        //Start At The End Of The List And Compare The Score To The Number Above It
        for (int i = collectedStats.Count - 1; i > 0; i -- )
        {
            //If The Current Score Is Higher Than The Score Above It , Swap
            if (collectedStats[i].score > collectedStats[i - 1].score)
            {
                //Temporary variable to hold small score
                LocalPlayerInfo tempInfo = collectedStats[i - 1];

                // Replace small score with big score
                collectedStats[i - 1] = collectedStats[i];
                //Set small score closer to the end of the list by placing it at "i" rather than "i-1"  
                collectedStats[i] = tempInfo;                
            }

            Debug.Log(collectedStats[i].name);
        }

        //Debug.Log(leaderboardEntryComparison);

        SortStats(true);

    } // END ReadData


    #endregion


    //-------------------------------------//
    public void ReadAirRunnerData(string _userEntry, bool isAirRunner = false)
    //------------------------------------//
    {
        if (_userEntry == "")
        {
            return;
        }

        string[] _userData = _userEntry.Split(',');
        string _userName = _userData[0];
        string _score = _userData[1];

        //Create Object Using Values From InputFields, This Is Done So That A Name And Score Can Easily Be Moved/Sorted At The Same Time
        LocalPlayerInfo _stats = new LocalPlayerInfo(_userName, Convert.ToInt32(_score));//Depending On How You Obtain The Score, It May Be Necessary To Parse To Integer
        if (!collectedStats.Contains(_stats))
        {
            //Add The New Player Info To The List
            collectedStats.Add(_stats);
        }

        _userData = null;
        _userName = "";
        _score = "";

        //Start At The End Of The List And Compare The Score To The Number Above It
        for (int i = collectedStats.Count - 1; i > 0; i--)
        {
            //If The Current Score Is Higher Than The Score Above It , Swap
            if (collectedStats[i].score > collectedStats[i - 1].score)
            {
                //Temporary variable to hold small score
                LocalPlayerInfo tempInfo = collectedStats[i - 1];

                // Replace small score with big score
                collectedStats[i - 1] = collectedStats[i];
                //Set small score closer to the end of the list by placing it at "i" rather than "i-1"  
                collectedStats[i] = tempInfo;
            }

            //Debug.Log(collectedStats[i].name);
        }

        SortStats(true);

    } // END ReadData


    #region SORT STATS


    //----------------------------------//
    void SortStats(bool isUpdating)
    //----------------------------------//
    {
        //Start At The End Of The List And Compare The Score To The Number Above It
        for (int i = collectedStats.Count - 1; i > 0; i -- )
        {
            //If The Current Score Is Higher Than The Score Above It , Swap
            if (collectedStats[i].score > collectedStats[i - 1].score)
            {
                //Temporary variable to hold small score
                LocalPlayerInfo tempInfo = collectedStats[i - 1];

                // Replace small score with big score
                collectedStats[i - 1] = collectedStats[i];
                //Set small score closer to the end of the list by placing it at "i" rather than "i-1"  
                collectedStats[i] = tempInfo;                
            }   
        }
        Debug.Log("LocalLeaderboard.cs // SortStats()//");
        //Update PlayerPref That Stores Leaderboard Values
        UpdateAndSetStatsString(isUpdating);
    }
    // END SortStats


    #endregion


    #region UPDATE AND SET STATS STRING


    /* on submit we create a new container 
     * set container with leaderboard data
     * on population of data containers check the scores to see which one is higher by 
        if we are at the maximum number of entries, sort the highest 10 scores
     * fill out the UI
     */
    //----------------------------------//
    void UpdateAndSetStatsString(bool isUpdating)
    //----------------------------------//
    {
        //Start With A Blank String
        string stats = "";

        //Add Each Name And Score From The Collection To The String
        for(int i = 0; i < collectedStats.Count; i++)
        {
            //Be Sure To Add A Comma To Both The Name And Score, It Will Be Used To Separate The String Later
            stats += collectedStats[i].name + ",";
            stats += collectedStats[i].score + ",";           
        }

        // 2 = AirRunner Hills || 3 = AirRunner DarkSky || 4 = AirRunner DesertScene || 5 = AirRunner Forest || 6 = RowingCanoeRace || 7 = FitFighter Rhythym
        //Add The String To The PlayerPrefs, This Allows The Information To Be Saved Even When The Game Is Turned Off
        switch (currentGameID)
        {
            case 1:
                {
                    PlayerPrefs.SetString("LB_Test", stats);
                    break;
                }
            case 3:
                {
                    firebaseStats = stats;
                    //lbGameNameTMP.text = "Air Runner - Hills";
                    break;
                }
            case 4:
                {
                    firebaseStats = stats;
                    //lbGameNameTMP.text = "Air Runner - Dark Sky";
                    break;
                }
            case 5:
                {
                    firebaseStats = stats;
                    //lbGameNameTMP.text = "Air Runner - Desert";
                    break;
                }
            case 6:
                {
                    firebaseStats = stats;
                    //lbGameNameTMP.text = "Air Runner - Forest";
                    break;
                }
            case 7:
                {
                    firebaseStats = stats;
                    //lbGameNameTMP.text = "Rowing Canoe Race";
                    break;
                }
            case 8:
                {
                    firebaseStats = stats;
                    //lbGameNameTMP.text = "Fit Fighter";
                    break;
                }
        }

        if(isUpdating == true)
        {
            UpdateOnlineLeaderboards();
        }
        else
        {
            //Debug.Log("LocalLeaderboard.cs // UpdatePlayerPrefsString()// Updated PlayerPrefs");
            //Now Update The On Screen LeaderBoard
            UpdateLeaderBoardVisual();
            //UpdateOnlineLeaderboards();
        }
        //UpdateOnlineLeaderboards();
    }
    // END UpdatePlayerPrefsString


    #endregion


    #region UPDATE LEADERBOARD VISUAL


    //----------------------------------//
    private void UpdateLeaderBoardVisual()
    //----------------------------------//
    {
        //Simply Loop Through The List And Add The Name And Score To The Display Text
        for (int i = 0; i < entries.Length && i < collectedStats.Count; i++)
        {
            if (i > 24)
            {
                return;
            }

            if (!leaderboardEntryComparison.Contains(entries[i].entryName + "," + entries[i].entryScore + " ") && entries[i].entryScore != 0)
            {
                leaderboardEntryComparison += entries[i].entryName + "," + entries[i].entryScore + " ";
            }

            entries[i].entryName = collectedStats[i].name;
            entries[i].entryScore = collectedStats[i].score;
            entries[i].SetEntry();

            //Debug.Log( "Collected Stats: " + collectedStats[i].name + " " + collectedStats[i].score);
        }
    }
    // END UpdateLeaderBoardVisual


    #endregion

    //-----------------------------------//
    private void UpdateAirRunnerEntries()
    //------------------------------------//
    {
        Debug.Log("UpdateAirRunnerEntries()");

        for (int i = 0; i < collectedStats.Count; i++)
        {
            if (i > 24)
            {
                return;
            }

            /*
            if (!leaderboardEntryComparison.Contains(entries[i].entryName + "," + entries[i].entryScore + " ") && entries[i].entryScore != 0)
            {
                leaderboardEntryComparison += entries[i].entryName + "," + entries[i].entryScore + " ";

            }

            entries[i].entryName = collectedStats[i].name;
            entries[i].entryScore = collectedStats[i].score;
            entries[i].SetEntry();
             */
        }

    } // END UpdateAirRunnerEntries


    #region UPDATE ONLINE LEADERBOARDS


    //----------------------------------//
    public void UpdateOnlineLeaderboards()
    //----------------------------------//
    {
        
        //Simply Loop Through The List And Add The Name And Score To The Display Text
        for (int i = 0; i < entries.Length && i < collectedStats.Count; i++)
        {
            if (i > 24)
            {
                return;
            }

            if (!leaderboardEntryComparison.Contains(entries[i].entryName + "," + entries[i].entryScore + " ") && entries[i].entryScore != 0)
            {
                leaderboardEntryComparison += entries[i].entryName + "," + entries[i].entryScore + " ";
            }

            entries[i].entryName = collectedStats[i].name;
            entries[i].entryScore = collectedStats[i].score;
            entries[i].SetEntry();

            //Debug.Log( "Collected Stats: " + collectedStats[i].name + " " + collectedStats[i].score);
        }
        
        //Debug.Log(leaderboardEntryComparison);
        //Debug.Log("LocalLeaderboard.cs // UpdateLeaderBoardVisual()// Finished setting data loop");

    } // END UpdateLeaderBoardVisual


    #endregion


    #region UPDATE TO FIREBASE


    //-----------------------------//
    public void UpdateCanoeToFirebase()
    //-----------------------------//
    {
        Debug.Log("Sent Data to Firebase");

        string[] updatedFirebaseData = leaderboardEntryComparison.Split(' ');

        for (int i = 0; i < updatedFirebaseData.Length; i++)
        {
            if (i > 24)
            {
                break;
            }
        }

        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            {"scores",  updatedFirebaseData}
        };

        DocumentReference _leaderboardReference = FirestoreDatabaseManager.db.Collection("maxProGamesApp").Document("gameLeaderboards").Collection("canoeRowing").Document("leaderboards").Collection("entry").Document("TEST");

        FirestoreDatabaseManager.Instance.SaveDocument(_leaderboardReference, userData, UserDataManager.OnUserDataInitialized,
        (error) =>
        {
            Debug.LogError("UserData//InitializeUserData// error " + error);
        });

    } // END UpdateToFirebase


    //-----------------------------//
    public void UpdateAirRunnerHillsToFirebase()
    //-----------------------------//
    {
        Debug.Log("Sent Data to Firebase");

        string[] updatedFirebaseData = leaderboardEntryComparison.Split(' ');

        for (int i = 0; i < updatedFirebaseData.Length; i++)
        {
            if (i > 24)
            {
                break;
            }
        }

        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            {"scores",  updatedFirebaseData}
        };

        DocumentReference _leaderboardReference = FirestoreDatabaseManager.db.Collection("maxProGamesApp").Document("gameLeaderboards").Collection("airRunner").Document("scores").Collection("hills").Document("leaderboards").Collection("entry").Document("VSm5NhKPR4HSm9JXYNrF");

        FirestoreDatabaseManager.Instance.SaveDocument(_leaderboardReference, userData, UserDataManager.OnUserDataInitialized,
        (error) =>
        {
            Debug.LogError("UserData//InitializeUserData// error " + error);
        });

    } // END UpdateToFirebase


    //-----------------------------//
    public void UpdateAirRunnerDarkSkyToFirebase()
    //-----------------------------//
    {
        Debug.Log("Sent Data to Firebase");

        string[] updatedFirebaseData = leaderboardEntryComparison.Split(' ');

        for (int i = 0; i < updatedFirebaseData.Length; i++)
        {
            if (i > 24)
            {
                break;
            }
        }

        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            {"scores",  updatedFirebaseData}
        };

        DocumentReference _leaderboardReference = FirestoreDatabaseManager.db.Collection("maxProGamesApp").Document("gameLeaderboards").Collection("airRunner").Document("scores").Collection("darkSky").Document("leaderboards").Collection("entry").Document("GP9rqC5XJtHGsV13xxFe");

        FirestoreDatabaseManager.Instance.SaveDocument(_leaderboardReference, userData, UserDataManager.OnUserDataInitialized,
        (error) =>
        {
            Debug.LogError("UserData//InitializeUserData// error " + error);
        });

    } // END UpdateToFirebase


    //-----------------------------//
    public void UpdateAirRunnerDesertToFirebase()
    //-----------------------------//
    {
        Debug.Log("Sent Data to Firebase");

        string[] updatedFirebaseData = leaderboardEntryComparison.Split(' ');

        for (int i = 0; i < updatedFirebaseData.Length; i++)
        {
            if (i > 24)
            {
                break;
            }
        }

        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            {"scores",  updatedFirebaseData}
        };

        DocumentReference _leaderboardReference = FirestoreDatabaseManager.db.Collection("maxProGamesApp").Document("gameLeaderboards").Collection("airRunner").Document("scores").Collection("desert").Document("leaderboards").Collection("entry").Document("FiHa3i3oDsHlPoFbpw7k");

        FirestoreDatabaseManager.Instance.SaveDocument(_leaderboardReference, userData, UserDataManager.OnUserDataInitialized,
        (error) =>
        {
            Debug.LogError("UserData//InitializeUserData// error " + error);
        });

    } // END UpdateToFirebase


    //-----------------------------//
    public void UpdateAirRunnerForestToFirebase()
    //-----------------------------//
    {
        Debug.Log("Sent Data to Firebase");

        string[] updatedFirebaseData = leaderboardEntryComparison.Split(' ');

        for (int i = 0; i < updatedFirebaseData.Length; i++)
        {
            if (i > 24)
            {
                break;
            }
        }

        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            {"scores",  updatedFirebaseData}
        };

        DocumentReference _leaderboardReference = FirestoreDatabaseManager.db.Collection("maxProGamesApp").Document("gameLeaderboards").Collection("airRunner").Document("scores").Collection("forest").Document("leaderboards").Collection("entry").Document("GYIAhNboL3s42hdyeTik");

        FirestoreDatabaseManager.Instance.SaveDocument(_leaderboardReference, userData, UserDataManager.OnUserDataInitialized,
        (error) =>
        {
            Debug.LogError("UserData//InitializeUserData// error " + error);
        });

    } // END UpdateToFirebase


    //-----------------------------//
    public void UpdateFitFighterToFirebase()
    //-----------------------------//
    {
        Debug.Log("Sent Data to Firebase");

        string[] updatedFirebaseData = leaderboardEntryComparison.Split(' ');

        for (int i = 0; i < updatedFirebaseData.Length; i++)
        {
            if (i > 24)
            {
                break;
            }
        }

        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            {"scores",  updatedFirebaseData}
        };

        DocumentReference _leaderboardReference = FirestoreDatabaseManager.db.Collection("maxProGamesApp").Document("gameLeaderboards").Collection("fitFighter").Document("leaderboards").Collection("entry").Document("JPiSMSJtkRB4XFXCk1x5");

        FirestoreDatabaseManager.Instance.SaveDocument(_leaderboardReference, userData, UserDataManager.OnUserDataInitialized,
        (error) =>
        {
            Debug.LogError("UserData//InitializeUserData// error " + error);
        });

    } // END UpdateToFirebase


    #endregion


    #region LOAD LEADERBOARD



    //------------------------------------//
    public void AirRunnerLeaderboardSetup()
    //------------------------------------//
    {
#if AIR_RUNNER
        switch (AirRunnerGameManager.selectedLevel.gameID)
        {
            case 3:
                LoadHillsLeaderboardsInGame();
                break;

            case 4:
                LoadDarkSkyLeaderboardsInGame();
                break;

            case 5:
                LoadDesertLeaderboardsInGame();
                break;

            case 6:
                LoadForestLeaderboardsInGame();
                break;

            default:
                LoadHillsLeaderboardsInGame();
                break;
        }
#endif
    }


    //---------------------------------------//
    private void LoadHillsLeaderboardsInGame()
    //---------------------------------------//
    {
        ClearLeaderboards();
        gameName = "Air Runner - Hills";
        currentGameID = 3;
        lbGameNameTMP.text = "Air Runner - Hills";
        UserDataManager.Instance.GetHillsLeaderboardEntries();

    } // END LoadHillsLeaderboardsInGame


    //-----------------------------------------//
    private void LoadDarkSkyLeaderboardsInGame()
    //-----------------------------------------//
    {
        gameName = "Air Runner - Dark Sky";
        currentGameID = 4;
        lbGameNameTMP.text = "Air Runner - Dark Sky";
        UserDataManager.Instance.GetDarkSkyLeaderboardEntries();

    }// END LoadDarkSkyLeaderboardsInGame


    //----------------------------------------//
    private void LoadDesertLeaderboardsInGame()
    //---------------------------------------//
    {
        gameName = "Air Runner - Desert";
        currentGameID = 5;
        lbGameNameTMP.text = "Air Runner - Desert";
        UserDataManager.Instance.GetDesertLeaderboardEntries();

    } // END LoadDesertLeaderboardsInGame


    //----------------------------------------//
    private void LoadForestLeaderboardsInGame()
    //----------------------------------------//
    {
        ClearLeaderboards();
        gameName = "Air Runner - Forest";
        currentGameID = 6;
        lbGameNameTMP.text = "Air Runner - Forest";
        UserDataManager.Instance.GetForestLeaderboardEntries();

    } // END LoadForestLeaderboardsInGame


    //---------------------------------------//
    public void LoadLeaderboardInGameRowing()
    //---------------------------------------//
    {
        currentGameID = 7;
        lbGameNameTMP.text = "Canoe Rowing";

        UserDataManager.Instance.GetRowingLeaderboardEntries();

    } // END LoadLeaderboardInGameRowing


    //------------------------------------------//
    public void LoadLeaderboardInGameFitFighter()
    //------------------------------------------//
    {
        currentGameID = 8;
        lbGameNameTMP.text = "Fit Fighter";
        UserDataManager.Instance.GetFitFighterLeaderboardEntries();

    } // END LoadLeaderboardInGameFitFighter


    //----------------------------------//
    public void LoadLeaderBoard(string _gameName, int _gameID)
    //----------------------------------//
    {
        //Clear the current collectedstats collection before re-displaying
        //collectedStats.Clear();
        //ClearLeaderboards();

        currentGameID = _gameID;

        // Update game name text
        //lbGameNameTMP.text = _gameName;

        //Load The String Of The Leaderboard That Was Saved In The "UpdatePlayerPrefsString" Method
        string _stats = "";

        // 2 = AirRunner Hills || 3 = AirRunner DarkSky || 4 = AirRunner DesertScene || 5 = AirRunner Forest || 6 = RowingCanoeRace || 7 = FitFighter Rhythym
        switch (currentGameID)
        {
            case 1:
                {
                    _stats = PlayerPrefs.GetString("LB_Test", "");
                    break;
                }
            case 3:
                {
                    lbGameNameTMP.text = _gameName;
                    _stats = firebaseStats;
                    break;
                }
            case 4:
                {
                    lbGameNameTMP.text = _gameName;
                    _stats = firebaseStats;
                    break;
                }
            case 5:
                {
                    lbGameNameTMP.text = _gameName;
                    _stats = firebaseStats;
                    break;
                }
            case 6:
                {
                    lbGameNameTMP.text = _gameName;
                    _stats = firebaseStats;
                    break;
                }
            case 7:
                {
                    lbGameNameTMP.text = _gameName;
                    _stats = firebaseStats;
                    break;
                }
            case 8:
                {
                    lbGameNameTMP.text = _gameName;
                    _stats = firebaseStats;
                    break;
                }
        }

        //Assign The String To An Array And Split Using The Comma Character
        //This Will Remove The Comma From The String, And Leave Behind The Separated Name And Score
        string[] _statsSplit = _stats.Split(',');

        //Loop Through The Array 2 At A Time Collecting Both The Name And Score
        for (int i = 0; i < _statsSplit.Length - 2; i += 2)
        {
            //Use The Collected Information To Create An Object
            LocalPlayerInfo loadedInfo = new LocalPlayerInfo(_statsSplit[i], int.Parse(_statsSplit[i + 1]));

            if(!collectedStats.Contains(loadedInfo))
            {
                //Add The Object To The List
                collectedStats.Add(loadedInfo);
            }
        }

        //Update On Screen LeaderBoard
        UpdateLeaderBoardVisual();

    }
    // END LoadLeaderboard


#endregion


    #region CLEAR LEADERBOARDS/PREFS


    //-----------------------------//
    public void ClearLeaderboards()
    //----------------------------//
    {
        Debug.Log("LocalLeaderboards.cs // ClearLeaderboards() // Clearing Leaderboards");

        for (int i = 0; i < entries.Length; i++)
        {
            entries[i].entryNameTMP.text = "Player";
            entries[i].entryScoreTMP.text = "0";
            entries[i].entryScore = 0;
        }

        collectedStats.Clear();
        leaderboardEntryComparison = "";
        firebaseStats = "";

    } // END ClearLeaderboards


    //----------------------------------//
    public void ClearPrefs()
    //----------------------------------//
    {
        //Use This To Delete All Names And Scores From The LeaderBoard
        //PlayerPrefs.DeleteAll();
    }
    // END ClearPrefs


    #endregion


    #region OPEN/CLOSE LOCAL LEADERBOARD


    //----------------------------------//
    public void CloseLocalLeaderboard()
    //----------------------------------//
    {
        localLeaderboardCanvasGroup.alpha = 0f;
        localLeaderboardCanvasGroup.interactable = false;
        localLeaderboardCanvasGroup.blocksRaycasts = false;
    }
    // END CloseLocalLeaderboard


    //----------------------------------//
    public void OpenLocalLeaderboard()
    //----------------------------------//
    {
        LoadLeaderBoard(gameName, currentGameID);

        localLeaderboardCanvasGroup.alpha = 1f;
        localLeaderboardCanvasGroup.interactable = true;
        localLeaderboardCanvasGroup.blocksRaycasts = true;
    }
    // END OpenLocalLeaderboard


    #endregion


} // END Class
