using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameHubManager : MonoBehaviour
{
    /*
    #region VARIABLES


    [Header("Debug")]
    [SerializeField] private bool isDebug;

    [Header("Scene To Load")]
    [SerializeField] private int sceneIndexToLoad;

    [Header("Level Indexes")]
    [SerializeField] private List<int> levelIndex = new List<int>();

    [Header("Game Level Containers")]
    [SerializeField] private List<GameHubLevelData> gameLevelDatas = new List<GameHubLevelData>();

    [Header("Game Level Objects")]
    [SerializeField] private List<GameHubLevelHandler> gameLevelObjects = new List<GameHubLevelHandler>();

    [Header("Prefab to Spawn")]
    [SerializeField] private GameObject prefab;

    [Header("Spawn Position")]
    [SerializeField] private GameObject scrollViewPanel;

    [Header("Display Name of Currently Selected Game")]
    [SerializeField] private TextMeshProUGUI gameNameText;

    [Header("Starting Position of Panel Object")]
    [SerializeField] private float startingXPosOffset;

    [Header("Current Index Counter")]
    [SerializeField] private int counter = 0;

    [Header("Button References")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button lArrowButton;
    [SerializeField] private Button rArrowButton;

    [System.Serializable]
    public class GameHubLevelData
    {
        [Header("Data")]
        public int sceneIndex;
        public string gameName;
        public Sprite sprite;
        public Button startButton;

    } // END GameHubLevelData.cs


    #endregion


    #region START


    //------------------//
    private void Start()
    //-----------------//
    {
        Init();

    } // END Start

    
    #endregion


    #region INIT


    //-----------------//
    private void Init()
    //-----------------//
    {
        if(isDebug)
        {
            Debug.Log("GameHubManager.cs // Init() //");
        }
        
        startButton.interactable = true;
        counter = 0;

        for (int i = 0; i < gameLevelDatas.Count; i++)
        {
            GameHubLevelHandler handler = Instantiate(prefab, scrollViewPanel.transform).GetComponentInChildren<GameHubLevelHandler>();
            handler.Init(gameLevelDatas[i].sceneIndex, gameLevelDatas[i].gameName, gameLevelDatas[i].sprite, gameLevelDatas[i].startButton);

            gameLevelObjects.Add(handler);
            levelIndex.Add(gameLevelDatas[i].sceneIndex);

            if(i == 0)
            {
                gameNameText.text = gameLevelDatas[0].gameName;
                sceneIndexToLoad = gameLevelDatas[0].sceneIndex;
            }
        }

        lArrowButton.gameObject.SetActive(false);
        rArrowButton.gameObject.SetActive(true);

    } // END Init


    #endregion


    #region ON LEFT / RIGHT ARROW PRESS

    
    //------------------------------//
    public void OnLeftArrowPressed()
    //-----------------------------//
    {
        if(isDebug)
        {
            Debug.Log("GameHubManager.cs // OnLeftArrowPressed() // Pressed Left");
        }

        counter--;

        if (counter <= 0)
        {
            counter = 0;
            lArrowButton.gameObject.SetActive(false);
        }

        MenuMoveLeft();

        // Re-enable right arrow button if it is not active
        if (rArrowButton.gameObject.activeInHierarchy == false)
        {
            rArrowButton.gameObject.SetActive(true);
        }

        gameNameText.text = gameLevelDatas[counter].gameName.ToString();
        sceneIndexToLoad = levelIndex[counter];

    } // END OnLeftArrowPressed


    //------------------------------//
    public void OnRightArrowPressed()
    //-----------------------------//
    {
        if(isDebug)
        {
            Debug.Log("GameHubManager.cs // OnRightArrowPressed() // Pressed Right");
        }

        counter++;

        // Disable button after reaching the end of the list
        if (counter >= levelIndex.Count - 1)
        {
            counter = levelIndex.Count - 1;
            rArrowButton.gameObject.SetActive(false);
        }

        MenuMoveRight();

        // Re-enable left arrow button if it is not active
        if (lArrowButton.gameObject.activeInHierarchy == false)
        {
            lArrowButton.gameObject.SetActive(true);
        }

        gameNameText.text = gameLevelDatas[counter].gameName.ToString();
        sceneIndexToLoad = levelIndex[counter];

    } // END OnRightArrowPressed


    #endregion


    #region ON START PRESSED


    //--------------------------//
    public void OnStartPressed()
    //--------------------------//
    {
        if(isDebug)
        {
            Debug.Log("GameHubManager.cs // OnStartPressed()");
        }

        startButton.interactable = false;
        SceneManager.LoadSceneAsync(sceneIndexToLoad);

    } // END OnStartPressed


    #endregion


    #region MENU MOVEMENT


    //--------------------------//
    private void MenuMoveLeft()
    //--------------------------//
    {
        scrollViewPanel.transform.DOLocalMoveX(startingXPosOffset - (counter * scrollViewPanel.GetComponent<HorizontalLayoutGroup>().spacing), 0.3f, true);

    } // END MenuMoveLeft


    //--------------------------//
    private void MenuMoveRight()
    //--------------------------//
    {
        scrollViewPanel.transform.DOLocalMoveX(startingXPosOffset - (counter * scrollViewPanel.GetComponent<HorizontalLayoutGroup>().spacing), 0.3f, true);

    } // END MenuMoveRight


    #endregion
    */

} // END GameHubManager.cs
