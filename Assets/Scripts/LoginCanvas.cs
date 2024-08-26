
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using static SystemOfMeasurementHelper;

#if EXT_DOTWEEN
using DG.Tweening;
#endif

#if FIREBASE_AUTH
using Firebase.Auth;
using Firebase.Database;
using Firebase.Firestore;
#endif

#if GOOGLE
using Google;
#endif

public class LoginCanvas : CanvasGroupUIBase
{
    #region VARIABLES
    public static bool enableAutoSignIn = false;

    [SerializeField] private Toggle autoLoginToggle;

    private VideoBackground videoBackground;

    private RegisterCanvas registerUI;
    private ForgotPasswordCanvas forgotPasswordUI;
    private UserInfoCanvas userInfoCanvas;
    private WarningMessageHelper warningHelper;
    private LoginMusicManager loginMusic;
    
    public GameObject appleLogoButton;
   
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    ///public TMP_Text confirmLoginText;
    public Button loginButton;

    private Coroutine loginPressed;

    private Vector3 punchScale = new Vector3(0.25f, 0.25f, 0.25f);
    #endregion

    #region START UP LOGIC
    //----------------------------------//
    protected override void Awake()
    //----------------------------------//
    {
        base.Awake();

        videoBackground = FindObjectOfType<VideoBackground>();
        forgotPasswordUI = FindObjectOfType<ForgotPasswordCanvas>();
        registerUI = FindObjectOfType<RegisterCanvas>();
        userInfoCanvas = FindObjectOfType<UserInfoCanvas>();
        loginMusic = FindObjectOfType<LoginMusicManager>();
        warningHelper = GetComponentInChildren<WarningMessageHelper>();

        ForceShow();

#if !UNITY_IOS
        appleLogoButton.SetActive(false);
#endif

        loginButton.onClick.AddListener(LoginButtonPressed);

        //enableAutoSignIn = false;

        bool.TryParse(PlayerPrefs.GetString("AutoLogin"), out enableAutoSignIn);
  
        autoLoginToggle.isOn = enableAutoSignIn;

        autoLoginToggle.onValueChanged.AddListener(ToggleAutoLogin);

        Debug.Log("LoginCanvas//Awake// enableAutoSignIn " + enableAutoSignIn);

#if FIREBASE_AUTH
        AuthenticationManager.OnAuthenticationStateChanged += OnAuthenticationStateChanged;
#endif  

    } // END Awake
    #endregion

    #region LOGIN
    //----------------------------------//
    public void LoginButtonPressed()
    //----------------------------------//
    {
        Debug.Log("LoginCanvas//LoginButtonPressed// ");

#if FIREBASE_AUTH
        AuthenticationManager.OnAuthenticationStateChanged -= OnAuthenticationStateChanged;
#endif

        if (loginPressed == null)
        {
            Debug.Log("LoginCanvas//LoginButtonPressed// calling CheckIfUserExist");

            loginButton.interactable = false;

#if FIREBASE_AUTH
            loginPressed = AuthenticationManager.Instance.Login(emailLoginField.text, passwordLoginField.text, () =>
            {
                Debug.Log("LoginCanvas//LoginButtonPressed// loged in");

                warningLoginText.text = "";
                warningHelper.HideWarningMessage();                
                loginPressed = null;

                LoadHomePage();
                
                //UserDataManager.Instance.InitializeUserData(OnUserDataFound, OnUserDataError);

                //loginButton.interactable = true;
            }, (error) =>
            {
                Debug.Log("LoginCanvas//LoginButtonPressed// error " + error);

                if (warningLoginText != null && loginButton != null) 
                {
                    warningLoginText.text = error;
                    StartCoroutine(warningHelper.UpdateSize());
                    loginButton.interactable = true;
                }

                loginPressed = null;
            });
#endif
        }

    } // END LoginButtonPressed

    //----------------------------------//
    private void OnUserDataFound(DocumentSnapshot document)
    //----------------------------------//
    {
        Debug.Log("LoginCanvas//OnUserDataFound// Exists " + document.Exists);

        loginButton.interactable = true;

        // Missing User data
        if (document.Exists == false)
        {
            ForceHide();
            userInfoCanvas.ForceShow();
        }
        else
        {
            // StartCoroutine(IGetData());
        }

    } // END OnUserDataFound

