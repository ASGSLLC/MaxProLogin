using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using maxprofitness.login;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class RegisterCanvas : CanvasGroupUIBase
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField confirmedPasswordInput;

    [SerializeField] private TMP_Text registerWarning;

    [SerializeField] private GameObject appleButton;

    private Coroutine registerPressed;

    private Vector3 punchScale = new Vector3(0.25f, 0.25f, 0.25f);

    private LoginCanvas loginCanvas;

    private UserInfoCanvas userInfoCanvas;

    [SerializeField] private WarningMessageHelper warningHelper;


    //----------------------------------//
    protected override void Awake()
    //----------------------------------//
    {
        base.Awake();

#if !UNITY_IOS
        appleButton.SetActive(false);
#endif

        loginCanvas = FindObjectOfType<LoginCanvas>();
        userInfoCanvas = FindObjectOfType<UserInfoCanvas>();
        warningHelper = GetComponentInChildren<WarningMessageHelper>();


        //nameInput.onValueChanged.AddListener(OnNameValueChanged);
        emailInput.onValueChanged.AddListener(OnEmailValueChanged);
        //passwordInput.onValueChanged.AddListener(OnPasswordValueChanged);
        //confirmedPasswordInput.onValueChanged.AddListener(OnConfirmPasswordValueChanged);

        UserEnteredData.LoadCachedData();

        nameInput.text = UserEnteredData.displayName;
        emailInput.text = UserEnteredData.email;

        ForceHide();
    }

    public void RegisterButtonPressed()
    {

#if FIREBASE_AUTH
        if (AuthenticationManager.Instance.user != null)
        {
            AuthenticationManager.Instance.SignOut();
        }

       // AuthenticationManager.OnAuthenticationStateChanged -= OnAuthenticationStateChanged;
        //AuthenticationManager.OnAuthenticationStateChanged += OnAuthenticationStateChanged;
#endif
        registerWarning.text = "";
        warningHelper.HideWarningMessage();

      /*  if (string.IsNullOrEmpty(nameInput.text))
        {
            registerWarning.text = "*Missing Full Name";
            return;
        }
        else
        {
            UserEnteredData.SaveName(nameInput.text);
        }
      */
        if (string.IsNullOrEmpty(emailInput.text))
        {
            registerWarning.text = "Missing Email";
            StartCoroutine(warningHelper.UpdateSize());
            return;
        }
        else
        {
            UserEnteredData.SaveEmail(emailInput.text);
        }

        if (string.IsNullOrEmpty(passwordInput.text))
        {
            registerWarning.text = "Missing Password";
            StartCoroutine(warningHelper.UpdateSize());
            return;
        }

        if (registerPressed == null)
        {
#if FIREBASE_AUTH
            registerPressed = AuthenticationManager.Instance.Register(emailInput.text, passwordInput.text, confirmedPasswordInput.text,
                 (user) =>
                 {
                     Debug.Log("RegisterCanvas//RegisterButtonPressed// Register Sucessfull");

                     registerWarning.text = "";
                     warningHelper.HideWarningMessage();

                     registerPressed = null;

                     //passwordLoginField = passwordRegisterField;
                     //emailLoginField = emailRegisterField;

                     //registerUI.SetActive(false);

                     SceneManager.LoadSceneAsync(1);

                     //ForceHide();

                     //userInfoCanvas.ForceShow();
                 },
                 (error) =>
                 {
                     registerWarning.text = error;
                     StartCoroutine(warningHelper.UpdateSize());
                     registerPressed = null;
                 });
#endif
        }
    }

    public void ShowRegistrationWarning(string text)
    {
        registerWarning.text = text;
        registerWarning.enabled = true;
        registerWarning.transform.DOKill();
        registerWarning.transform.localScale = Vector3.one;
        registerWarning.transform.DOPunchScale(punchScale, 0.1f, 1, 1);
    }

    /*private void OnNameValueChanged(string value)
    {
        UserEnteredData.SaveDisplayName(value);
    }*/

    private void OnEmailValueChanged(string value)
    {
        UserEnteredData.SaveEmail(value);
    }

    private void OnPasswordValueChanged(string value)
    {

    }

    private void OnConfirmPasswordValueChanged(string value)
    {

    }

    public void OnBackButtonPressed() 
    {
        ForceHide();
        loginCanvas.ForceShow();
    }

    private void OnDestroy()
    {
        //nameInput.onValueChanged.RemoveListener(OnNameValueChanged);
        emailInput.onValueChanged.RemoveListener(OnEmailValueChanged);
        //passwordInput.onValueChanged.RemoveListener(OnPasswordValueChanged);
        //confirmedPasswordInput.onValueChanged.RemoveListener(OnConfirmPasswordValueChanged);
    }
}
