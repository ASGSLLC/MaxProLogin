using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
//using Sirenix.OdinInspector;

#if MAXPRO_LOGIN

#endif
using maxprofitness.login;
using maxprofitness.shared;
public class GameOverCanvas : YScaleUIBase
{
#if AIR_RUNNER
    public CanvasGroup lockCanvasGroup;
    public CanvasGroup newBestCanvasGroup;

    public Image levelImage;

    public Sprite[] levelSprits;

    public TextMeshProUGUI levelNameText;

    public TextMeshProUGUI unlockTextLevel;
    public TextMeshProUGUI unlockTextShip;

    public TextMeshProUGUI yourScoreText;
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI ringsCollectedText;

    public CanvasGroupUIBase canvasGroup;

    private GameManager gameManager;

    private ShipController shipController;

    private AirRunnerGameManager airRunnerGameManager;

    private MaxProMobileControlSelection controlSelection;

    private RectTransform rt;

    [SerializeField] private CanvasGroupUIBase modeSelection;

    [SerializeField] private GameObject metricsButton;

    public float startingPosX;

    [Header("Tint")]
    [SerializeField] private Image tintImage;
    [SerializeField] private float tintStrength;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        shipController = FindObjectOfType<ShipController>();

        airRunnerGameManager = FindObjectOfType<AirRunnerGameManager>();

        controlSelection = FindObjectOfType<MaxProMobileControlSelection>();

        HideLevelUnlock();

        canvasGroup = GetComponent<CanvasGroupUIBase>();
        rt = GetComponent<RectTransform>();

        startingPosX = this.transform.localPosition.x;
    }

    public void UpdateLevelUnlockText(string _levelText) 
    {
        unlockTextLevel.gameObject.SetActive(true);
        lockCanvasGroup.alpha = 1;
        unlockTextLevel.text = _levelText;        
    }


    public void UpdateShipUnlockText(string _shipText)
    {
        unlockTextShip.gameObject.SetActive(true);
        lockCanvasGroup.alpha = 1;
        unlockTextShip.text = _shipText;
    }


    public void HideLevelUnlock() 
    {
        unlockTextLevel.gameObject.SetActive(false);
        unlockTextShip.gameObject.SetActive(false);
        lockCanvasGroup.alpha = 0;
    }


    public override Tween Show()
    {
        if (controlSelection.isUsingMaxProControls == true)
        {
            metricsButton.gameObject.SetActive(true);
        }
        else
        {
            metricsButton.gameObject.SetActive(false);
        }

        canvasGroup.ForceShow();

        levelNameText.text = AirRunnerGameManager.selectedLevel.levelName;

        switch (AirRunnerGameManager.selectedLevel.levelName)
        {
            case "Hills":
                levelImage.sprite = levelSprits[0];
                break;

            case "Dark Sky":
                levelImage.sprite = levelSprits[1];
                break;

            case "Desert":
                levelImage.sprite = levelSprits[2];
                break;

            case "Forest":
                levelImage.sprite = levelSprits[3];
                break;
        }
        
        levelImage.preserveAspect = true;

        DOTween.Kill(transform);

        Tween openTween = transform.DOScaleY(1, 0);

        Tween tintTween = tintImage.DOFade(tintStrength, 1);

        tintTween.onComplete = () =>
        {
            Tween slideTween = transform.DOLocalMoveX(0, 1);
        };
        return tintTween;
    }

    public void OnMainMenuPressed() 
    {
        // Debug.Log("GameOverCanvas//OnRestartPressed//");

        //SceneManager.LoadScene(0);

        airRunnerGameManager.ResetPlayerPosition();

        shipController.resetShipPos();

        //gameManager.StartGame();

        //options.Show(Options.ShowOption.Options, Options.StartLocation.Start);

        transform.localScale = new Vector3(1, 0, 1);

        tintImage.color = Vector4.zero;

        rt.localPosition = new Vector3(startingPosX, 0, 0);

        modeSelection.ForceShow();
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (transform.localScale.y != 0)
        {
            if (GUI.Button(new Rect(10, 70, 50, 30), "Main Menu"))
            {
                OnMainMenuPressed();
            }

            if (GUI.Button(new Rect(10, 120, 50, 30), "ShowGameOver"))
            {
                Show();
            }
        }

    }
#endif
#endif
}
