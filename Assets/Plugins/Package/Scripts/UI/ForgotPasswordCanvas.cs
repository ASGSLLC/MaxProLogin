using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using maxprofitness.login;

public class ForgotPasswordCanvas : CanvasGroupUIBase
{
    [SerializeField] private TMP_InputField forgotPasswordEmail;
    [SerializeField] private TMP_Text warningForgotPasswordText;
    [SerializeField] private TMP_Text regularForgotPasswordText;

    private LoginCanvas loginCanvas;
    private WarningMessageHelper warningHelper;


    protected override void Awake()
    {
        base.Awake();

        ForceHide();

        loginCanvas = FindObjectOfType<LoginCanvas>();
        warningHelper = GetComponentInChildren<WarningMessageHelper>();

    }

    public void OnBackButtonPressed() 
    {
        ForceHide();
        loginCanvas.ForceShow();
    }

    //----------------------------------//
    public void SendResetEmail()
    //----------------------------------//
    {
        Debug.Log("LoginCanvasManager//SendResetEmail//");

        warningForgotPasswordText.text = "";
        regularForgotPasswordText.text = "Sending Email...";
        warningHelper.HideWarningMessage();


#if FIREBASE_AUTH
        AuthenticationManager.Instance.SendResetPasswordEmail(forgotPasswordEmail.text, OnPasswordResetSuccess, OnPasswordResetError);
#endif
    } // END SendResetEmail

    //----------------------------------//
    public void OnPasswordResetSuccess()
    //----------------------------------//
    {
        Debug.Log("LoginCanvasManager//OnPasswordResetSuccess//");

        warningForgotPasswordText.text = "";
        regularForgotPasswordText.text = "Email Sent!";
        warningHelper.HideWarningMessage();


    } // END OnPasswordResetEmailSent

    //----------------------------------//
    private void OnPasswordResetError(string error)
    //----------------------------------//
    {
        Debug.Log("LoginCanvasManager//OnPasswordResetError// error " + error);

        warningForgotPasswordText.text = error;
        regularForgotPasswordText.text = "";
        StartCoroutine(warningHelper.UpdateSize());


    } // END OnPasswordResetError

}
