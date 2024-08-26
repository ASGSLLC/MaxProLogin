using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using maxprofitness.login;

public class UserDataCache : MonoBehaviour
{
    private void Awake()
    {
        //AuthenticationManager.OnAuthenticationStateChanged += ReceiveProfileData;
    }
    private void OnDestroy()
    {
        //AuthenticationManager.OnAuthenticationStateChanged -= ReceiveProfileData;
    }

    //-------------------------------------//
    public void ReceiveProfileData(FirebaseUser user)
    //-------------------------------------//
    {
        Debug.Log("NameInput.cs // ReceiveProfileData // Getting Profile Data for User");
        //countryDropDown.OnListGenerated -= ReceiveProfileData;
        if (UserDataManager.Instance == null)
        {
            return;
        }

        UserDataManager userDataManager = FindObjectOfType<UserDataManager>();
        UserDataMeta profileData = UserDataManager.loadedData;

        if (profileData == null)
        {
            //Grab User Data
            userDataManager.InitializeUserData((snapShot) =>
            {
                Debug.Log("UserProfilePage.cs // ReceiveProfileData() // Successfully got data");
            }, (error) =>
            {
                Debug.Log("UserProfilePage.cs // ReceiveProfileData() // ERROR: Was not able to initialize user data");
            });


        }
        else
        {
            Debug.Log("NameInput.cs // ReceiveProfileData // We already have profileData");
        }


    } // END ReceiveProfileData
}