    //----------------------------------//
    private void OnUserDataError(string error)
    //----------------------------------//
    {
        loginButton.interactable = true;

    } // END OnUserDataError
    #endregion

    #region SOCIAL LOGINS
    //----------------------------------//
    public void GoogleLogin()
    //----------------------------------//
    {
#if FIREBASE_AUTH
        AuthenticationManager.OnAuthenticationStateChanged -= OnAuthenticationStateChanged;
#endif

        Debug.Log("LoginCanvas//GoogleLogin//");
#if GOOGLE
        AuthenticationManager.Instance.RequestGoogleCredentials((GoogleSignInUser, credentials) => 
        { 
            Debug.LogWarning("LoginCanvas//GoogleLogin// Got credentials ");

            AuthenticationManager.Instance.SignInWithCredentials(credentials, (user) =>
            {
                Debug.Log("LoginCanvasManager//GoogleLogin// SignInWithCredentials success");
                Debug.Log("LoginCanvasManager//GoogleLogin// user " + user.Email);

                LoadHomePage();
            },
            (error) =>
            {
                Debug.Log("LoginCanvasManager//SignInWithCredentials// error " + error);
            });
        },
        (error) =>
        { 
            Debug.LogWarning("LoginCanvas//GoogleLogin//NOPE!!");
        });
#endif

    } // END GoogleLogin

    //----------------------------------//
    public void FacebookLogin()
    //----------------------------------//
    {
#if FIREBASE_AUTH
        AuthenticationManager.OnAuthenticationStateChanged -= OnAuthenticationStateChanged;
#endif

        Debug.Log("LoginCanvas//FacebookLogin//");

#if FACEBOOK
        AuthenticationManager.Instance.RequestFacebookCredentials((credentials) =>
        {
            AuthenticationManager.Instance.SignInWithCredentials(credentials, (user) =>
            {
                Debug.Log("LoginCanvasManager//FacebookLogin// SignInWithCredentials success");
                Debug.Log("LoginCanvasManager//FacebookLogin// success user " + user.Email);

                LoadHomePage();
            },
           (error) =>
           {
               Debug.Log("LoginCanvasManager//FacebookLogin// SignInWithCredentials error " + error);
           });
        },
     (error) =>
     {
         Debug.LogError("LoginCanvasManager//FacebookLogin// RequestFacebookCredentials error " + error);
     });
#endif

    } // END FacebookLogin

    //----------------------------------//
    public void LoginWithApple()
    //----------------------------------//
    {
#if FIREBASE_AUTH
        AuthenticationManager.OnAuthenticationStateChanged -= OnAuthenticationStateChanged;
#endif

#if APPLE
        AuthenticationManager.Instance.RequestAppleCredentials((cred, appleAccount) =>
        {
            Debug.Log("LoginCanvasManager//RequestAppleCredentials// success ");

            if (cred == null)
            {
                Debug.Log("LoginCanvasManager//RequestAppleCredentials// credentials are null");
                return;
            }


            if (appleAccount == null)
            {
                Debug.Log("LoginCanvasManager//RequestAppleCredentials// apple account null");
                return;
            }

            AuthenticationManager.Instance.SignInWithCredentials(cred, (user) =>
            {
                Debug.Log("LoginCanvasManager//SignInWithCredentials// success");
                Debug.Log("LoginCanvasManager//SignInWithCredentials// success user " + user.Email);
                Debug.Log("LoginCanvasManager//SignInWithCredentials// fullname " + appleAccount.FullName);

                if (string.IsNullOrEmpty(user.DisplayName) == true)
                {
                    Debug.Log("LoginCanvasManager//SignInWithCredentials// doesnt have display name");

                    UserProfile profile = new UserProfile();
                    profile.DisplayName = user.Email;
                    Debug.Log("LoginCanvasManager//SignInWithCredentials// " + profile.DisplayName);

                    AuthenticationManager.Instance.UpdateUserProfile(profile, (user) =>
                    {
                        Debug.Log("LoginCanvasManager//UpdateUserProfile// success");
                        LoadHomePage();
                    });
                }
                else
                {
                    LoadHomePage();
                }
             
            },
           (error) =>
           {
               Debug.Log("LoginCanvasManager//SignInWithCredentials// error " + error);
           });
        },
        (error) =>
        {
            Debug.LogError("LoginCanvasManager//RequestAppleCredentials// error " + error);
        });
#endif

    } // END LoginWithApple
    #endregion

