using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using static SystemOfMeasurementHelper;
using UnityEngine.Networking;
using UnityEngine.UI;
using Firebase.Storage;
using maxprofitness.login;

public class UserProfilePage : CanvasGroupUIBase
{
    #region VARIABLES


    [SerializeField] private CanvasGroup profileButton;
    [SerializeField] private List<GameObject> pages = new List<GameObject>();
    [SerializeField] private List<TextMeshProUGUI> profileNameText = new List<TextMeshProUGUI>();
    [SerializeField] private MeasurementSystemToggle measurementSystem;

    [Header("Edit Page Content")]
    [SerializeField] private TMP_InputField fullNameInput;
    [SerializeField] private TMP_InputField nicknameInput;
    [SerializeField] private TMP_Dropdown genderDropdown;
    [SerializeField] private TMP_Dropdown countryDropdown;
    [SerializeField] private TMP_Dropdown yearDropdown;
    [SerializeField] private TMP_Dropdown monthDropdown;
    [SerializeField] private TMP_Dropdown dayDropdown;
    [SerializeField] private TMP_Dropdown weightImperialDropdown;
    [SerializeField] private TMP_Dropdown weightMetricDropdown;
    [SerializeField] private TMP_Dropdown heightFeetDropdown;
    [SerializeField] private TMP_Dropdown heightInchesDropdown;
    [SerializeField] private TMP_Dropdown heightMetricDropdown;
    [SerializeField] private TMP_Dropdown goalDropdown;
    [SerializeField] private TMP_Dropdown fitnessLevelDropdown;
    [SerializeField] private Image editPageProfileImage;

    [Header("Profile Page Content")]
    [Space]
    [SerializeField] private TextMeshProUGUI fullNameText;
    [SerializeField] private TextMeshProUGUI nicknameText;
    [SerializeField] private TextMeshProUGUI emailText;
    [SerializeField] private TextMeshProUGUI weightText;
    [SerializeField] private TextMeshProUGUI heightText;
    [SerializeField] private TextMeshProUGUI genderText;
    [SerializeField] private TextMeshProUGUI birthdayText;
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private TextMeshProUGUI fitnessLevelText;
    [SerializeField] private TextMeshProUGUI countryText;
    [SerializeField] private Image profileImage;

    private int heightInches;
    private int heightFeet;
    private int height;
    private int weightImperial;
    private int weightMetric;

    private string url;
    private string userID;

    private FirebaseStorage storage;
    private StorageReference storageReference;

    private CanvasGroup canvasGroup;
    private Dictionary<string, object> userProfile = new Dictionary<string, object>();


    #endregion


    #region MONOBEHAVIOURS


    //-------------------------------------//
    private void Awake()
    //-------------------------------------//
    {
        canvasGroup = GetComponent<CanvasGroup>();
        measurementSystem = FindObjectOfType<MeasurementSystemToggle>();
        //countryDropDown.OnListGenerated += ReceiveProfileData;

    }//END Awake


    //------------------//
    private void Start()
    //------------------//
    {
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://maxfit-app.appspot.com/users-avatars");

    } // END Start


    #endregion


    #region OPEN NATIVE GALLERY PHOTO UPLOAD


    //------------------------------------------//
    public void OpenGalleryToUpdateProfilePhoto()
    //------------------------------------------//
    {
        OpenNativeGallery();

    } // END OpenGalleryToUpdateProfilePhoto


