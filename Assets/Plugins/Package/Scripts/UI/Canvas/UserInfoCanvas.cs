using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System;
using UnityEngine.UI;
using System.Runtime.ExceptionServices;

using DG.Tweening;
using UnityEngine.SceneManagement;
using Firebase.Database;
using static SystemOfMeasurementHelper;
using maxprofitness.login;
using maxprofitness.shared;

public class UserInfoCanvas : CanvasGroupUIBase
{
    //[SerializeField] private GameObject firstPage;
    //[SerializeField] private GameObject secondPage;
    //[SerializeField] private GameObject thirdPage;

    [SerializeField] private GameObject[] pages;

    [SerializeField] private TextMeshProUGUI nextButtonText;
    
    [Space(20)]

    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField nicknameInput;
    [SerializeField] private TMP_Dropdown goalDropDown;
    [SerializeField] private TMP_Dropdown fitnessLevelDropDown;

    [Space(20)]
    [SerializeField] private Sprite enabledSprite;
    [SerializeField] private Sprite disabledSprite;
    [SerializeField] private Button imperialButton;
    [SerializeField] private Button metricButton;
    [SerializeField] private TMP_InputField weightInput;
    [SerializeField] private TMP_InputField heightInput;

    [Space(20)]

    [SerializeField] private TMP_Dropdown genderDropDown;
    [SerializeField] private TMP_Dropdown countryDropDown;

    [SerializeField] private TMP_InputField dayInput;
    [SerializeField] private TMP_InputField monthInput;
    [SerializeField] private TMP_InputField yearInput;

    [SerializeField] private TMP_Text warningRegisterText;

    private int currentPage = 0;

    private LoginCanvas loginCanvas;

    protected override void Awake()
    {
        base.Awake();

        ForceHide();

        loginCanvas = FindObjectOfType<LoginCanvas>();

        nameInput.onValueChanged.AddListener(OnNameValueChanged);
        nicknameInput.onValueChanged.AddListener(OnNicknameValueChanged);

        goalDropDown.onValueChanged.AddListener(OnGoalValueChanged);
        fitnessLevelDropDown.onValueChanged.AddListener(OnFitnessValueChanged);

        imperialButton.onClick.AddListener(ImperialPressed);
        metricButton.onClick.AddListener(MetricPressed);

        weightInput.onValueChanged.AddListener(OnWeightValueChanged);
        heightInput.onValueChanged.AddListener(OnHeightValueChanged);

        genderDropDown.onValueChanged.AddListener(OnGenderValueChanged);
        countryDropDown.onValueChanged.AddListener(OnCountryValueChanged);

        dayInput.onValueChanged.AddListener(OnDayValueChanged);
        monthInput.onValueChanged.AddListener(OnMonthValueChanged);
        yearInput.onValueChanged.AddListener(OnYearValueChanged);

        UserEnteredData.LoadCachedData();



    }

    #region NEXT AND BACK BUTTONS
    public void NextPagePressed()
    {
        Debug.Log("UserInfoCanvas//NextPagePressed// currentPage " + currentPage);

        if (currentPage == 2)
        {
            Debug.Log("UserInfoCanvas//NextPagePressed// currentPage == 2");

            //SceneManager.LoadSceneAsync(1);
            UserDataManager.FirstTimeUserUpload();

            return;
        }

        currentPage++;

        for (int i = 0; i < pages.Length; i++)
        {
            if (currentPage == i)
            {
                pages[i].SetActive(true);
            }
            else
            {
                pages[i].SetActive(false);
            }
        }
    }

    public void BackButtonPressed()
    {
        Debug.Log("UserInfoCanvas//BackButtonPressed// currentPage " + currentPage);

        if (currentPage == 0) 
        {
            Debug.Log("UserInfoCanvas//BackButtonPressed// back to login ");

            ForceHide();
            loginCanvas.ForceShow();
            return;
        }

        currentPage--;

        for (int i = 0; i < pages.Length; i++)
        {
            if (currentPage == i)
            {
                pages[i].SetActive(true);
            }
            else
            {
                pages[i].SetActive(false);
            }
        }
    }
    #endregion

    #region PAGE 1
    private void OnNameValueChanged(string value)
    {
       UserEnteredData.SaveDisplayName(value);
    }

    private void OnNicknameValueChanged(string value)
    {
        UserEnteredData.SaveNickname(value);
    }

    private void OnGoalValueChanged(int arg0)
    {
        Debug.Log(arg0);
        Debug.Log(goalDropDown.captionText);
    }

    private void OnFitnessValueChanged(int arg0)
    {

    }
    #endregion

    public void ImperialPressed()
    {
        imperialButton.image.sprite = enabledSprite;
        metricButton.image.sprite = disabledSprite;

        UserEnteredData.SaveIsImperial(true);
    }

    public void MetricPressed()
    {
        metricButton.image.sprite = enabledSprite;
        imperialButton.image.sprite = disabledSprite;

        UserEnteredData.SaveIsImperial(false);
    }

    private void OnWeightValueChanged(string value)
    {
        //UserEnteredData.Save(false);
    }

    private void OnHeightValueChanged(string value) 
    {
    
    }

    private void OnGenderValueChanged(int value)
    {

    }

    private void OnCountryValueChanged(int value)
    {
        Debug.Log("UserInfoCanvas//OnCountryValueChanged// value " + value);

        UserEnteredData.SaveCountry(countryDropDown.captionText.text);
    }

    private void OnDayValueChanged(string value)
    {
        UserEnteredData.SaveDay(int.Parse(value));
    }

    private void OnMonthValueChanged(string value)
    {
        UserEnteredData.SaveMonth(int.Parse(value));
    }

    private void OnYearValueChanged(string value)
    {
        UserEnteredData.SaveYear(int.Parse(value));
    }

    public override void ForceShow() 
    {
        base.ForceShow();


    }

}