    #region AUTO LOGIN 
    //----------------------------------//
    public void ToggleAutoLogin(bool toggle)
    //----------------------------------//
    {
        Debug.Log("LoginCanvas//ToggleAutoLogin// " + toggle);

        enableAutoSignIn = toggle;

        PlayerPrefs.SetString("AutoLogin", enableAutoSignIn.ToString());

    } // END ToggleAutoLogin

#if FIREBASE_AUTH
    //----------------------------------//
    private void OnAuthenticationStateChanged(FirebaseUser user)
    //----------------------------------//
    {
        Debug.Log("LoginCanvas//OnAuthenticationStateChanged// enableAutoSignIn " + enableAutoSignIn);

        if (enableAutoSignIn == false)
        {
            //loginMusic.StartPlayMusic();
            return;
        }

        if (user != null)
        {
            loginMusic.StopPlayMusic();
            StartCoroutine(IAutoSignIn(user));
        }
        else if (user == null)
        {
            //loginMusic.StartPlayMusic();
        }

    } // END OnAuthenticationStateChanged 

    //----------------------------------//
    private IEnumerator IAutoSignIn(FirebaseUser user)
    //----------------------------------//
    {
        Debug.Log("LoginCanvas//IAutoSignIn//");

        emailLoginField.text = user.Email;
        passwordLoginField.text = "***********";

        loginButton.interactable = false;

        yield return new WaitForSeconds(1.5f);

        LoadHomePage();
        //UserDataManager.Instance.InitializeUserData(OnUserDataFound, OnUserDataError);

    } // END IAutoSighnIn
#endif
    #endregion

    #region SIGN UP PRESSED
    //----------------------------------//
    public void SignUpPressed()
    //----------------------------------//
    {
        ForceHide();

        registerUI.ForceShow();

        //PlayerPrefs.DeleteKey("isFirstTimeUser");
        //PlayerPrefs.Save();

    } // END CreateAnAccountPresssed
    #endregion

    #region FORGOT PASSWORD
    //----------------------------------//
    public void OnForgotPasswordPressed()
    //----------------------------------//
    {
        //Debug.Log("LoginCanvasManager//OnForgotPasswordPressed//");

        //videoBackground.DisableVideo();

        ForceHide();
        registerUI.ForceHide();

        forgotPasswordUI.ForceShow();

    } // END OnForgotPasswordPressed
    #endregion

    #region LOAD HOME PAGE
    //----------------------------------//
    private void LoadHomePage()
    //----------------------------------//
    {
        SceneManager.LoadSceneAsync(1);

    }//END LoadHomePage
    #endregion

    #region FORCE SHOW AND HIDE
    //----------------------------------//
    public override void ForceShow()
    //----------------------------------//
    {
        base.ForceShow();

        //videoBackground.EnableVideo();

    } // END ForceShow

    //----------------------------------//
    public override void ForceHide()
    //----------------------------------//
    {
        base.ForceHide();

        //videoBackground.DisableVideo();

    } // END ForceHide
    #endregion

    #region ON DESTROY
    //----------------------------------//
    private void OnDestroy()
    //----------------------------------//
    {

#if FIREBASE_AUTH
        AuthenticationManager.OnAuthenticationStateChanged -= OnAuthenticationStateChanged;
#endif

        loginButton.onClick.RemoveListener(LoginButtonPressed);

        autoLoginToggle.onValueChanged.RemoveListener(ToggleAutoLogin);

        StopAllCoroutines();

    } // END OnDestroy
    #endregion

}