    //--------------------------------//
    public void OpenNativeGallery()
    //--------------------------------//
    {
        NativeGallery.Permission _permission = NativeGallery.GetImageFromGallery((_path) =>
        {
            if(string.IsNullOrEmpty(_path))
            {
                Debug.Log("UserProfilePage.cs // OpenNativeGallery() //Photo choosing was canceled.  Returning");
                return;
            }
            Texture2D _texture = NativeGallery.LoadImageAtPath(_path);
            
            if (_texture == null)
            {
                //Debug.Log("Couldn't load texture from " + _path);
                return;
            }

            profileImage.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), Vector2.zero);
            editPageProfileImage.sprite = profileImage.sprite;

#if UNITY_IOS
            UploadPhoto(_texture);

#elif UNITY_ANDROID
            UploadPhoto(null, _path);
#endif
        }, "Select an Image to Upload");

        //Debug.Log("Permission Result: " + _permission);

    } // END OpenNativeGallery


    //-----------------------------------//
    public void GetPhoto()
    //-----------------------------------//
    {
        StorageReference _uploadRef = storageReference.Child(userID).Child("profilePicture.jpg");

        _uploadRef.GetDownloadUrlAsync().ContinueWithOnMainThread((_task) =>
        {
            if (_task.IsFaulted || _task.IsCanceled)
            {
                //Debug.Log("Could not upload.  Error: " + _task.Exception.ToString());
            }
            else
            {
                StartCoroutine(IDownloadImage(_task.Result.ToString()));
                //Debug.Log("Finished uploading...");
            }
        });
        
    } // END GetPhoto


    //-----------------------------------//
    public void UploadPhoto(Texture2D _tex = null, string _path = null)
    //-----------------------------------//
    {
        MetadataChange _metaData = new MetadataChange();
        _metaData.ContentType = "image/jpeg";

        StorageReference _uploadRef = storageReference.Child(userID).Child("profilePicture.jpg");

        //Debug.Log("Uploading new Picture");

#if UNITY_IOS

        byte[] _bytes = DuplicateTexture(_tex).EncodeToJPG();

        _uploadRef.PutBytesAsync(_bytes, _metaData).ContinueWithOnMainThread((_task) =>
        {
            if (_task.IsFaulted || _task.IsCanceled)
            {
                Debug.Log("Could not upload.  Error: " + _task.Exception.ToString());
            }
            else
            {
                Debug.Log("Finished uploading...");
            }
        });

#elif UNITY_ANDROID

        _uploadRef.PutFileAsync(_path, _metaData).ContinueWithOnMainThread((_task) =>
        {
            if(_task.IsFaulted || _task.IsCanceled)
            {
                Debug.Log("Could not upload.  Error: " + _task.Exception.ToString());
            }
            else
            {
                Debug.Log("Finished uploading...");
            }
        });

#endif
        _uploadRef.GetDownloadUrlAsync().ContinueWithOnMainThread((_task) =>
        {
            userProfile.Add("photoURL", _task.Result.ToString());
            //Debug.Log(_task.Result.ToString());
        });

    } // END UploadPhoto


