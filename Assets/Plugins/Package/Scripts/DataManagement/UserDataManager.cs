using Firebase.Database;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Linq;
using UnityEngine;
using static SystemOfMeasurementHelper;
using static UnityEngine.UIElements.UxmlAttributeDescription;
//using MaxProFitness.Shared.Utilities;
using Firebase.Auth;

#if FIT_FIGHTER
using _Project.FitFighter.RhythmRevamp.Scripts.Score;
using FitFighter.RhythmRevamp.Scripts.Results;
#endif

#if ROWING_CANOE
using _Project.RowingCanoe.Scripts;
#endif

#if UNITY_EDITOR
using UnityEditor.PackageManager;
#endif
//using Sirenix.Utilities;
using System.Linq;
using System.Runtime.CompilerServices;
using maxprofitness.login;

namespace maxprofitness.login 
{
    public class UserDataManager : Singleton<UserDataManager>
    {
        #region VARIABLES

#if ROWING_CANOE
    public RowingTrackManager rowingTrackManager;
#endif
        public static Action OnUserDataInitialized;

        public LocalLeaderboard localLeaderboard;
        public string[] leaderboardEntries;

        public static UserDataMeta loadedData;
        public static GameMetrics.AirRunnerGameMetrics airRunnerLoadedData;
        public static GameMetrics.FitFighterGameMetrics fitFighterLoadedData;
        public static GameMetrics.RowingCanoeGameMetrics rowingCanoeLoadedData;

        private static string mobileFirebaseCollection = "maxProGamesApp";
        private static string gameRecordsFirebaseDocument = "gameRecords";
        private static string rowingGameCollection = "rowing";
        private static string personalBestDocument = "personalBests";
        private DocumentReference leaderboardRef;

        #endregion


        #region MONOBEHAVIOURS


        //------------------//
        private void Start()
        //-----------------//
        {
            localLeaderboard = FindObjectOfType<LocalLeaderboard>();

        } // END Start


        #endregion


        #region INITIALIZE USER DATA


        //----------------------------------//
        public void InitializeUserData(Action<DocumentSnapshot> OnSuccess, Action<string> OnError)
        //----------------------------------//
        {
            Debug.Log("AppManager//InitializeUserData// ");

#if !FIREBASE_STORAGE
            FirestoreDatabaseManager.Instance.CheckDependecies(() =>
            {
                if (FirestoreDatabaseManager.db == null)
                {
                    Debug.Log("AppManager//InitializeUserData// db null");
                }
                else
                {
                    Debug.Log("AppManager//InitializeUserData// db");
                }

                DocumentReference usersRef = FirestoreDatabaseManager.db.Collection("users").Document(AuthenticationManager.Instance.user.UserId);

                OnSuccess += (document) =>
                {
                    if (document.Exists == false)
                    {
                        Debug.Log("UserDataManager//InitializeUserData// dont have document");

                        FirstTimeUserUpload();
                    }
                    else
                    {
                        Debug.Log("UserDataManager//InitializeUserData// have document");

                        ReadUserDocument(document);
                    }
                };

                // Download the users data
                FirestoreDatabaseManager.Instance.GetDocument(usersRef, OnSuccess, OnError);

            });
#endif

        } // END InitializeUserData


        #endregion


        #region FIRST TIME USER UPLOAD


        //-----------------------------------//
        public static void FirstTimeUserUpload()
        //-----------------------------------//
        {
            string userJson = JsonUtility.ToJson(loadedData);

            Debug.Log("UserData//UpdateUserData//userJson " + userJson);

            UserDataMeta userMetadata = new UserDataMeta();
            userMetadata.birthday = UserEnteredData.GetBirthday();
            userMetadata.challenge = (Goal)UserEnteredData.challenge;
            userMetadata.country = UserEnteredData.country;
            userMetadata.creationTime = Timestamp.FromDateTime(DateTime.Now);
            userMetadata.displayName = UserEnteredData.displayName;
            userMetadata.email = UserEnteredData.email;
            userMetadata.firstName = UserEnteredData.firstName;
            userMetadata.firstTimeInApp = true;
            userMetadata.fitnessLevel = (FitnessLevel)UserEnteredData.fitnessLevel;
            userMetadata.gender = (Gender)UserEnteredData.gender;
            userMetadata.height = UserEnteredData.height;
            FeetInchContainer feetInchContainer = ConvertCentimetersToFeet(UserEnteredData.height);
            userMetadata.heightFeets = feetInchContainer.feet;
            userMetadata.heightInches = feetInchContainer.inches;
            userMetadata.imperialMeasurement = UserEnteredData.imperialMeasurement;
            userMetadata.instructor = "Shauna";
            userMetadata.isGrandFathered = false;
            userMetadata.lastLogin = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            userMetadata.lastName = UserEnteredData.lastName;
            userMetadata.nickname = UserEnteredData.nickname;
            userMetadata.photo = null;
            userMetadata.photoURL = null;
            userMetadata.photoUrl = null;
            userMetadata.uid = AuthenticationManager.Instance.user.UserId;
            userMetadata.weight = UserEnteredData.weight;
            userMetadata.weightLbs = (int)SystemOfMeasurementHelper.ConvertKilogramsToPounds(UserEnteredData.weight);

            UserDataManager.loadedData = userMetadata;

            Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "birthday", loadedData.birthday },
            { "challenge", loadedData.challenge },
            { "country", loadedData.country },
            { "creationTime", loadedData.creationTime },
            { "displayName", loadedData.displayName },
            { "email", loadedData.email },
            { "firstName", loadedData.firstName },
            { "firstTimeInApp", loadedData.firstTimeInApp },
            { "fitnessLevel", loadedData.fitnessLevel },
            { "gender", loadedData.gender },
            { "height", loadedData.height },
            { "heightFeets", loadedData.heightFeets },
            { "heightInches", loadedData.heightInches },
            { "imperialMeasurement", loadedData.imperialMeasurement },
            { "isGrandFathered", loadedData.isGrandFathered },
            { "lastLogin", loadedData.lastLogin },
            { "lastName", loadedData.lastName },
            { "nickname", loadedData.nickname },
            { "photo", loadedData.photo },
            { "photoURL", loadedData.photoURL },
            { "photoUrl", loadedData.photoUrl },
            { "uid", loadedData.uid },
            { "weight", loadedData.weight },
            { "weightLbs", loadedData.weightLbs }
        };

            DocumentReference documentReference = FirestoreDatabaseManager.db.Collection("users").Document(AuthenticationManager.Instance.user.UserId);

            FirestoreDatabaseManager.Instance.SaveDocument(documentReference, userData, OnUserDataInitialized,
            (error) =>
            {
                Debug.LogError("UserData//InitializeUserData// error " + error);
            });

        } // END FirstTimeUpload


        #endregion


        #region GET AIR RUNNER LEADERBOARD DATA


        //----------------------------------//
        public void GetAirRunnerLeaderboardData(Action<DocumentSnapshot> OnSuccess, Action<string> OnError)
        //----------------------------------//
        {
            Debug.Log("UserDataManager // GetAirRunnerLeaderboardData() // ");
#if AIR_RUNNER

#if !FIREBASE_STORAGE
            FirestoreDatabaseManager.Instance.CheckDependecies(() =>
            {
                if (FirestoreDatabaseManager.db == null)
                {
                    Debug.Log("UserDataManager // GetAirRunnerLeaderboardData() // db null");
                }
                else
                {
                    Debug.Log("UserDataManager // GetAirRunnerLeaderboardData() // Grabbed Leaderboard");
                }

                if (AirRunnerGameManager.selectedLevel.levelName == "Hills")
                {
                    DocumentReference _airRunnerLeaderboardDocumentRef = FirestoreDatabaseManager.db.Collection(mobileFirebaseCollection).Document(gameRecordsFirebaseDocument).Collection("airRunner").Document("scores").Collection("hills").Document("leaderboards").Collection("entry").Document("VSm5NhKPR4HSm9JXYNrF");

                    leaderboardRef = _airRunnerLeaderboardDocumentRef;
                }
                else
                {
                    //Debug.Log("Getting Dark Sky Leaderboard");
                    DocumentReference _airRunnerLeaderboardDocumentRef = FirestoreDatabaseManager.db.Collection(mobileFirebaseCollection).Document(gameRecordsFirebaseDocument).Collection("airRunner").Document("scores").Collection("hills").Document("leaderboards").Collection("entry").Document("VSm5NhKPR4HSm9JXYNrF");

                    leaderboardRef = _airRunnerLeaderboardDocumentRef;
                }

                OnSuccess += (document) =>
                {
                    if (document.Exists == false)
                    {
                        Debug.Log("UserDataManager//InitializeUserData// Dont have document, we are going to upload one using the current match data");
                        airRunnerLoadedData = AirRunnerGameManager.GetAirRunnerMetricsFromJson();
                        CompareAirRunnerData(airRunnerLoadedData, airRunnerLoadedData);
                    }
                    else
                    {
                        Debug.Log("UserDataManager //GetAirRunnerLeaderboardData() // We have document");
                        ReadAirRunnerDocument(document);
                    }
                };

                // Download the users data
                FirestoreDatabaseManager.Instance.GetDocument(leaderboardRef, OnSuccess, OnError);
                //Debug.Log("Downloaded Document");
#endif
            });
#endif

        } // END GetRowingLeaderboardData


