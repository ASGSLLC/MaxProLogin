using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using maxprofitness.login;

[Serializable]
public class FirmwareDataChunk
{
    public string offset;
    public string data;
    public string updateCommandString;
}

public class Firmware : MonoBehaviour
{
    #region VARIABLES


    private MaxProController maxProController;
    private string pathToFirmware;

    [Header("Local Debugging")]
    [SerializeField] private bool isDebug;

    [Header("Json Data")]
    public string jsonData;
    public string[] dataArray;
    public List<FirmwareDataChunk> firmwareDataChunkList = new List<FirmwareDataChunk>();


    #endregion


    #region MONOBEHAVIOURS


    //------------------//
    private void Awake()
    //------------------//
    {
        //maxProController = FindObjectOfType<MaxProController>();
        //spathToFirmware = Application.persistentDataPath + "/Firmware.json";

    } // END Awake


    //------------------//
    private void Start()
    //------------------//
    {
        //InitJsonData();
        
    } // END Start


    //----------------------//
    private void OnDestroy()
    //----------------------//
    {
        

    } // END Destroy


    #endregion


    #region INIT JSON DATA


    //-----------------//
    public void InitJsonData()
    //-----------------//
    {
        if (isDebug == true)
        {
#if UNITY_IOS
            jsonData = File.ReadAllText("/Users/asquaredevmac/Desktop/Firmware.json");
#elif UNITY_ANDROID
            jsonData = File.ReadAllText(pathToFirmware);
#endif
        }
        else
        {
            jsonData = File.ReadAllText(pathToFirmware);
        }

        dataArray = jsonData.Split('}');

        // Skip the last element it is an empty string once this loop is completed
        for(int i = 0; i < dataArray.Length - 1; i ++)
        {
            if(dataArray[i].Contains("{"))
            {
                dataArray[i] = dataArray[i].Replace("{", "");
            }

            if(dataArray[i].Contains(","))
            {
                dataArray[i] = dataArray[i].Remove(0, 1);
            }

            if (dataArray[i].Contains("]"))
            {
                dataArray[i] = dataArray[i].Replace("]", "");
            }

            dataArray[i] = dataArray[i].Trim();
            dataArray[i] = dataArray[i].Replace(" ", "");

            string[] _splitData = dataArray[i].Split(',');
            _splitData[0] = _splitData[0].Replace("offset", "");
            _splitData[0] = _splitData[0].Replace('"', ' ');
            _splitData[0] = _splitData[0].Replace(':', ' ');
            _splitData[0] = _splitData[0].Replace(" ", "");
            Debug.Log("Offset: " + _splitData[0]);

            _splitData[1] = _splitData[1].Replace("data", "");
            _splitData[1] = _splitData[1].Replace('"', ' ');
            _splitData[1] = _splitData[1].Replace(':', ' ');
            _splitData[1] = _splitData[1].Replace(" ", "");
            Debug.Log("Data:" + _splitData[1]);

            FirmwareDataChunk _dataChunk = new FirmwareDataChunk();

            _dataChunk.offset = _splitData[0];
            _dataChunk.data = _splitData[1].Trim();
            _dataChunk.updateCommandString = "[UPG," + _dataChunk.offset + "," + _dataChunk.data + "]";

            firmwareDataChunkList.Add(_dataChunk);
        }

    } // END InitJsonData


#endregion

    public void WriteFirmwareUpdateToMaxPro()
    {
        if(maxProController != null)
        {
            Debug.Log("Firmware.cs // WriteFirmwareupdateToMaxPro() // Have controller and are trying to connect");
            //maxProController.ConnectToMaxProFirmwareupdate();
        }
        else
        {
            Debug.Log("Firmware.cs // WriteFirmwareupdateToMaxPro() // MaxProController is null");
        }
    }

    
} // END Firmware.cs