#endregion


    #region DUPLICATE TEXTURE


    //--------------------------------------------------//
    private Texture2D DuplicateTexture(Texture2D _source)
    //--------------------------------------------------//
    {
        RenderTexture _renderTex = RenderTexture.GetTemporary(
                    _source.width,
                    _source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(_source, _renderTex);

        RenderTexture _previous = RenderTexture.active;
        RenderTexture.active = _renderTex;

        Texture2D readableText = new Texture2D(_source.width, _source.height);
        readableText.ReadPixels(new Rect(0, 0, _renderTex.width, _renderTex.height), 0, 0);
        readableText.Apply();

        RenderTexture.active = _previous;
        RenderTexture.ReleaseTemporary(_renderTex);

        return readableText;

    } // END DuplicateTexture


    #endregion


    #region SET DATA


    //------------------------//
    private void SetData()
    //------------------------//
    {
        //UserDataManager.OnUserDataInitialized -= SetData;
        UserDataMeta profileData = UserDataManager.loadedData;

        for (int i = 0; i < profileNameText.Count; i++)
        {
            profileNameText[i].text = profileData.displayName.Substring(0, 1);
        }

        //Debug.Log(profileData.photoURL);
        userID = profileData.uid;

        GetPhoto();

        fullNameText.text = profileData.firstName + " " + profileData.lastName;
        nicknameText.text = profileData.nickname;
        emailText.text = profileData.email;
        countryText.text = profileData.country;

        genderText.text = profileData.gender.ToString();
        goalText.text = profileData.challenge.ToString();
        fitnessLevelText.text = profileData.fitnessLevel.ToString();

        height = profileData.height;
        heightFeet = profileData.heightFeets;
        heightInches = profileData.heightInches;
        weightImperial = profileData.weightLbs;
        weightMetric = profileData.weight;

        if (measurementSystem.isMetric == true)
        {
            weightText.text = profileData.weight.ToString() + " kg";
            heightText.text = profileData.height.ToString() + " cm";
        }
        else
        {
            weightText.text = profileData.weightLbs.ToString() + " lbs";
            heightText.text = profileData.heightFeets.ToString() + "' " + profileData.heightInches.ToString() + '"';
        }

        string[] _dateAndTime = profileData.birthday.Split('T');
        string[] _date = _dateAndTime[0].Split('-');

        // Month - Day - Year
        birthdayText.text = FormatBirthday(_date[1], _date[2], _date[0]);

        //profileButton.DOFade(1f, 0.3f).OnComplete(() => { profileButton.interactable = true; profileButton.blocksRaycasts = true; });

    } //END SetData


    //---------------------------------------------//
    private IEnumerator IDownloadImage(string _url)
    //--------------------------------------------//s
    {
        //Debug.Log("Sending Request");

        UnityWebRequest _request = UnityWebRequestTexture.GetTexture(_url);
        yield return _request.SendWebRequest();

        if (_request.result != UnityWebRequest.Result.Success)
        {
            //Debug.Log(_request.error);
            //Debug.Log(_request.result);
            //Debug.Log(_request.responseCode);
        }
        else
        {
            //Debug.Log("Creating Texture");
            Texture2D _texture = DownloadHandlerTexture.GetContent(_request);
            profileImage.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2());
        }

    } // END IDownloadImage


    //-------------------------------------------------------------------//
    private string FormatBirthday(string _month, string _day, string _year)
    //-------------------------------------------------------------------//
    {
        switch (_month)
        {
            case "01":
                birthdayText.text = "Jan" + " " + _day + " " + _year;
                break;

            case "02":
                birthdayText.text = "Feb" + " " + _day + " " + _year;
                break;

            case "03":
                birthdayText.text = "Mar" + " " + _day + " " + _year;
                break;

            case "04":
                birthdayText.text = "Apr" + " " + _day + " " + _year;
                break;

            case "05":
                birthdayText.text = "May" + " " + _day + " " + _year;
                break;

            case "06":
                birthdayText.text = "Jun" + " " + _day + " " + _year;
                break;

            case "07":
                birthdayText.text = "Jul" + " " + _day + " " + _year;
                break;

            case "08":
                birthdayText.text = "Aug" + " " + _day + " " + _year;
                break;

            case "09":
                birthdayText.text = "Sep" + " " + _day + " " + _year;
                break;

            case "10":
                birthdayText.text = "Oct" + " " + _day + " " + _year;
                break;

            case "11":
                birthdayText.text = "Nov" + " " + _day + " " + _year;
                break;

            case "12":
                birthdayText.text = "Dec" + " " + _day + " " + _year;
                break;
        }

        return birthdayText.text;

    } // END FormatBirthday


    #endregion


    #region RECEIVE DATA


    //-------------------------------------//
    public void ReceiveProfileData()
    //-------------------------------------//
    {
        UserDataManager.loadedData = null;
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

            UserDataManager.OnUserDataInitialized += SetData;
        }
        else
        {
            SetData();
        }

    } // END ReceiveProfileData


    #endregion


    #region SET EDIT PAGE DATA


    //---------------------------//
    public void SetEditPageData()
    //---------------------------//
    {
        //Debug.Log("UserProfilePage.cs // SetEditPageData() // ");
        editPageProfileImage.sprite = profileImage.sprite;

        fullNameInput.text = fullNameText.text;
        nicknameInput.text = nicknameText.text;

        genderDropdown.captionText.text = genderText.text;
        countryDropdown.captionText.text = countryText.text;
        goalDropdown.captionText.text = goalText.text;
        fitnessLevelDropdown.captionText.text = fitnessLevelText.text;
        genderDropdown.captionText.text = genderText.text;

        if(measurementSystem.isMetric == true)
        {
            heightMetricDropdown.captionText.text = heightText.text;
            weightMetricDropdown.captionText.text = weightText.text;
        }
        else
        {
            heightFeetDropdown.captionText.text = heightFeet.ToString();
            heightInchesDropdown.captionText.text = heightInches.ToString();

            string[] _heightSplit = heightText.text.Split(' ');

            heightFeetDropdown.captionText.text = _heightSplit[0].Replace("'", "");

            heightInchesDropdown.captionText.text = _heightSplit[1].Replace('"', ' ').Replace(" ", "");

            weightImperialDropdown.captionText.text = weightText.text.Replace(" lbs", "");
        }

        if (birthdayText.text.Contains("Jan"))
        {
            string[] _splitDate = birthdayText.text.Split(' ');

            monthDropdown.captionText.text = "01";
            dayDropdown.captionText.text = _splitDate[1];
            yearDropdown.captionText.text = _splitDate[2];
        }
        else if (birthdayText.text.Contains("Feb"))
        {
            string[] _splitDate = birthdayText.text.Split(' ');

            monthDropdown.captionText.text = "02";
            dayDropdown.captionText.text = _splitDate[1];
            yearDropdown.captionText.text = _splitDate[2];
        }
        else if (birthdayText.text.Contains("Mar"))
        {
            string[] _splitDate = birthdayText.text.Split(' ');

            monthDropdown.captionText.text = "03";
            dayDropdown.captionText.text = _splitDate[1];
            yearDropdown.captionText.text = _splitDate[2];
        }
        else if (birthdayText.text.Contains("Apr"))
        {
            string[] _splitDate = birthdayText.text.Split(' ');

            monthDropdown.captionText.text = "04";
            dayDropdown.captionText.text = _splitDate[1];
            yearDropdown.captionText.text = _splitDate[2];
        }
        else if (birthdayText.text.Contains("May"))
        {
            string[] _splitDate = birthdayText.text.Split(' ');

            monthDropdown.captionText.text = "05";
            dayDropdown.captionText.text = _splitDate[1];
            yearDropdown.captionText.text = _splitDate[2];
        }
        else if (birthdayText.text.Contains("Jun"))
        {
            string[] _splitDate = birthdayText.text.Split(' ');

            monthDropdown.captionText.text = "06";
            dayDropdown.captionText.text = _splitDate[1];
            yearDropdown.captionText.text = _splitDate[2];
        }
        else if (birthdayText.text.Contains("Jul"))
        {
            string[] _splitDate = birthdayText.text.Split(' ');

            monthDropdown.captionText.text = "07";
            dayDropdown.captionText.text = _splitDate[1];
            yearDropdown.captionText.text = _splitDate[2];
        }
        else if (birthdayText.text.Contains("Aug"))
        {
            string[] _splitDate = birthdayText.text.Split(' ');

            monthDropdown.captionText.text = "08";
            dayDropdown.captionText.text = _splitDate[1];
            yearDropdown.captionText.text = _splitDate[2];
        }
        else if (birthdayText.text.Contains("Sep"))
        {
            string[] _splitDate = birthdayText.text.Split(' ');

            monthDropdown.captionText.text = "09";
            dayDropdown.captionText.text = _splitDate[1];
            yearDropdown.captionText.text = _splitDate[2];
        }
        else if (birthdayText.text.Contains("Oct"))
        {
            string[] _splitDate = birthdayText.text.Split(' ');

            monthDropdown.captionText.text = "10";
            dayDropdown.captionText.text = _splitDate[1];
            yearDropdown.captionText.text = _splitDate[2];
        }
        else if (birthdayText.text.Contains("Nov"))
        {
            string[] _splitDate = birthdayText.text.Split(' ');

            monthDropdown.captionText.text = "11";
            dayDropdown.captionText.text = _splitDate[1];
            yearDropdown.captionText.text = _splitDate[2];
        }
        else if (birthdayText.text.Contains("Dec"))
        {
            string[] _splitDate = birthdayText.text.Split(' ');

            monthDropdown.captionText.text = "12";
            dayDropdown.captionText.text = _splitDate[1];
            yearDropdown.captionText.text = _splitDate[2];
        }
        
    } // END SetEditPageData


    #endregion


    #region HIDE / SHOW


    //------------------------//
    public void HidePage()
    //------------------------//
    {
        foreach (var page in pages)
        {
            page.SetActive(false);              
        }

        pages[0].SetActive(true);

        Hide();

    }//END HidePage


    //------------------------//
    public void ShowPage()
    //------------------------//
    {
        /*
        Sequence sequence = DOTween.Sequence();
        sequence.Append(profileButton.transform.DOPunchScale(new Vector3(0.3f,0.3f,0.3f), 0.2f));
        sequence.AppendCallback(() => 
        {
            Show();
        });
       */
    }//END ShowPage


    #endregion


    #region FIND ENUM VALUE


    //----------------------------//
    private int FindEnumValue(TMP_Dropdown dropdown, string stringToLookFor)
    //----------------------------//
    {
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            //Debug.Log(dropdown.options[i].text + " Looking for: " + stringToLookFor );
            if (dropdown.options[i].text == stringToLookFor)
            {
                return i + 1;
            }
        }

        return 0;

    }//END FindEnumValue


    #endregion


    #region SAVE/CANCEL/UPDATE VISUAL


    //----------------//
    public void Save()
    //----------------//
    {
        if(userProfile.Count != 0)
        {
            UpdateProfile();
            UpdateVisual();

            //Debug.Log("UserProfilePage.cs // Save()");
        }

    } // END Save


    //------------------//
    public void Cancel()
    //------------------//
    {
        userProfile.Clear();

    } // END Cancel


    //------------------//
    private void UpdateVisual()
    //------------------//
    {
        if(measurementSystem.isMetric == true)
        {
            weightText.text = weightMetricDropdown.captionText.text + " kg";
            heightText.text = heightMetricDropdown.captionText.text + " cm";
        }
        else
        {
            weightText.text = weightImperialDropdown.captionText.text + " lbs";
            heightText.text = heightFeetDropdown.captionText.text + "' " + heightInchesDropdown.captionText.text + '"';
        }

        genderText.text = genderDropdown.captionText.text;
        countryText.text = countryDropdown.captionText.text;
        birthdayText.text = FormatBirthday(monthDropdown.captionText.text, dayDropdown.captionText.text, yearDropdown.captionText.text);

        string[] _nameSplit = fullNameInput.text.Split(' ');

        if (fullNameInput.text.Contains(" "))
        {
            fullNameText.text = _nameSplit[0] + " " + _nameSplit[1];

            //Debug.Log("firstName: " + _nameSplit[0]);
            //Debug.Log("lastName: " + _nameSplit[1]);
        }
        else
        {
            fullNameText.text = fullNameInput.text;
            //Debug.Log("UserProfilePage.cs // SetName() // firstName: " + fullNameInput.text);
        }

        string[] _nicknameSplit = nicknameInput.text.Split(' ');

        if (nicknameInput.text.Contains(" "))
        {
            nicknameText.text = _nicknameSplit[0] + " " + _nicknameSplit[1];

            //Debug.Log("firstName: " + _nicknameSplit[0]);
            //Debug.Log("lastName: " + _nicknameSplit[1]);
        }
        else
        {
            nicknameText.text = nicknameInput.text;

            //Debug.Log("UserProfilePage.cs // SetName() // firstName: " + nicknameInput.text);
        }

        if (goalDropdown.captionText.text == "Burn")
        {
            goalText.text = "Burn";
            //Debug.Log("Challenge value :Burn");
        }
        else if (goalDropdown.captionText.text == "Tone")
        {
            goalText.text = "Tone";
            //Debug.Log("Challenge value :Tone");
        }
        else if (goalDropdown.captionText.text == "Build")
        {
            goalText.text = "Build";
            //Debug.Log("Challenge value :Build");
        }

        if (fitnessLevelDropdown.captionText.text.Contains("Starter"))
        {
            fitnessLevelText.text = "Starter";
            //Debug.Log("UserProfile.cs // UpdateVisual() // :Starter");
        }
        else if (fitnessLevelDropdown.captionText.text.Contains("Casual"))
        {
            fitnessLevelText.text = "Casual";
            //Debug.Log("UserProfile.cs // UpdateVisual() // :Casual");
        }
        else if (fitnessLevelDropdown.captionText.text.Contains("Serious"))
        {
            fitnessLevelText.text = "Serious";
            //Debug.Log("UserProfile.cs // UpdateVisual() // :Serious");
        }
        else if (fitnessLevelDropdown.captionText.text.Contains("Elite"))
        {
            fitnessLevelText.text = "Elite";
            //Debug.Log("UserProfile.cs // UpdateVisual() // :Elite");
        }

    } // END UpdateVisual


