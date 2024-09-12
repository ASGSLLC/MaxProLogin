using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.iOS;
using Firebase.Auth;
using System.Runtime.CompilerServices;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif
namespace maxprofitness.login
{
    public class NameInput : MonoBehaviour
    {
        [SerializeField] private bool isUsingProfileName;

        [SerializeField] CanvasGroup NameInputCanvasGroup;
        [SerializeField] LocalLeaderboard leaderboard;

        [SerializeField] TextMeshProUGUI inputNameText;
        [SerializeField] TextMeshProUGUI pointValueText;

        [SerializeField] GameOverCanvas airRunnerGameOverCanvas;

        private TouchScreenKeyboard keyboard;
        private int currentScore;
        private int currentGameID;

        //----------------------------------//
        private void Awake()
        //----------------------------------//
        {
            CloseNameInput();

            if (PlayerPrefs.GetString("InputPlayerName") != null)
            {
                //inputNameText.text = PlayerPrefs.GetString("InputPlayerName");
            }
        }
        // END Awake


        //----------------------------------//
        private void Update()
        //----------------------------------//
        {
            if (keyboard != null && keyboard.active == true && inputNameText.text != keyboard.text)
            {
                inputNameText.text = keyboard.text;
            }
        }
        // END Update


        //----------------------------------//
        public void OpenKeyboard()
        //----------------------------------//
        {
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false, "Name", 16);
        }
        // END OpenKeyboard




        //----------------------------------//
        public void GetGameResults(int _score, int _gameID, bool _openNameInput)
        // Get results from the finished game, should be called when finishing any game before opening the name input canvas
        //----------------------------------//
        {
#if AIR_RUNNER
        if (isUsingProfileName)
        {
            if (UserDataManager.loadedData != null)
            {
                // TODO: Replace with firstName with nickName once when leaderboards upload nickname as well
                inputNameText.text = UserDataManager.loadedData.firstName;
            }

        }
        
        currentScore = _score;
        currentGameID = _gameID;
        pointValueText.text = _score.ToString() + " points!";
        
        if (_openNameInput)
        {
            OpenNameInput();
        }
#endif
        }
        // END GetGameResults


        //----------------------------------//
        public void OpenNameInput()
        //----------------------------------//
        {
            NameInputCanvasGroup.alpha = 1f;
            NameInputCanvasGroup.interactable = true;
            NameInputCanvasGroup.blocksRaycasts = true;
        }
        // END OpenNameInput


        //----------------------------------//
        public void CloseNameInput()
        //----------------------------------//
        {
            NameInputCanvasGroup.alpha = 0f;
            NameInputCanvasGroup.interactable = false;
            NameInputCanvasGroup.blocksRaycasts = false;
        }
        // END CloseNameInput


        //----------------------------------//
        public void SubmitButtonPressed()
        //----------------------------------//
        {
            PlayerPrefs.SetString("InputPlayerName", inputNameText.text);
            leaderboard.LoadLeaderBoard(leaderboard.gameName, currentGameID);
            leaderboard.SubmitButton(inputNameText.text, currentScore, currentGameID);
            EndGame();
        }
        // END SubmitButtonPressed


        public void EndGame()
        {
            // End game depending on current game
            // 2 = AirRunner Hills || 3 = AirRunner DarkSky || 4 = AirRunner DesertScene || 5 = AirRunner Forest || 6 = RowingCanoeRace || 7 = FitFighter Rhythym

            switch (currentGameID)
            {
#if AIR_RUNNER
            case 2:
                {
                    airRunnerGameOverCanvas.OnMainMenuPressed();
                    break;
                }
            case 3:
                {
                    airRunnerGameOverCanvas.OnMainMenuPressed();
                    break;
                }
            case 4:
                {
                    airRunnerGameOverCanvas.OnMainMenuPressed();
                    break;
                }
            case 5:
                {
                    airRunnerGameOverCanvas.OnMainMenuPressed();
                    break;
                }
#endif
                case 6:
                    {
                        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
                        break;
                    }
                case 7:
                    {
                        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
                        break;
                    }
            }

        }

    } // END Class

} // END Namespace