#endregion


        #region GET FIT FIGHTER LEADERBOARD DATA


        //----------------------------------//
        public void GetFitFighterLeaderboardData(Action<DocumentSnapshot> OnSuccess, Action<string> OnError)
        //----------------------------------//
        {
            Debug.Log("UserDataManager // GetAirRunnerLeaderboardData() // ");

#if FIT_FIGHTER

#if !FIREBASE_STORAGE
            FirestoreDatabaseManager.Instance.CheckDependecies(() =>
            {
                if (FirestoreDatabaseManager.db == null)
                {
                    Debug.Log("UserDataManager // GetAirRunnerLeaderboardData() // db null");
                }
                else
                {
                    Debug.Log("UserDataManager // GetAirRunnerLeaderboardData() // Grabbed Leaderboard");
                }

                DocumentReference _fitFighterLeaderboardDocumentRef = FirestoreDatabaseManager.db.Collection(mobileFirebaseCollection).Document(gameRecordsFirebaseDocument).Collection("fitFighter").Document(AuthenticationManager.Instance.user.UserId);

                OnSuccess += (document) =>
                {
                    if (document.Exists == false)
                    {
                        Debug.Log("UserDataManager//GetFitFighterLeaderboardData() // Dont have document, we are going to upload one using the current match data");
                        fitFighterLoadedData = RhythmScoreSystem.GetFitFighterMetricsFromJson();
                        CompareFitFighterData(fitFighterLoadedData, fitFighterLoadedData);
                    }
                    else
                    {
                        Debug.Log("UserDataManager //GetFitFighterLeaderboardData() // We have document");
                        ReadFitFighterDocument(document);
                    }
                };

                // Download the users data
                FirestoreDatabaseManager.Instance.GetDocument(_fitFighterLeaderboardDocumentRef, OnSuccess, OnError);

            });
#endif
#endif
        } // END GetFitFighterLeaderboardData


#endregion


        #region GET ROWING LEADERBOARD DATA


        //----------------------------------//
        public void GetRowingLeaderboardData(Action<DocumentSnapshot> OnSuccess, Action<string> OnError)
        //----------------------------------//
        {
            Debug.Log("UserDataManager // GetLeaderboardData() // ");
#if ROWING_CANOE
#if !FIREBASE_STORAGE
            FirestoreDatabaseManager.Instance.CheckDependecies(() =>
            {
                if (FirestoreDatabaseManager.db == null)
                {
                    Debug.Log("UserDataManager // GetRowingLeaderboardData() // db null");
                }
                else
                {
                    Debug.Log("UserDataManager // GetRowingLeaderboardData() // Grabbed Leaderboard");
                }

                DocumentReference _leaderboardDocumentRef = FirestoreDatabaseManager.db.Collection(mobileFirebaseCollection).Document(gameRecordsFirebaseDocument).Collection(rowingGameCollection).Document(personalBestDocument).Collection("user").Document(AuthenticationManager.Instance.user.UserId);

                OnSuccess += (document) =>
                {
                    if (document.Exists == false)
                    {
                        Debug.Log("UserDataManager // GetRowingLeaderboardData() // Dont have document, we are going to upload one using the current match data");

                        rowingCanoeLoadedData = RowingTrackManager.GetRowingMetricsFromJson();
                        CompareRowingData(rowingCanoeLoadedData, rowingCanoeLoadedData);
                    }
                    else
                    {
                        Debug.Log("UserDataManager // GetRowingLeaderboardData() // We have document");
                        ReadRowingDocument(document);
                    }
                };

                // Download the users data
                FirestoreDatabaseManager.Instance.GetDocument(_leaderboardDocumentRef, OnSuccess, OnError);
#endif
            });
#endif

        } // END GetRowingLeaderboardData


