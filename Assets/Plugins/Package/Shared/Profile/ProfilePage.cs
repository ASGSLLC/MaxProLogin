using Firebase.Auth;
using Firebase.Database;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using maxprofitness.login;
using maxprofitness.shared;
public class ProfilePage : CanvasGroupUIBase
{
    #region VARIABLES
    [SerializeField] private GameObject deleteAccountConfirmation;
    #endregion

    #region START UP LOGIC
    //----------------------------------//
    protected override void Awake()
    //----------------------------------//
    {
        base.Awake();

        ForceHide();

        FirestoreDatabaseManager.Instance.CheckDependecies();

        AuthenticationManager.OnAuthenticationStateChanged += OnAuthenticationStateChanged;

    } // END Awake
    #endregion

    #region PROFILE OPTIONS
    //----------------------------------//
    public void LogoutPressed()
    //----------------------------------//
    {
        AuthenticationManager.Instance.SignOut();


    } // END LogoutPressed

    //----------------------------------//
    private void OnAuthenticationStateChanged(FirebaseUser user)
    //----------------------------------//
    {
        //Debug.Log("ProfilePage//OnAuthenticationStateChanged//");
        Debug.Log("LoginCanvas//LoginButtonPressed// Reloaded Scene");
        //SceneManager.LoadSceneAsync(0);

    } // END OnAuthenticationStateChanged
    #endregion

    #region DELETE ACCOUNT
    //----------------------------------//
    public void DeleteAccountPressed()
    //----------------------------------//
    {
        deleteAccountConfirmation.SetActive(true);

    } // END DeleteAccountPressed

    //----------------------------------//
    public void DeleteAccountConfirmed()
    //----------------------------------//
    {
        canvasGroup.blocksRaycasts = false;

        if (AuthenticationManager.Instance.user == null)
            return;

        FirestoreDatabaseManager.Instance.CheckDependecies(() =>
        {
            Debug.Log("ProfilePage//CheckDependecies// Success");

            DocumentReference userRef = FirestoreDatabaseManager.db.Collection("users").Document(AuthenticationManager.Instance.user.UserId);

            FirestoreDatabaseManager.Instance.DeleteDocument(userRef, () =>
            {
                Debug.Log("ProfilePage//DeleteDocument// Success");

                AuthenticationManager.Instance.DeleteUser(() =>
                {
                    //Debug.Log("ProfilePage//DeleteUser// Success");
                    Debug.Log("LoginCanvas//LoginButtonPressed// Reloaded Scene");
                    SceneManager.LoadSceneAsync(0);
                },
                (error) =>
                {
                    Debug.Log("ProfilePage//DeleteUser// error " + error);

                    canvasGroup.blocksRaycasts = true;
                });
            },
            (error) =>
            {
                Debug.Log("ProfilePage//DeleteDocument// error " + error);

                canvasGroup.blocksRaycasts = true;
            });
        });

    } // END DeleteAccountConfirmed

    //----------------------------------//
    public void CancelDeleteAccount()
    //----------------------------------//
    {
        deleteAccountConfirmation.SetActive(false);

    } // END CancelDeleteAccount
    #endregion

    #region EXIT BUTTON PRESSED
    //----------------------------------//
    public void ExitButtonPressed()
    //----------------------------------//
    {
        ForceHide();

    } // END ExitButtonPressed
    #endregion

    private void OnDestroy()
    {
        Destroy(canvasGroup);
    }

} // END Class
