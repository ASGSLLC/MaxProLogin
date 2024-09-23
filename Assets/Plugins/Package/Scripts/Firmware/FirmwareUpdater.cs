using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using maxprofitness.login;

public class FirmwareUpdater : MonoBehaviour
{
    #region VARIABLES


    public Dictionary<string, object> firmwareDataDictionary = new Dictionary<string, object>();

    [SerializeField] private string firmwareLink;
    [SerializeField] private string firmwareName;
    [SerializeField] private string firmwareJsonName;
    [SerializeField] private float firmwareVersion;

    private string firmwareVersionDocument = "firmwareVersionsList";
    private string collectionID = "1g0UXUbLsK0yYHHjKGlN";

    private FirebaseStorage storage;
    private StorageReference storageReference;
    private Firmware firmware;


    #endregion


    #region MONOBEHAVIOURS


    //------------------//
    private void Awake()
    //-----------------//
    {
        firmware = GetComponent<Firmware>();

    } // END Awake


    //------------------//
    private void Start()
    //------------------//
    {
        Init();

    } // END Start


    #endregion


    #region INIT


    //-----------------//
    private void Init()
    //----------------//
    {
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://maxfit-app.appspot.com/firmwareFiles");

    } // END Init


    #endregion


    #region CLEAR FIRMWARE DATA


    //------------------------------//
    private void ClearFirmwareData()
    //-----------------------------//
    {
        firmware.jsonData = "";
        firmware.firmwareDataChunkList.Clear();
        firmware.dataArray = null;

    } // ClearFirmwareData


    #endregion


    #region RECEIVE FIRMWARE DATA


    //-------------------------------------//
    public void ReceiveFirmwareData()
    //-------------------------------------//
    {
        ClearFirmwareData();
        
        //Grab User Data
        GetFirmwareDocument((snapShot) =>
        {
            Debug.Log("FirmwareUpdate.cs // ReceiveFirmwareData() // Successfully got data");

        }, (error) =>
        {
            Debug.Log("UserProfilePage.cs // ReceiveFirmwareData() // ERROR: Was not able to initialize user data");
        });

    } // END ReceiveProfileData


    #endregion


    #region INITIALIZE USER DATA


    //----------------------------------//
    private void GetFirmwareDocument(Action<DocumentSnapshot> OnSuccess, Action<string> OnError)
    //----------------------------------//
    {
        //Debug.Log("FirmwareUpdate.cs // GetFirmwareDocument() // ");

#if !FIREBASE_STORAGE && ROWING
        FirestoreDatabaseManager.Instance.CheckDependecies(() =>
        {
            if (FirestoreDatabaseManager.db == null)
            {
                //Debug.Log("FirmwareUpdate.cs // GetFirmwareDocument() // db null");
            }
            else
            {
                //Debug.Log("FirmwareUpdate.cs // GetFirmwareDocument() // db");
            }

            DocumentReference usersRef = FirestoreDatabaseManager.db.Collection("firmwareVersionsList").Document("1g0UXUbLsK0yYHHjKGlN");

            OnSuccess += (document) =>
            {
                if (document.Exists == false)
                {
                    //Debug.Log("FirmwareUpdate.cs // GetFirmwareDocument() // Could not download firmware file");
                }
                else
                {
                    //Debug.Log("FirmwareUpdate.cs // GetFirmwareDocument() // have document");
                    ReadFirmwareDocument(document);
                }
            };

            // Download the users data
            FirestoreDatabaseManager.Instance.GetDocument(usersRef, OnSuccess, OnError);

        });
#endif

    } // END GetFirmwareDocuments


    #endregion


    #region READ FIRMWARE DOCUMENT


    //--------------------------------------------------//
    private void ReadFirmwareDocument(DocumentSnapshot document)
    //--------------------------------------------------//
    {
        //Debug.Log("FirmwareUpdater.cs // ReadFirmwareDocument() // ");
        
        Dictionary<string, object> _firmwareDataDictionary = document.ToDictionary();

        firmwareVersion = Convert.ToSingle(_firmwareDataDictionary["version"]);

        // Getting the properties of the nested Dictionary
        firmwareDataDictionary = (Dictionary<string, object>)_firmwareDataDictionary["file"];

        firmwareLink = (string)firmwareDataDictionary["link"];
        firmwareName = (string)firmwareDataDictionary["name"];
        firmwareJsonName = (string)firmwareDataDictionary["ref"];

        GetFirmwareJson();

    } // END ReadFirmwareDocument


    #endregion


    #region GET FIRMWARE JSON


    //---------------------------//
    private void GetFirmwareJson()
    //---------------------------//
    {
        StorageReference _downloadRef = storageReference.Child(firmwareJsonName);

        _downloadRef.GetDownloadUrlAsync().ContinueWithOnMainThread((_task) =>
        {
            if (_task.IsFaulted || _task.IsCanceled)
            {
                Debug.Log("Could not upload.  Error: " + _task.Exception.ToString());
            }
            else
            {
                Debug.Log(_task.Result.ToString());

                StartCoroutine(IDownloadJson(_task.Result.ToString()));

                
                //Debug.Log("Firmwareupdate.cs // GetFirmwareJson() // Finished downloading Firmware...");
            }
        });

    } // END GetFirmwareJson


    #endregion


    #region IDOWNLOAD JSON


    //--------------------------------------------//
    private IEnumerator IDownloadJson(string _url)
    //--------------------------------------------//
    {
        UnityWebRequest _request = UnityWebRequest.Get(_url);

        yield return _request.SendWebRequest();

        if (_request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(_request.error);
            Debug.Log(_request.result);
            Debug.Log(_request.responseCode);
        }
        else
        {
            //Debug.Log("FirmwareUpdater.cs // IDownloadJson() // Downloading ");

            

            //Debug.Log(_text.ToString());
            //Debug.Log("FirmwareUpdater.cs // IDownloadJson() // Downloaded. Wrote to local device");
        }

    } // END IDownloadJson


    #endregion


} // END FirmwareUpdate.cs