#endregion


        #region GET ROWING LEADERBOARD ENTRIES


        //----------------------------//
        public async void GetRowingLeaderboardEntries(string _name = null, string _score = null)
        //----------------------------//
        {
            Debug.Log("UserDataManager.cs // GetRowingLeaderboardEntries() // ");

            Firebase.Firestore.Query _leaderboardQuery = FirestoreDatabaseManager.db.Collection(mobileFirebaseCollection).Document("gameLeaderboards").Collection("canoeRowing").Document("leaderboards").Collection("entry");

            if (localLeaderboard == null)
            {
                localLeaderboard = FindObjectOfType<LocalLeaderboard>();
            }

            await _leaderboardQuery.GetSnapshotAsync().ContinueWith(task =>
            {
                QuerySnapshot _leaderboardEntriesQuerySnapshot = task.Result;

                foreach (DocumentSnapshot _documentSnapshot in _leaderboardEntriesQuerySnapshot.Documents)
                {
                    //Debug.Log(String.Format("Document data for {0} document:", _documentSnapshot.Id));

                    Dictionary<string, object> leaderboardEntry = _documentSnapshot.ToDictionary();

                    leaderboardEntries = GetStringArrayValue(leaderboardEntry["scores"]);
                }
            });

            if (_name != null && _score != null)
            {
                LocalPlayerInfo info = new LocalPlayerInfo(_name, Convert.ToInt32(_score));
                localLeaderboard.collectedStats.Add(info);

                Debug.Log("Got new high score data");
            }

            for (int i = 0; i < leaderboardEntries.Length; i++)
            {
                //Assign The String To An Array And Split Using The Comma Character
                //This Will Remove The Comma From The String, And Leave Behind The Separated Name And Score
                string _statsSplit = leaderboardEntries.ToArray().GetValue(i).ToString();

                Debug.Log(_statsSplit);

                //RowingTrackManager.GetRowingMetricsFromJson();
                localLeaderboard.ReadData(_statsSplit);
            }

            string[] GetStringArrayValue(object _raw)
            {
                List<object> _objList = (List<object>)_raw;

                List<string> _stringList = new List<string>();

                foreach (object _obj in _objList)
                {
                    string _string = Convert.ToString(_obj);
                    _stringList.Add(_string);
                }

                string[] _stringArr = _stringList.ToArray();

                return _stringArr;
            }

        } // END GetRowingLeaderboardEntries


        #endregion


        #region GET FIT FIGHTER LEADERBOARD ENTRIES


        //----------------------------//
        public async void GetFitFighterLeaderboardEntries(string _name = null, string _score = null)
        //----------------------------//
        {
            Debug.Log("UserDataManager.cs // GetFitFighterLeaderboardEntries() // ");

            Firebase.Firestore.Query _leaderboardQuery = FirestoreDatabaseManager.db.Collection(mobileFirebaseCollection).Document("gameLeaderboards").Collection("fitFighter").Document("leaderboards").Collection("entry");

            if (localLeaderboard == null)
            {
                localLeaderboard = FindObjectOfType<LocalLeaderboard>();
            }

            await _leaderboardQuery.GetSnapshotAsync().ContinueWith(task =>
            {
                QuerySnapshot _leaderboardEntriesQuerySnapshot = task.Result;

                foreach (DocumentSnapshot _documentSnapshot in _leaderboardEntriesQuerySnapshot.Documents)
                {
                    //Debug.Log(String.Format("Document data for {0} document:", _documentSnapshot.Id));

                    Dictionary<string, object> leaderboardEntry = _documentSnapshot.ToDictionary();

                    leaderboardEntries = GetStringArrayValue(leaderboardEntry["scores"]);
                }
            });

            if (_name != null && _score != null)
            {
                LocalPlayerInfo info = new LocalPlayerInfo(_name, Convert.ToInt32(_score));
                localLeaderboard.collectedStats.Add(info);

                Debug.Log("Got new high score data");
            }

            for (int i = 0; i < leaderboardEntries.Length; i++)
            {
                //Assign The String To An Array And Split Using The Comma Character
                //This Will Remove The Comma From The String, And Leave Behind The Separated Name And Score
                string _statsSplit = leaderboardEntries.ToArray().GetValue(i).ToString();

                Debug.Log(_statsSplit);

                //RhythmScoreSystem.GetFitFighterMetricsFromJson();
                localLeaderboard.ReadData(_statsSplit);
            }

            //localLeaderboard.UpdateFitFighterToFirebase();

            string[] GetStringArrayValue(object _raw)
            {
                List<object> _objList = (List<object>)_raw;

                List<string> _stringList = new List<string>();

                foreach (object _obj in _objList)
                {
                    string _string = Convert.ToString(_obj);
                    _stringList.Add(_string);
                }

                string[] _stringArr = _stringList.ToArray();

                return _stringArr;
            }

        } // END GetHillsLeaderboardEntries


        #endregion


        #region GET HILLS LEADERBOARD ENTRIES


        //----------------------------//
        public async void GetHillsLeaderboardEntries(string _name = null, string _score = null)
        //----------------------------//
        {
            Debug.Log("UserDataManager.cs // GetHillsLeaderboardEntries() // ");

            Firebase.Firestore.Query _leaderboardQuery = FirestoreDatabaseManager.db.Collection(mobileFirebaseCollection).Document("gameLeaderboards").Collection("airRunner").Document("scores").Collection("hills").Document("leaderboards").Collection("entry");

            if (localLeaderboard == null)
            {
                localLeaderboard = FindObjectOfType<LocalLeaderboard>();
            }

            await _leaderboardQuery.GetSnapshotAsync().ContinueWith(task =>
            {
                QuerySnapshot _leaderboardEntriesQuerySnapshot = task.Result;

                foreach (DocumentSnapshot _documentSnapshot in _leaderboardEntriesQuerySnapshot.Documents)
                {
                    //Debug.Log(String.Format("Document data for {0} document:", _documentSnapshot.Id));

                    Dictionary<string, object> leaderboardEntry = _documentSnapshot.ToDictionary();

                    leaderboardEntries = GetStringArrayValue(leaderboardEntry["scores"]);
                }
            });

            if (_name != null && _score != null)
            {
                LocalPlayerInfo info = new LocalPlayerInfo(_name, Convert.ToInt32(_score));
                localLeaderboard.collectedStats.Add(info);

                Debug.Log("Got new high score data");
            }

            for (int i = 0; i < leaderboardEntries.Length; i++)
            {
                //Assign The String To An Array And Split Using The Comma Character
                //This Will Remove The Comma From The String, And Leave Behind The Separated Name And Score
                string _statsSplit = leaderboardEntries.ToArray().GetValue(i).ToString();

                Debug.Log(_statsSplit);

                localLeaderboard.ReadData(_statsSplit);
            }

            string[] GetStringArrayValue(object _raw)
            {
                List<object> _objList = (List<object>)_raw;

                List<string> _stringList = new List<string>();

                foreach (object _obj in _objList)
                {
                    string _string = Convert.ToString(_obj);
                    _stringList.Add(_string);
                }

                string[] _stringArr = _stringList.ToArray();

                return _stringArr;
            }

        } // END GetHillsLeaderboardEntries


        #endregion


        #region GET DARK SKY LEADERBOARD ENTRIES


        //----------------------------//
        public async void GetDarkSkyLeaderboardEntries(string _name = null, string _score = null)
        //----------------------------//
        {
            Debug.Log("UserDataManager.cs // GetDarkSkyLeaderboardEntries() // ");

            Firebase.Firestore.Query _leaderboardQuery = FirestoreDatabaseManager.db.Collection(mobileFirebaseCollection).Document("gameLeaderboards").Collection("airRunner").Document("scores").Collection("darkSky").Document("leaderboards").Collection("entry");

            if (localLeaderboard == null)
            {
                localLeaderboard = FindObjectOfType<LocalLeaderboard>();
            }

            await _leaderboardQuery.GetSnapshotAsync().ContinueWith(task =>
            {
                QuerySnapshot _leaderboardEntriesQuerySnapshot = task.Result;

                foreach (DocumentSnapshot _documentSnapshot in _leaderboardEntriesQuerySnapshot.Documents)
                {
                    //Debug.Log(String.Format("Document data for {0} document:", _documentSnapshot.Id));

                    Dictionary<string, object> leaderboardEntry = _documentSnapshot.ToDictionary();

                    leaderboardEntries = GetStringArrayValue(leaderboardEntry["scores"]);
                }
            });

            if (_name != null && _score != null)
            {
                LocalPlayerInfo info = new LocalPlayerInfo(_name, Convert.ToInt32(_score));
                localLeaderboard.collectedStats.Add(info);

                Debug.Log("Got new high score data");
            }

            for (int i = 0; i < leaderboardEntries.Length; i++)
            {
                //Assign The String To An Array And Split Using The Comma Character
                //This Will Remove The Comma From The String, And Leave Behind The Separated Name And Score
                string _statsSplit = leaderboardEntries.ToArray().GetValue(i).ToString();

                //Debug.Log(_statsSplit);

                localLeaderboard.ReadData(_statsSplit);
            }

            //localLeaderboard.UpdateAirRunnerDarkSkyToFirebase();

            string[] GetStringArrayValue(object _raw)
            {
                List<object> _objList = (List<object>)_raw;

                List<string> _stringList = new List<string>();

                foreach (object _obj in _objList)
                {
                    string _string = Convert.ToString(_obj);
                    _stringList.Add(_string);
                }

                string[] _stringArr = _stringList.ToArray();

                return _stringArr;
            }

        } // END GetHillsLeaderboardEntries


        #endregion


        #region GET DESERT LEADERBOARD ENTRIES


        //----------------------------//
        public async void GetDesertLeaderboardEntries(string _name = null, string _score = null)
        //----------------------------//
        {
            Debug.Log("UserDataManager.cs // GetDesertLeaderboardEntries() // ");

            Firebase.Firestore.Query _leaderboardQuery = FirestoreDatabaseManager.db.Collection(mobileFirebaseCollection).Document("gameLeaderboards").Collection("airRunner").Document("scores").Collection("desert").Document("leaderboards").Collection("entry");

            if (localLeaderboard == null)
            {
                localLeaderboard = FindObjectOfType<LocalLeaderboard>();
            }

            await _leaderboardQuery.GetSnapshotAsync().ContinueWith(task =>
            {
                QuerySnapshot _leaderboardEntriesQuerySnapshot = task.Result;

                foreach (DocumentSnapshot _documentSnapshot in _leaderboardEntriesQuerySnapshot.Documents)
                {
                    //Debug.Log(String.Format("Document data for {0} document:", _documentSnapshot.Id));

                    Dictionary<string, object> leaderboardEntry = _documentSnapshot.ToDictionary();

                    leaderboardEntries = GetStringArrayValue(leaderboardEntry["scores"]);
                }
            });

            if (_name != null && _score != null)
            {
                LocalPlayerInfo info = new LocalPlayerInfo(_name, Convert.ToInt32(_score));
                localLeaderboard.collectedStats.Add(info);

                Debug.Log("Got new high score data");
            }

            for (int i = 0; i < leaderboardEntries.Length; i++)
            {
                //Assign The String To An Array And Split Using The Comma Character
                //This Will Remove The Comma From The String, And Leave Behind The Separated Name And Score
                string _statsSplit = leaderboardEntries.ToArray().GetValue(i).ToString();

                //Debug.Log(_statsSplit);

                localLeaderboard.ReadData(_statsSplit);
            }

            //localLeaderboard.UpdateAirRunnerDesertToFirebase();

            string[] GetStringArrayValue(object _raw)
            {
                List<object> _objList = (List<object>)_raw;

                List<string> _stringList = new List<string>();

                foreach (object _obj in _objList)
                {
                    string _string = Convert.ToString(_obj);
                    _stringList.Add(_string);
                }

                string[] _stringArr = _stringList.ToArray();

                return _stringArr;
            }

        } // END GetHillsLeaderboardEntries


        #endregion


        #region GET FOREST LEADERBOARD ENTRIES


        //----------------------------//
        public async void GetForestLeaderboardEntries(string _name = null, string _score = null)
        //----------------------------//
        {
            Debug.Log("UserDataManager.cs // GetDesertLeaderboardEntries() // ");

            Firebase.Firestore.Query _leaderboardQuery = FirestoreDatabaseManager.db.Collection(mobileFirebaseCollection).Document("gameLeaderboards").Collection("airRunner").Document("scores").Collection("forest").Document("leaderboards").Collection("entry");

            if (localLeaderboard == null)
            {
                localLeaderboard = FindObjectOfType<LocalLeaderboard>();
            }

            await _leaderboardQuery.GetSnapshotAsync().ContinueWith(task =>
            {
                QuerySnapshot _leaderboardEntriesQuerySnapshot = task.Result;

                foreach (DocumentSnapshot _documentSnapshot in _leaderboardEntriesQuerySnapshot.Documents)
                {
                    //Debug.Log(String.Format("Document data for {0} document:", _documentSnapshot.Id));

                    Dictionary<string, object> leaderboardEntry = _documentSnapshot.ToDictionary();

                    leaderboardEntries = GetStringArrayValue(leaderboardEntry["scores"]);
                }
            });

            if (_name != null && _score != null)
            {
                LocalPlayerInfo info = new LocalPlayerInfo(_name, Convert.ToInt32(_score));
                localLeaderboard.collectedStats.Add(info);

                Debug.Log("Got new high score data");
            }

            for (int i = 0; i < leaderboardEntries.Length; i++)
            {
                //Assign The String To An Array And Split Using The Comma Character
                //This Will Remove The Comma From The String, And Leave Behind The Separated Name And Score
                string _statsSplit = leaderboardEntries.ToArray().GetValue(i).ToString();

                //Debug.Log(_statsSplit);

                localLeaderboard.ReadData(_statsSplit);
            }

            //localLeaderboard.UpdateAirRunnerForestToFirebase();

            string[] GetStringArrayValue(object _raw)
            {
                List<object> _objList = (List<object>)_raw;

                List<string> _stringList = new List<string>();

                foreach (object _obj in _objList)
                {
                    string _string = Convert.ToString(_obj);
                    _stringList.Add(_string);
                }

                string[] _stringArr = _stringList.ToArray();

                return _stringArr;
            }

        } // END GetHillsLeaderboardEntries


        #endregion


        #region UPDATE LEADERBOARD ENTRIES


        //----------------------------//
        public async void UpdateRowingLeaderboardEntries(string _name = null, string _score = null)
        //----------------------------//
        {
            Debug.Log("UserDataManager.cs // UpdateLeaderboardEntries() // ");

            Firebase.Firestore.Query _leaderboardQuery = FirestoreDatabaseManager.db.Collection(mobileFirebaseCollection).Document("gameLeaderboards").Collection("canoeRowing").Document("leaderboards").Collection("entry");

            if (localLeaderboard == null)
            {
                localLeaderboard = FindObjectOfType<LocalLeaderboard>();
            }

            await _leaderboardQuery.GetSnapshotAsync().ContinueWith(task =>
            {
                QuerySnapshot _leaderboardEntriesQuerySnapshot = task.Result;

                foreach (DocumentSnapshot _documentSnapshot in _leaderboardEntriesQuerySnapshot.Documents)
                {
                    //Debug.Log(String.Format("Document data for {0} document:", _documentSnapshot.Id));

                    Dictionary<string, object> leaderboardEntry = _documentSnapshot.ToDictionary();

                    leaderboardEntries = GetStringArrayValue(leaderboardEntry["scores"]);
                }
            });

            if (_name != null && _score != null)
            {
                LocalPlayerInfo info = new LocalPlayerInfo(_name, Convert.ToInt32(_score));
                localLeaderboard.collectedStats.Add(info);

                Debug.Log("Got new high score data");
            }

            for (int i = 0; i < leaderboardEntries.Length; i++)
            {
                //Assign The String To An Array And Split Using The Comma Character
                //This Will Remove The Comma From The String, And Leave Behind The Separated Name And Score
                string _statsSplit = leaderboardEntries.ToArray().GetValue(i).ToString();

                Debug.Log(_statsSplit);

                localLeaderboard.ReadData(_statsSplit);
            }

            string[] GetStringArrayValue(object _raw)
            {
                List<object> _objList = (List<object>)_raw;

                List<string> _stringList = new List<string>();

                foreach (object _obj in _objList)
                {
                    string _string = Convert.ToString(_obj);
                    _stringList.Add(_string);
                }

                string[] _stringArr = _stringList.ToArray();

                return _stringArr;
            }

        } // END GetRowingLeaderboardEntries


        //----------------------------//
        public async void UpdateFitFighterLeaderboardEntries(string _name = null, string _score = null)
        //----------------------------//
        {
            Debug.Log("UserDataManager.cs // GetRowingLeaderboardEntries() // ");

            Firebase.Firestore.Query _leaderboardQuery = FirestoreDatabaseManager.db.Collection(mobileFirebaseCollection).Document("gameLeaderboards").Collection("fitFighter").Document("leaderboards").Collection("entry");

            if (localLeaderboard == null)
            {
                localLeaderboard = FindObjectOfType<LocalLeaderboard>();
            }

            await _leaderboardQuery.GetSnapshotAsync().ContinueWith(task =>
            {
                QuerySnapshot _leaderboardEntriesQuerySnapshot = task.Result;

                foreach (DocumentSnapshot _documentSnapshot in _leaderboardEntriesQuerySnapshot.Documents)
                {
                    //Debug.Log(String.Format("Document data for {0} document:", _documentSnapshot.Id));

                    Dictionary<string, object> leaderboardEntry = _documentSnapshot.ToDictionary();

                    leaderboardEntries = GetStringArrayValue(leaderboardEntry["scores"]);
                }
            });

            if (_name != null && _score != null)
            {
                LocalPlayerInfo info = new LocalPlayerInfo(_name, Convert.ToInt32(_score));
                localLeaderboard.collectedStats.Add(info);

                Debug.Log("Got new high score data");
            }

            for (int i = 0; i < leaderboardEntries.Length; i++)
            {
                //Assign The String To An Array And Split Using The Comma Character
                //This Will Remove The Comma From The String, And Leave Behind The Separated Name And Score
                string _statsSplit = leaderboardEntries.ToArray().GetValue(i).ToString();

                Debug.Log(_statsSplit);

                localLeaderboard.ReadData(_statsSplit);
            }

            string[] GetStringArrayValue(object _raw)
            {
                List<object> _objList = (List<object>)_raw;

                List<string> _stringList = new List<string>();

                foreach (object _obj in _objList)
                {
                    string _string = Convert.ToString(_obj);
                    _stringList.Add(_string);
                }

                string[] _stringArr = _stringList.ToArray();

                return _stringArr;
            }

        } // END GetRowingLeaderboardEntries


        #endregion


        #region READ DOCUMENTS


        #region READ USER DOCUMENT


        //--------------------------------------------------//
        public void ReadUserDocument(DocumentSnapshot document)
        //--------------------------------------------------//
        {
            Dictionary<string, object> userDataDictionary = document.ToDictionary();

            UserDataMeta userMetadata = new UserDataMeta();
            userMetadata.birthday = (string)userDataDictionary["birthday"];
            userMetadata.challenge = (Goal)(Convert.ToInt32(userDataDictionary["challenge"]));
            userMetadata.country = (string)userDataDictionary["country"];
            userMetadata.creationTime = (Timestamp)userDataDictionary["creationTime"];
            userMetadata.displayName = (string)userDataDictionary["displayName"];
            userMetadata.email = (string)userDataDictionary["email"];
            userMetadata.firstName = (string)userDataDictionary["firstName"];
            userMetadata.firstTimeInApp = (bool)userDataDictionary["firstTimeInApp"];
            userMetadata.fitnessLevel = (FitnessLevel)(Convert.ToInt32(userDataDictionary["fitnessLevel"]));
            userMetadata.gender = (Gender)(Convert.ToInt32(userDataDictionary["gender"]));
            userMetadata.height = Convert.ToInt32(userDataDictionary["height"]);
            userMetadata.heightFeets = Convert.ToInt32(userDataDictionary["heightFeets"]);
            userMetadata.heightInches = Convert.ToInt32(userDataDictionary["heightInches"]);
            userMetadata.imperialMeasurement = (bool)(userDataDictionary["imperialMeasurement"]);
            userMetadata.instructor = (string)(userDataDictionary["instructor"]);
            userMetadata.isGrandFathered = (bool)(userDataDictionary["isGrandFathered"]);
            userMetadata.lastLogin = (string)(userDataDictionary["lastLogin"]);
            userMetadata.lastName = (string)(userDataDictionary["lastName"]);
            userMetadata.nickname = (string)(userDataDictionary["nickname"]);
            userMetadata.photo = (string)(userDataDictionary["photo"]);
            userMetadata.photoURL = (string)(userDataDictionary["photoURL"]);

            if (userDataDictionary.ContainsKey("photoUrl"))
                userMetadata.photoUrl = (string)(userDataDictionary["photoUrl"]);

            userMetadata.uid = (string)(userDataDictionary["uid"]);
            userMetadata.weight = Convert.ToInt32(userDataDictionary["weight"]);
            userMetadata.weightLbs = Convert.ToInt32(userDataDictionary["weightLbs"]);

            loadedData = userMetadata;

            OnUserDataInitialized?.Invoke();

            //Debug.Log("UserDataManager//ReadDocument// gender " + loadedData.gender.ToString());

        } // END ReadDocument


        #endregion


        #region READ AIR RUNNER DOCUMENT


        //--------------------------------------------------//
        private static void ReadAirRunnerDocument(DocumentSnapshot _document)
        //--------------------------------------------------//
        {
#if AIR_RUNNER
        Dictionary<string, object> _airRunnerDataDictionary = _document.ToDictionary();

        GameMetrics.AirRunnerGameMetrics _airRunnerMetrics = new GameMetrics.AirRunnerGameMetrics();

        _airRunnerMetrics.date = _airRunnerDataDictionary["date"].ToString();
        _airRunnerMetrics.bestScore = Convert.ToInt32(_airRunnerDataDictionary["bestScore"]);
        _airRunnerMetrics.currentScore = Convert.ToInt32(_airRunnerDataDictionary["currentScore"]);
        _airRunnerMetrics.ringsCollected = Convert.ToInt32(_airRunnerDataDictionary["ringsCollected"]);
        
        airRunnerLoadedData = _airRunnerMetrics;

        Debug.Log("UserDataManager.cs // ReadRowingDocument() // Grabbed Document from Firebase" );

        CompareAirRunnerData(AirRunnerGameManager.GetAirRunnerMetricsFromJson(), airRunnerLoadedData);
#endif
        } // END ReadRowingDocument


        #endregion


        #region READ FIT FIGHTER DOCUMENT


        //--------------------------------------------------//
        private static void ReadFitFighterDocument(DocumentSnapshot _document)
        //--------------------------------------------------//
        {
#if FIT_FIGHTER
        Dictionary<string, object> _fighterDataDictionary = _document.ToDictionary();

        GameMetrics.FitFighterGameMetrics _fighterMetrics= new GameMetrics.FitFighterGameMetrics();

        //_fighterMetrics.gradeMessage = _fighterDataDictionary["gradeMessage"].ToString();
        _fighterMetrics.date = _fighterDataDictionary["date"].ToString();
        _fighterMetrics.timeToFinish = _fighterDataDictionary["timeToFinish"].ToString();
        _fighterMetrics.workByRepetitions = GetArrayValue(_fighterDataDictionary["workByRepetitions"]);
        _fighterMetrics.powerList = GetArrayValue(_fighterDataDictionary["powerList"]);
        _fighterMetrics.repetitionsPerRound = GetArrayValue(_fighterDataDictionary["repetitionsPerRound"]);
        _fighterMetrics.roundsWon = Convert.ToInt32(_fighterDataDictionary["roundsWon"]);
        _fighterMetrics.repetitionsExecuted = Convert.ToInt32(_fighterDataDictionary["repetitionsExecuted"]);
        _fighterMetrics.extraRepetitions = Convert.ToInt32(_fighterDataDictionary["extraRepetitions"]);
        _fighterMetrics.score = Convert.ToInt32(_fighterDataDictionary["score"]);
        _fighterMetrics.matchDuration = Convert.ToInt32(_fighterDataDictionary["matchDuration"]);
        _fighterMetrics.currentRound = Convert.ToInt32(_fighterDataDictionary["currentRound"]);
        _fighterMetrics.peakWork = Convert.ToInt32(_fighterDataDictionary["peakWork"]);
        _fighterMetrics.totalWork = Convert.ToInt32(_fighterDataDictionary["totalWork"]);
        _fighterMetrics.repetitions = Convert.ToInt32(_fighterDataDictionary["repetitions"]);
        _fighterMetrics.averageWork = Convert.ToInt32(_fighterDataDictionary["averagWork"]);
        _fighterMetrics.caloriesBurned = Convert.ToInt32(_fighterDataDictionary["caloriesBurned"]);

        fitFighterLoadedData = _fighterMetrics;

        Debug.Log("UserDataManager.cs // ReadFitFighterDocument() // Grabbed Document from Firebase");

        int[] GetArrayValue(object _raw)
        {
            List<object> _objList = (List<object>)_raw;

            List<int> _intList = new List<int>();

            foreach (object _obj in _objList)
            {
                int _int = Convert.ToInt32(_obj);
                _intList.Add(_int);
            }

            int[] _intArr = _intList.ToArray();

            return _intArr;
        }

        CompareFitFighterData(RhythmScoreSystem.GetFitFighterMetricsFromJson(), fitFighterLoadedData);
#endif
        } // END ReadRowingDocument


        #endregion


        #region READ ROWING DOCUMENT


        //--------------------------------------------------//
        private static void ReadRowingDocument(DocumentSnapshot _document)
        //--------------------------------------------------//
        {
#if ROWING_CANOE
        Dictionary<string, object> _rowingDataDictionary = _document.ToDictionary();

        GameMetrics.RowingCanoeGameMetrics _rowingMetrics = new GameMetrics.RowingCanoeGameMetrics();

        _rowingMetrics.date = _rowingDataDictionary["date"].ToString();
        _rowingMetrics.timeToFinish = _rowingDataDictionary["timeToFinish"].ToString();
        _rowingMetrics.workByRepetitions = GetArrayValue(_rowingDataDictionary["workByRepetitions"]);
        _rowingMetrics.powerList = GetArrayValue(_rowingDataDictionary["powerList"]);
        _rowingMetrics.caloriesBurned = Convert.ToInt32(_rowingDataDictionary["caloriesBurned"]);
        _rowingMetrics.averageWork = Convert.ToInt32(_rowingDataDictionary["averageWork"]);
        _rowingMetrics.repetitions = Convert.ToInt32(_rowingDataDictionary["repetitions"]);
        _rowingMetrics.highScore = Convert.ToInt32(_rowingDataDictionary["highScore"]);
        _rowingMetrics.strokes = Convert.ToInt32(_rowingDataDictionary["strokes"]);
        _rowingMetrics.averageSpeed = Convert.ToDouble(_rowingDataDictionary["averageSpeed"]);
        _rowingMetrics.paceMinutes = Convert.ToDouble(_rowingDataDictionary["paceMinutes"]);
        _rowingMetrics.paceSeconds = Convert.ToDouble(_rowingDataDictionary["paceSeconds"]);
        _rowingMetrics.maxSpeed = Convert.ToDouble(_rowingDataDictionary["maxSpeed"]);
        _rowingMetrics.cadence = Convert.ToDouble(_rowingDataDictionary["cadence"]);
        _rowingMetrics.leaderboardScore = GetStringArrayValue(_rowingDataDictionary["leaderboardScore"]);

        rowingCanoeLoadedData = _rowingMetrics;

        Debug.Log("UserDataManager.cs // ReadRowingDocument() // Grabbed Document from Firebase");

        int[] GetArrayValue(object _raw)
        {
            List<object> _objList = (List<object>)_raw;

            List<int> _intList = new List<int>();

            foreach (object _obj in _objList)
            {
                int _int = Convert.ToInt32(_obj);
                _intList.Add(_int);
            }

            int[] _intArr = _intList.ToArray();

            return _intArr;
        }


        string[] GetStringArrayValue(object _raw)
        {
            List<object> _objList = (List<object>)_raw;

            List<string> _stringList = new List<string>();

            foreach (object _obj in _objList)
            {
                string _string = Convert.ToString(_obj);
                _stringList.Add(_string);
            }

            string[] _stringArr = _stringList.ToArray();

            return _stringArr;
        }

        CompareRowingData(RowingTrackManager.GetRowingMetricsFromJson(), rowingCanoeLoadedData);
#endif
        } // END ReadRowingDocument


        #endregion


        #endregion


        //----------------------------//
        public async void UpdateCanoeLeaderboard()
        //----------------------------//
        {
            Debug.Log("UserDataManager.cs // UpdateCanoeLeaderboard() // ");

            Firebase.Firestore.Query _leaderboardQuery = FirestoreDatabaseManager.db.Collection(mobileFirebaseCollection).Document("gameLeaderboards").Collection("canoeRowing").Document("leaderboards").Collection("entry");

            if (localLeaderboard == null)
            {
                localLeaderboard = FindObjectOfType<LocalLeaderboard>();
            }

            await _leaderboardQuery.GetSnapshotAsync().ContinueWith(task =>
            {
                QuerySnapshot _leaderboardEntriesQuerySnapshot = task.Result;

                foreach (DocumentSnapshot _documentSnapshot in _leaderboardEntriesQuerySnapshot.Documents)
                {
                    //Debug.Log(String.Format("Document data for {0} document:", _documentSnapshot.Id));

                    Dictionary<string, object> leaderboardEntry = _documentSnapshot.ToDictionary();

                    leaderboardEntries = GetStringArrayValue(leaderboardEntry["scores"]);
                }
            });

            for (int i = 0; i < leaderboardEntries.Length; i++)
            {
                //Assign The String To An Array And Split Using The Comma Character
                //This Will Remove The Comma From The String, And Leave Behind The Separated Name And Score
                string _statsSplit = leaderboardEntries.ToArray().GetValue(i).ToString();
                /*
                if(_statsSplit == "")
                {
                    Debug.Log("Returned because of null string");
                    return;
                }
                */
                Debug.Log(_statsSplit);

                localLeaderboard.ReadData(_statsSplit);
            }

            localLeaderboard.UpdateCanoeToFirebase();

            string[] GetStringArrayValue(object _raw)
            {
                List<object> _objList = (List<object>)_raw;

                List<string> _stringList = new List<string>();

                foreach (object _obj in _objList)
                {
                    string _string = Convert.ToString(_obj);
                    _stringList.Add(_string);
                }

                string[] _stringArr = _stringList.ToArray();

                return _stringArr;
            }

        } // END GetHillsLeaderboardEntries


        //----------------------------//
        public async void UpdateFitFighterLeaderboard()
        //----------------------------//
        {
            Debug.Log("UserDataManager.cs // UpdateCanoeLeaderboard() // ");

            Firebase.Firestore.Query _leaderboardQuery = FirestoreDatabaseManager.db.Collection(mobileFirebaseCollection).Document("gameLeaderboards").Collection("fitFighter").Document("leaderboards").Collection("entry");

            if (localLeaderboard == null)
            {
                localLeaderboard = FindObjectOfType<LocalLeaderboard>();
            }

            await _leaderboardQuery.GetSnapshotAsync().ContinueWith(task =>
            {
                QuerySnapshot _leaderboardEntriesQuerySnapshot = task.Result;

                foreach (DocumentSnapshot _documentSnapshot in _leaderboardEntriesQuerySnapshot.Documents)
                {
                    //Debug.Log(String.Format("Document data for {0} document:", _documentSnapshot.Id));

                    Dictionary<string, object> leaderboardEntry = _documentSnapshot.ToDictionary();

                    leaderboardEntries = GetStringArrayValue(leaderboardEntry["scores"]);
                }
            });

            for (int i = 0; i < leaderboardEntries.Length; i++)
            {
                //Assign The String To An Array And Split Using The Comma Character
                //This Will Remove The Comma From The String, And Leave Behind The Separated Name And Score
                string _statsSplit = leaderboardEntries.ToArray().GetValue(i).ToString();
                /*
                if(_statsSplit == "")
                {
                    Debug.Log("Returned because of null string");
                    return;
                }
                */
                Debug.Log(_statsSplit);

                localLeaderboard.ReadData(_statsSplit);
            }

            localLeaderboard.UpdateFitFighterToFirebase();

            string[] GetStringArrayValue(object _raw)
            {
                List<object> _objList = (List<object>)_raw;

                List<string> _stringList = new List<string>();

                foreach (object _obj in _objList)
                {
                    string _string = Convert.ToString(_obj);
                    _stringList.Add(_string);
                }

                string[] _stringArr = _stringList.ToArray();

                return _stringArr;
            }

        } // END GetHillsLeaderboardEntries


        //----------------------------//
        public async void UpdateHillsLeaderboard()
        //----------------------------//
        {
            Debug.Log("UserDataManager.cs // UpdateHillsLeaderboardEntries() // ");

            Firebase.Firestore.Query _leaderboardQuery = FirestoreDatabaseManager.db.Collection(mobileFirebaseCollection).Document("gameLeaderboards").Collection("airRunner").Document("scores").Collection("hills").Document("leaderboards").Collection("entry");

            if (localLeaderboard == null)
            {
                localLeaderboard = FindObjectOfType<LocalLeaderboard>();
            }

            await _leaderboardQuery.GetSnapshotAsync().ContinueWith(task =>
            {
                QuerySnapshot _leaderboardEntriesQuerySnapshot = task.Result;

                foreach (DocumentSnapshot _documentSnapshot in _leaderboardEntriesQuerySnapshot.Documents)
                {
                    //Debug.Log(String.Format("Document data for {0} document:", _documentSnapshot.Id));

                    Dictionary<string, object> leaderboardEntry = _documentSnapshot.ToDictionary();

                    leaderboardEntries = GetStringArrayValue(leaderboardEntry["scores"]);
                }
            });

            for (int i = 0; i < leaderboardEntries.Length; i++)
            {
                //Assign The String To An Array And Split Using The Comma Character
                //This Will Remove The Comma From The String, And Leave Behind The Separated Name And Score
                string _statsSplit = leaderboardEntries.ToArray().GetValue(i).ToString();

                Debug.Log(_statsSplit);

                localLeaderboard.ReadData(_statsSplit);
            }

            localLeaderboard.UpdateAirRunnerHillsToFirebase();

            string[] GetStringArrayValue(object _raw)
            {
                List<object> _objList = (List<object>)_raw;

                List<string> _stringList = new List<string>();

                foreach (object _obj in _objList)
                {
                    string _string = Convert.ToString(_obj);
                    _stringList.Add(_string);
                }

                string[] _stringArr = _stringList.ToArray();

                return _stringArr;
            }

        } // END GetHillsLeaderboardEntries


        //----------------------------//
        public async void UpdateDarkSkyLeaderboard()
        //----------------------------//
        {
            Debug.Log("UserDataManager.cs // UpdateDarkSkyLeaderboard() // ");

            Firebase.Firestore.Query _leaderboardQuery = FirestoreDatabaseManager.db.Collection(mobileFirebaseCollection).Document("gameLeaderboards").Collection("airRunner").Document("scores").Collection("darkSky").Document("leaderboards").Collection("entry");

            if (localLeaderboard == null)
            {
                localLeaderboard = FindObjectOfType<LocalLeaderboard>();
            }

            await _leaderboardQuery.GetSnapshotAsync().ContinueWith(task =>
            {
                QuerySnapshot _leaderboardEntriesQuerySnapshot = task.Result;

                foreach (DocumentSnapshot _documentSnapshot in _leaderboardEntriesQuerySnapshot.Documents)
                {
                    //Debug.Log(String.Format("Document data for {0} document:", _documentSnapshot.Id));

                    Dictionary<string, object> leaderboardEntry = _documentSnapshot.ToDictionary();

                    leaderboardEntries = GetStringArrayValue(leaderboardEntry["scores"]);
                }
            });

            for (int i = 0; i < leaderboardEntries.Length; i++)
            {
                //Assign The String To An Array And Split Using The Comma Character
                //This Will Remove The Comma From The String, And Leave Behind The Separated Name And Score
                string _statsSplit = leaderboardEntries.ToArray().GetValue(i).ToString();

                Debug.Log(_statsSplit);

                localLeaderboard.ReadData(_statsSplit);
            }

            localLeaderboard.UpdateAirRunnerDarkSkyToFirebase();

            string[] GetStringArrayValue(object _raw)
            {
                List<object> _objList = (List<object>)_raw;

                List<string> _stringList = new List<string>();

                foreach (object _obj in _objList)
                {
                    string _string = Convert.ToString(_obj);
                    _stringList.Add(_string);
                }

                string[] _stringArr = _stringList.ToArray();

                return _stringArr;
            }

        } // END GetHillsLeaderboardEntries


        //----------------------------//
        public async void UpdateDesertLeaderboard()
        //----------------------------//
        {
            Debug.Log("UserDataManager.cs // UpdateDesertLeaderboard() // ");

            Firebase.Firestore.Query _leaderboardQuery = FirestoreDatabaseManager.db.Collection(mobileFirebaseCollection).Document("gameLeaderboards").Collection("airRunner").Document("scores").Collection("desert").Document("leaderboards").Collection("entry");

            if (localLeaderboard == null)
            {
                localLeaderboard = FindObjectOfType<LocalLeaderboard>();
            }

            await _leaderboardQuery.GetSnapshotAsync().ContinueWith(task =>
            {
                QuerySnapshot _leaderboardEntriesQuerySnapshot = task.Result;

                foreach (DocumentSnapshot _documentSnapshot in _leaderboardEntriesQuerySnapshot.Documents)
                {
                    //Debug.Log(String.Format("Document data for {0} document:", _documentSnapshot.Id));

                    Dictionary<string, object> leaderboardEntry = _documentSnapshot.ToDictionary();

                    leaderboardEntries = GetStringArrayValue(leaderboardEntry["scores"]);
                }
            });

            for (int i = 0; i < leaderboardEntries.Length; i++)
            {
                //Assign The String To An Array And Split Using The Comma Character
                //This Will Remove The Comma From The String, And Leave Behind The Separated Name And Score
                string _statsSplit = leaderboardEntries.ToArray().GetValue(i).ToString();
                /*
                if(_statsSplit == "")
                {
                    Debug.Log("Returned because of null string");
                    return;
                }
                */
                Debug.Log(_statsSplit);

                localLeaderboard.ReadData(_statsSplit);
            }

            localLeaderboard.UpdateAirRunnerDesertToFirebase();

            string[] GetStringArrayValue(object _raw)
            {
                List<object> _objList = (List<object>)_raw;

                List<string> _stringList = new List<string>();

                foreach (object _obj in _objList)
                {
                    string _string = Convert.ToString(_obj);
                    _stringList.Add(_string);
                }

                string[] _stringArr = _stringList.ToArray();

                return _stringArr;
            }

        } // END GetHillsLeaderboardEntries


        //----------------------------//
        public async void UpdateForestLeaderboard()
        //----------------------------//
        {
            Debug.Log("UserDataManager.cs // UpdateForestLeaderboard() // ");

            Firebase.Firestore.Query _leaderboardQuery = FirestoreDatabaseManager.db.Collection(mobileFirebaseCollection).Document("gameLeaderboards").Collection("airRunner").Document("scores").Collection("forest").Document("leaderboards").Collection("entry");

            if (localLeaderboard == null)
            {
                localLeaderboard = FindObjectOfType<LocalLeaderboard>();
            }

            await _leaderboardQuery.GetSnapshotAsync().ContinueWith(task =>
            {
                QuerySnapshot _leaderboardEntriesQuerySnapshot = task.Result;

                foreach (DocumentSnapshot _documentSnapshot in _leaderboardEntriesQuerySnapshot.Documents)
                {
                    //Debug.Log(String.Format("Document data for {0} document:", _documentSnapshot.Id));

                    Dictionary<string, object> leaderboardEntry = _documentSnapshot.ToDictionary();

                    leaderboardEntries = GetStringArrayValue(leaderboardEntry["scores"]);
                }
            });

            for (int i = 0; i < leaderboardEntries.Length; i++)
            {
                //Assign The String To An Array And Split Using The Comma Character
                //This Will Remove The Comma From The String, And Leave Behind The Separated Name And Score
                string _statsSplit = leaderboardEntries.ToArray().GetValue(i).ToString();
                /*
                if(_statsSplit == "")
                {
                    Debug.Log("Returned because of null string");
                    return;
                }
                */
                Debug.Log(_statsSplit);

                localLeaderboard.ReadData(_statsSplit);
            }

            localLeaderboard.UpdateAirRunnerForestToFirebase();

            string[] GetStringArrayValue(object _raw)
            {
                List<object> _objList = (List<object>)_raw;

                List<string> _stringList = new List<string>();

                foreach (object _obj in _objList)
                {
                    string _string = Convert.ToString(_obj);
                    _stringList.Add(_string);
                }

                string[] _stringArr = _stringList.ToArray();

                return _stringArr;
            }

        } // END GetHillsLeaderboardEntries


        #region COMPARE DATA


        #region COMPARE AIRRUNNER DATA


        //--------------------------------------------------------//
        private static void CompareAirRunnerData(GameMetrics.AirRunnerGameMetrics _localJsonMetrics, GameMetrics.AirRunnerGameMetrics _firebaseMetrics)
        //--------------------------------------------------------//
        {
#if AIR_RUNNER
        Debug.Log("UserDataManager CompareAirRunnerData.cs // Comparing Data to see if we should update");

        bool hasNewValuesToUpload = false;

        // If we have a higher score, then update Firebase
        if (_localJsonMetrics.highScore >= _firebaseMetrics.highScore)
        {
            hasNewValuesToUpload = true;            
            UpdateAirRunnerMetrics();
        }
        
        if (hasNewValuesToUpload == true)
        {
            Debug.Log("UserDataManager CompareData.cs // We have a change in scores, so upload a new document to Firebase");

            if (_localJsonMetrics.game == GameMetrics.AirRunnerGameMetrics.Game.Hills)
            {
                Debug.Log("UserDataManager CompareAirRunnerData.cs // Updating Hills");
                UserDataManager.Instance.UpdateHillsLeaderboard();
            }
            else if (_localJsonMetrics.game == GameMetrics.AirRunnerGameMetrics.Game.DarkSky)
            {
                Debug.Log("UserDataManager CompareAirRunnerData.cs // Updating DarkSky");
                UserDataManager.Instance.UpdateDarkSkyLeaderboard();
            }
            else if (_localJsonMetrics.game == GameMetrics.AirRunnerGameMetrics.Game.Desert)
            {
                Debug.Log("UserDataManager CompareAirRunnerData.cs // Updating Desert");
                UserDataManager.Instance.UpdateDesertLeaderboard();
            }
            else if (_localJsonMetrics.game == GameMetrics.AirRunnerGameMetrics.Game.Forest)
            {
                Debug.Log("UserDataManager CompareAirRunnerData.cs // Updating Forest");
                UserDataManager.Instance.UpdateForestLeaderboard();
            }

        }
        else
        {
            Debug.Log("UserDataManager CompareData.cs // No data to update");
            return;
        }

        void UpdateAirRunnerMetrics()
        {
            airRunnerLoadedData.date = _localJsonMetrics.date;
            airRunnerLoadedData.longestStreak = _localJsonMetrics.longestStreak;
            airRunnerLoadedData.highScore = _localJsonMetrics.highScore;
            airRunnerLoadedData.highestDifficultyLevel = _localJsonMetrics.highestDifficultyLevel;
            airRunnerLoadedData.bestScore = _localJsonMetrics.bestScore;
            airRunnerLoadedData.currentScore = _localJsonMetrics.currentScore;
            airRunnerLoadedData.ringsCollected = _localJsonMetrics.ringsCollected;

        } // END UpdateRowingMetrics

#endif
        } // END CompareAirRunnerData


        #endregion


        #region COMPARE ROWING DATA


        //--------------------------------------------------------//
        private static void CompareRowingData(GameMetrics.RowingCanoeGameMetrics _localJsonMetrics, GameMetrics.RowingCanoeGameMetrics _firebaseMetrics)
        //--------------------------------------------------------//
        {
            Debug.Log("UserDataManager CompareRowingData.cs // Comparing Data to see if we should update");
#if ROWING_CANOE
        bool hasNewValuesToUpload = false;
        //UpdateRowingMetrics();
        //UserDataManager.Instance.localLeaderboard.UpdateCanoeToFirebase();

        // If we have a higher score, then update Firebase
        if (_localJsonMetrics.highScore >= _firebaseMetrics.highScore)
        {
            // Check if we belong in leaderboard
            // If we do, then check where we belong in that list
            hasNewValuesToUpload = true;
            UpdateRowingMetrics();
        }

        if (hasNewValuesToUpload == true)
        {
            Debug.Log("UserDataManager CompareData.cs // We have a change in scores, so upload a new document to Firebase");
            UserDataManager.Instance.UpdateCanoeLeaderboard();
        }
        else
        {
            Debug.Log("UserDataManager CompareData.cs // No data to update");
            return;
        }

        void UpdateRowingMetrics()
        {
            rowingCanoeLoadedData.date = _localJsonMetrics.date;
            rowingCanoeLoadedData.timeToFinish = _localJsonMetrics.timeToFinish;
            rowingCanoeLoadedData.highScore = _localJsonMetrics.highScore;
            rowingCanoeLoadedData.strokes = _localJsonMetrics.strokes;
            rowingCanoeLoadedData.averageWork = _localJsonMetrics.averageWork;
            rowingCanoeLoadedData.totalWork = _localJsonMetrics.totalWork;
            rowingCanoeLoadedData.peakWork = _localJsonMetrics.peakWork;
            rowingCanoeLoadedData.caloriesBurned = _localJsonMetrics.caloriesBurned;
            rowingCanoeLoadedData.repetitions = _localJsonMetrics.repetitions;
            rowingCanoeLoadedData.maxSpeed = _localJsonMetrics.maxSpeed;
            rowingCanoeLoadedData.averageSpeed = _localJsonMetrics.averageSpeed;
            rowingCanoeLoadedData.paceMinutes = _localJsonMetrics.paceMinutes;
            rowingCanoeLoadedData.paceSeconds = _localJsonMetrics.paceSeconds;
            rowingCanoeLoadedData.cadence = _localJsonMetrics.cadence;
            rowingCanoeLoadedData.workByRepetitions = _localJsonMetrics.workByRepetitions;
            rowingCanoeLoadedData.powerList = _localJsonMetrics.powerList;
            rowingCanoeLoadedData.leaderboardScore = _localJsonMetrics.leaderboardScore;

        } // END UpdateRowingMetrics
#endif
        } // END CompareRowingData


        #endregion


        #region COMPARE FIT FIGHTER DATA


        //--------------------------------------------------------//
        private static void CompareFitFighterData(GameMetrics.FitFighterGameMetrics _localJsonMetrics, GameMetrics.FitFighterGameMetrics _firebaseMetrics)
        //--------------------------------------------------------//
        {
            Debug.Log("UserDataManager CompareRowingData.cs // Comparing Data to see if we should update");

#if FIT_FIGHTER
        bool _hasNewValuesToUpload = false;

        // If we have a higher score, then update Firebase
        if (_localJsonMetrics.score >= _firebaseMetrics.score)
        {
            _hasNewValuesToUpload = true;
            UpdateFighterMetrics();
        }
        
        if (_hasNewValuesToUpload == true)
        {
            Debug.Log("UserDataManager CompareData.cs // We have a change in scores, so upload a new document to Firebase");
            //UserDataManager.UpdateFitFighterDocument();
            UserDataManager.Instance.UpdateFitFighterLeaderboard();
        }
        else
        {
            Debug.Log("UserDataManager CompareData.cs // No data to update");
            return;
        }

        void UpdateFighterMetrics()
        {
            fitFighterLoadedData.gradeMessage = _localJsonMetrics.gradeMessage;
            fitFighterLoadedData.date = _localJsonMetrics.date;
            fitFighterLoadedData.roundsWon = _localJsonMetrics.roundsWon;
            fitFighterLoadedData.repetitionsExecuted = _localJsonMetrics.repetitionsExecuted;
            fitFighterLoadedData.extraRepetitions = _localJsonMetrics.extraRepetitions;
            fitFighterLoadedData.score = _localJsonMetrics.score;
            fitFighterLoadedData.matchDuration = _localJsonMetrics.matchDuration;
            fitFighterLoadedData.currentRound = _localJsonMetrics.currentRound;
            fitFighterLoadedData.peakWork = _localJsonMetrics.peakWork;
            fitFighterLoadedData.totalWork = _localJsonMetrics.totalWork;
            fitFighterLoadedData.repetitions = _localJsonMetrics.repetitions;
            fitFighterLoadedData.averageWork = _localJsonMetrics.averageWork;
            fitFighterLoadedData.caloriesBurned = _localJsonMetrics.caloriesBurned;
            fitFighterLoadedData.workByRepetitions = _localJsonMetrics.workByRepetitions;
            fitFighterLoadedData.powerList = _localJsonMetrics.powerList;
            fitFighterLoadedData.repetitionsPerRound = _localJsonMetrics.repetitionsPerRound;

        } // END UpdateFighterMetrics
#endif

        } // END CompareFitFighterData


        #endregion


        #endregion


        #region RECIEVE DATA


        #region RECIEVE AIR RUNNER DATA


        //-------------------------------------//
        public void RecieveAirRunnerData()
        //-------------------------------------//
        {
#if AIR_RUNNER
        if (airRunnerLoadedData == null)
        {
            //Grab User Data
            GetAirRunnerLeaderboardData((snapShot) =>
            {
                Debug.Log("UserProfilePage.cs // ReceiveAirRunnerData() // Successfully got data");
            }, (error) =>
            {
                Debug.Log("UserProfilePage.cs // ReceiveAirRunnerData() // ERROR: Was not able to initialize user data");
            });
        }
        else
        {
            Debug.Log("UserDataManager.cs // ReceiveAirRunnerData() // we already have loaded data.  Comparing Data");
            CompareAirRunnerData(AirRunnerGameManager.GetAirRunnerMetricsFromJson(), airRunnerLoadedData);
        }
#endif
        }//END ReceiveRowingData

        #endregion


        #region RECIEVE FIT FIGHTER DATA


        //-------------------------------------//
        public void RecieveFitFighterData()
        //-------------------------------------//
        {
#if FIT_FIGHTER
        if (airRunnerLoadedData == null)
        {
            //Grab User Data
            GetFitFighterLeaderboardData((snapShot) =>
            {
                Debug.Log("UserProfilePage.cs // ReceiveFitFighterData() // Successfully got data");
            }, (error) =>
            {
                Debug.Log("UserProfilePage.cs // ReceiveFitFighterData() // ERROR: Was not able to initialize user data");
            });
        }
        else
        {
            Debug.Log("UserDataManager.cs // ReceiveFitFighterData() // we already have loaded data.  Comparing Data");
            CompareFitFighterData(RhythmScoreSystem.GetFitFighterMetricsFromJson(), fitFighterLoadedData);
        }
#endif
        }//END ReceiveRowingData

        #endregion


        #region RECIEVE ROWING DATA


        //-------------------------------------//
        public void RecieveRowingData()
        //-------------------------------------//
        {
#if ROWING_CANOE
        if (rowingCanoeLoadedData == null)
        {
            // Grab User Data
            GetRowingLeaderboardData((snapShot) =>
            {
                Debug.Log("UserProfilePage.cs // ReceiveProfileData() // Successfully got data");
            }, (error) =>
            {
                Debug.Log("UserProfilePage.cs // ReceiveProfileData() // ERROR: Was not able to initialize user data");
            });
        }
        else
        {
            CompareRowingData(RowingTrackManager.GetRowingMetricsFromJson(), rowingCanoeLoadedData);
            Debug.Log("UserDataManager.cs // SetRowingData() // we already have loaded data.  Comparing Data");
        }
#endif
        } // END ReceiveRowingData


        #endregion


        #endregion


        #region UPDATE MOBILE GAME METRICS DOCUMENTS


        #region UPDATE AIRRUNNER DOCUMENT


        //---------------------------------//
        private static void UpdateAirRunnerDocument()
        //---------------------------------//
        {
#if AIR_RUNNER
        Debug.Log("UserDataManager.cs // UploadAirRunnerDocument() //  Starting upload");

        Dictionary<string, object> _airRunnerData = new Dictionary<string, object>
        {
            { "longestStreak", airRunnerLoadedData.longestStreak },
            { "highScore", airRunnerLoadedData.highScore },
            { "highestDifficultyLevel", airRunnerLoadedData.highestDifficultyLevel },
            { "bestScore", airRunnerLoadedData.bestScore },
            { "currentScore", airRunnerLoadedData.currentScore },
            { "ringsCollected", airRunnerLoadedData.ringsCollected },
        };

        DocumentReference _documentReference = FirestoreDatabaseManager.db.Collection(mobileFirebaseCollection).Document(gameRecordsFirebaseDocument).Collection("airRunner").Document(AuthenticationManager.Instance.user.UserId);

        FirestoreDatabaseManager.Instance.SaveDocument(_documentReference, _airRunnerData, OnUserDataInitialized,
            (error) =>
            {
                Debug.LogError("UserData//InitializeUserData// error " + error);
            });
#endif
        } // END UploadAirRunnerDocument


        #endregion


        #region UPDATE FIT FIGHTER DOCUMENT


        //---------------------------------//
        public static void UpdateFitFighterDocument()
        //---------------------------------//
        {
            Debug.Log("UserDataManager.cs // UploadFitFighterDocument() //  Starting upload");
#if FIT_FIGHTER
        Dictionary<string, object> _fitFighterData = new Dictionary<string, object>
        {
            { "roundsWon", fitFighterLoadedData.roundsWon },
            { "repetitionsExecuted", fitFighterLoadedData.repetitionsExecuted },
            { "extraRepetitions", fitFighterLoadedData.extraRepetitions },
            { "score", fitFighterLoadedData.score },
            { "matchDuration", fitFighterLoadedData.matchDuration },
            { "currentRound", fitFighterLoadedData.currentRound },
            { "peakWork", fitFighterLoadedData.peakWork },
            { "totalWork", fitFighterLoadedData.totalWork },
            { "repetitions", fitFighterLoadedData.repetitions },
            { "averagWork", fitFighterLoadedData.averageWork },
            { "caloriesBurned", fitFighterLoadedData.caloriesBurned },
            { "workByRepetitions", fitFighterLoadedData.workByRepetitions },
            { "powerList", fitFighterLoadedData.powerList },
            { "repetitionsPerRound", fitFighterLoadedData.repetitionsPerRound },
            { "roundScore", fitFighterLoadedData.roundScore },
            { "timeToFinish", fitFighterLoadedData.timeToFinish },
            { "date", fitFighterLoadedData.date },
        };

        DocumentReference _documentReference = FirestoreDatabaseManager.db.Collection(mobileFirebaseCollection).Document(gameRecordsFirebaseDocument).Collection("fitFighter").Document(AuthenticationManager.Instance.user.UserId);

        FirestoreDatabaseManager.Instance.SaveDocument(_documentReference, _fitFighterData, OnUserDataInitialized,
            (error) =>
            {
                Debug.LogError("UserData//InitializeUserData// error " + error);
            });
#endif
        } // END UploadFitFighterDocument


        #endregion


        #region UPLOAD ROWING DOCUMENT


        //---------------------------------//
        public static void UpdateRowingDocument()
        //---------------------------------//
        {
            Debug.Log("UserDataManager.cs // UploadRowingDocument() //  Starting upload");
#if ROWING_CANOE

        Dictionary<string, object> _rowingData = new Dictionary<string, object>
        { 
            { "highScore", rowingCanoeLoadedData.highScore },
            { "strokes", rowingCanoeLoadedData.strokes },
            { "averageWork", rowingCanoeLoadedData.averageWork },
            { "totalWork", rowingCanoeLoadedData.totalWork },
            { "peakWork", rowingCanoeLoadedData.peakWork },
            { "caloriesBurned", rowingCanoeLoadedData.caloriesBurned },
            { "repetitions", rowingCanoeLoadedData.repetitions },
            { "maxSpeed", rowingCanoeLoadedData.maxSpeed },
            { "averageSpeed", rowingCanoeLoadedData.averageSpeed },
            { "paceMinutes", rowingCanoeLoadedData.paceMinutes },
            { "paceSeconds", rowingCanoeLoadedData.paceSeconds },
            { "cadence", rowingCanoeLoadedData.cadence },
            { "workByRepetitions", rowingCanoeLoadedData.workByRepetitions },
            { "powerList", rowingCanoeLoadedData.powerList },
            { "timeToFinish", rowingCanoeLoadedData.timeToFinish },
            { "date", rowingCanoeLoadedData.date },
            { "leaderboardScore" , rowingCanoeLoadedData.leaderboardScore },
        };

        DocumentReference _personalBestReference = FirestoreDatabaseManager.db.Collection(mobileFirebaseCollection).Document(gameRecordsFirebaseDocument).Collection(rowingGameCollection).Document(personalBestDocument).Collection("user").Document(AuthenticationManager.Instance.user.UserId);

        // TODO Change from Save Document
        FirestoreDatabaseManager.Instance.SaveDocument(_personalBestReference, _rowingData, OnUserDataInitialized,
            (error) =>
            {
                Debug.LogError("UserData//InitializeUserData// error " + error);
            });
#endif
        } // END UploadRowingDocument


        #endregion


        #endregion


    } // END UserDataManager.cs
}

public enum Goal
{
    Burn = 1,
    Tone = 2,
    Build = 3
}

public enum FitnessLevel
{
    Elite = 1, // (6 - 7 Days)
    Serious = 2, // (3 - 5 Days)
    Casual = 3, // (1 - 2 Days)
    Starter = 4 // 
}

public enum Gender
{
    Female = 1,
    Male = 2
}