#endregion


    #region UPDATE PROFILE


    //--------------------------//
    public void UpdateProfile()
    //--------------------------//
    {
        DocumentReference _docRef = FirestoreDatabaseManager.db.Collection("users").Document(AuthenticationManager.Instance.user.UserId);

        _docRef.SetAsync(userProfile, SetOptions.MergeAll).ContinueWithOnMainThread(_task => {
            //Debug.Log("Added updated data to the Firebase account.");
        });

    } // END UpdateProfile


#endregion


    #region SET DATA FIELDS


    /// <summary>
    /// Ex: 1967-02-09T01:56:21.844Z
    /// </summary>
    //-------------------------//
    public string GetBirthday()
    //------------------------//
    {
        DateTime date = DateTime.Now; // store 2011-06-27 12:00:00
        
        if (DateTime.TryParse(yearDropdown.captionText.text + "-" + monthDropdown.captionText.text + "-" + dayDropdown.captionText.text, out date))
        {
            //Debug.Log("UserEnteredData//GetBirthday// bday parsed " + date.ToString("yyyy/MM/dd"));
        }
        else
        {
            //Debug.Log("UserEnteredData//GetBirthday// bday not parsed");
        }

        return date.ToString("yyyy/MM/dd" + "T08:09:28.857Z");

    } // END GetBirthday


    // <summary>
    /// Ex: 1967-02-09T01:56:21.844Z
    /// </summary>
    //-------------------------//
    public string FormatBirthdayForFirbase()
    //------------------------//
    {
        DateTime date = DateTime.Now; // store 2011-06-27 12:00:00
        
        if (DateTime.TryParse(yearDropdown.captionText.text + "-" + monthDropdown.captionText.text + "-" + dayDropdown.captionText.text, out date))
        {
            //Debug.Log("UserEnteredData//GetBirthday// bday parsed " + date.ToString("yyyy/MM/dd"));
        }
        else
        {
            //Debug.Log("UserEnteredData//GetBirthday// bday not parsed");
        }

        return date.ToString("yyyy/MM/dd" + "T08:09:28.857Z");

    } // END GetBirthday


    //------------------------//
    public void SetBirthday()
    //------------------------//
    {
        if(userProfile.ContainsKey("birthday"))
        {
            userProfile.Remove("birthday");
        }
        
        Debug.Log(GetBirthday().Replace("/", "-"));

        userProfile.Add("birthday", GetBirthday().Replace("/", "-"));

    } // END SetBirthday


    //---------------------//
    public void SetWeightKg()
    //---------------------//
    {
        if(userProfile.ContainsKey("weight"))
        {
            userProfile.Remove("weight");
        }
        
        userProfile.Add("weight", Convert.ToInt32(weightMetricDropdown.captionText.text));

        //Debug.Log("Edited Weight");

    } // END SetWeight


    //------------------------//
    public void SetWeightLbs()
    //-----------------------//
    {
        if(userProfile.ContainsKey("weightLbs"))
        {
            userProfile.Remove("weightLbs");
        }
        
        userProfile.Add("weightLbs", Convert.ToInt32(weightImperialDropdown.captionText.text));

    } // END SetWeightLbs


    //---------------------//
    public void SetHeightCM()
    //---------------------//
    {
        if (userProfile.ContainsKey("height"))
        {
            userProfile.Remove("height");
        }

        userProfile.Add("height", Convert.ToInt32(heightMetricDropdown.captionText.text));

    } // END SetHeight


    //-------------------------//
    public void SetHeightFeet()
    //------------------------//
    {
        if(userProfile.ContainsKey("heightFeets"))
        {
            userProfile.Remove("heightFeets");
        }

        userProfile.Add("heightFeets", Convert.ToInt32(heightFeetDropdown.captionText.text));

    } // END SetHeightFeet


    //---------------------------//
    public void SetHeightInches()
    //---------------------------//
    {
        if (userProfile.ContainsKey("heightInches"))
        {
            userProfile.Remove("heightInches");
        }

        userProfile.Add("heightInches", Convert.ToInt32(heightFeetDropdown.captionText.text));

    } // END SetHeightInches


    //----------------------//
    public void SetCountry()
    //---------------------//
    {
        if(userProfile.ContainsKey("country"))
        {
            userProfile.Remove("country");
        }
        
        userProfile.Add("country", countryDropdown.captionText.text);

    } // END SetCountry


    //----------------------//
    public void SetNickname()
    //----------------------//
    {
        if(userProfile.ContainsKey("nickname"))
        {
            userProfile.Remove("nickname");
        }

        userProfile.Add("nickname", nicknameInput.text);

        //Debug.Log("UserProfile.cs // SetNickname() // ");

    } // END SetNickName


    //------------------------//
    public void SetName()
    //------------------------//
    {
        if(userProfile.ContainsKey("firstName"))
        {
            userProfile.Remove("firstName");
        }
        
        if(userProfile.ContainsKey("lastName"))
        {
            userProfile.Remove("lastName");
        }

        string[] _nameSplit = fullNameInput.text.Split(' ');

        if (fullNameInput.text.Contains(" "))
        {
            userProfile.Add("firstName", _nameSplit[0]);
            userProfile.Add("lastName", _nameSplit[1]);

            fullNameText.text = _nameSplit[0] + " " + _nameSplit[1];

            //Debug.Log("firstName: " + _nameSplit[0]);
            //Debug.Log("lastName: " + _nameSplit[1]);
        }
        else
        {
            userProfile.Add("firstName", fullNameInput.text);
            userProfile.Add("lastName", "");

            fullNameText.text = fullNameInput.text;

            //Debug.Log("UserProfilePage.cs // SetName() // firstName: " + fullNameInput.text);
        }

    } // END SetFirstName


    //-------------------//
    public void SetGoal()
    //-------------------//
    {
        int _goal = 0;
        
        if (userProfile.ContainsKey("challenge"))
        {
            userProfile.Remove("challenge");
        }

        if (goalDropdown.captionText.text == "Burn")
        {
            _goal = 1;
            //Debug.Log("Challenge value " + _goal + ":Burn");
        }
        else if (goalDropdown.captionText.text == "Tone")
        {
            _goal = 2;
            //Debug.Log("Challenge value " + _goal + ":Tone");
        }
        else if (goalDropdown.captionText.text == "Build")
        {
            _goal = 3;
            //Debug.Log("Challenge value " + _goal + ":Build");
        }

        userProfile.Add("challenge", _goal);

    } // END SetGoal


    //---------------------------//
    public void SetFitnessLevel()
    //---------------------------//
    {
        int _fitnessLevel = 0;
        
        if(userProfile.ContainsKey("fitnessLevel"))
        {
            userProfile.Remove("fitnessLevel");
        }

        if(fitnessLevelDropdown.captionText.text.Contains("Starter"))
        {
            _fitnessLevel = 4;
            //Debug.Log("UserProfile.cs // SetGender() // " + _fitnessLevel + " " + ":Starter");
        }
        else if (fitnessLevelDropdown.captionText.text.Contains("Casual"))
        {
            _fitnessLevel = 3;
            //Debug.Log("UserProfile.cs // SetGender() // " + _fitnessLevel + " " + ":Casual");
        }
        else if (fitnessLevelDropdown.captionText.text.Contains("Serious"))
        {
            _fitnessLevel = 2;
            //Debug.Log("UserProfile.cs // SetGender() // " + _fitnessLevel + " " + ":Serious");
        }
        else if (fitnessLevelDropdown.captionText.text.Contains("Elite"))
        {
            _fitnessLevel = 1;
            //Debug.Log("UserProfile.cs // SetGender() // " + _fitnessLevel + " " + ":Elite");
        }

        userProfile.Add("fitnessLevel", _fitnessLevel);

    } // END SetFitnessLevel


    //--------------------//
    public void SetGender()
    //--------------------//
    {
        int _gender = 0;
        
        if(userProfile.ContainsKey("gender"))
        {
            userProfile.Remove("gender");
        }

        if(genderDropdown.captionText.text == "Female")
        {
            _gender = 1;
            //Debug.Log("UserProfile.cs // SetGender() // " + _gender + " " + ":Female");
        }
        else
        {
            _gender = 2;
            //Debug.Log("UserProfile.cs // SetGender() // " + _gender + " " + ":Male");
        }

        userProfile.Add("gender", _gender);

    } // END SetGender


#endregion


} // END UserProfilePage.cs