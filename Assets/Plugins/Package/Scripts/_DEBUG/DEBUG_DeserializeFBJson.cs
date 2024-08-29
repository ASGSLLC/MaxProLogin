using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DEBUG_DeserializeFBJson : MonoBehaviour
{
    [Header("Debug")]
    public bool isDebug;

    private LocalLeaderboard leaderboard;
    private FirmwareUpdater firmwareUpdater;
    private void Start()
    {
        leaderboard = FindObjectOfType<LocalLeaderboard>();
        firmwareUpdater = FindObjectOfType<FirmwareUpdater>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDebug)
        {
            DEBUG_ReadForCommand();
        }
    }

    private void DEBUG_ReadForCommand()
    {
        if (Keyboard.current[Key.Numpad1].wasPressedThisFrame)
        {
            Debug.Log("Pressed Debug Key");
            firmwareUpdater.ReceiveFirmwareData();
            //UserDataManager.Instance.RecieveRowingData();
            //leaderboard.ClearLeaderboards();
            //UserDataManager.Instance.GetHillsLeaderboardEntries();
            //leaderboard.UpdateOnlineLeaderboards();
            //UserDataManager.Instance.GetFitFighterLeaderboardEntries();
            //UserDataManager.Instance.GetRowingLeaderboardEntries();
            /*
            UserDataManager.Instance.GetFitFighterLeaderboardData((snapShot) =>
            {
                Debug.Log("UserProfilePage.cs // ReceiveProfileData() // Successfully got data");
            }, (error) =>
            {
                Debug.Log("UserProfilePage.cs // ReceiveProfileData() // ERROR: Was not able to initialize user data");
            });
            */
        }
    }

}
