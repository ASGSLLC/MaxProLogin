using UnityEngine.SceneManagement;
using maxprofitness.login;
#if FIREBASE_AUTH
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using Firebase;
using Firebase.Auth;

#if TMP
using TMPro;
#endif

#if GOOGLE
using Google;
#endif

#if FACEBOOK
using Facebook.Unity;
#endif

#if APPLE
using AppleAuth;
using AppleAuth.Native;
using AppleAuth.Enums;
using AppleAuth.Interfaces;
using AppleAuth.Extensions;
#endif
using maxprofitness.shared;

public class AuthenticationManager : Singleton<AuthenticationManager>
{

#region VARIABLES
    public static Action<FirebaseUser> OnAuthenticationStateChanged;

    public bool sendVerificationEmailOnUserRegisterd = true;

    public FirebaseAuth auth;
    public FirebaseUser user;

    public string webClientId;

    private string email;
    private string password;
    private Credential credential;
    private UserDataManager userDataManager;

#if APPLE
    private IAppleAuthManager appleAuthManager;
    private string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private System.Random random = new System.Random();
#endif
#endregion

#region START UP LOGIC
    //----------------------------------//
    protected override void Awake()
    //----------------------------------//
    {
        //Debug.Log("AuthenticationManager//Awake// " + webClientId);

        base.Awake();

        StartCoroutine(CheckDependecies());

#if GOOGLE
        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            RequestIdToken = true,
            RequestEmail = true,
            WebClientId = webClientId
        };
#endif

#if FACEBOOK
        if (!FB.IsInitialized)
        {
            FB.Init(() =>
            {
                if (!FB.IsInitialized)
                    FB.ActivateApp();
            },
            (isgameshown) =>
            {
                if (!isgameshown)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;
            });
        }
        else
        {
            FB.ActivateApp();
        }
#endif

#if APPLE
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            var deserializer = new PayloadDeserializer();
            this.appleAuthManager = new AppleAuthManager(deserializer);
        }
#endif

    } // END Awake
#endregion

#region GET USER PROVIDERS
    //----------------------------------//
    public Coroutine GetUserProviders(string email, Action<List<string>> OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    { 
        return StartCoroutine(IGetUserProviders(email, OnSuccess, OnError));

    } // END GetUserProviders

    //----------------------------------//
    private IEnumerator IGetUserProviders(string email, Action<List<string>> OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        yield return StartCoroutine(CheckDependecies());

        Debug.Log("AuthenticationManager//IGetUserProviders// email " + email);

        var task = auth.FetchProvidersForEmailAsync(email);

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null || task.IsCanceled)
        {
            string error = FirebaseExeptionHelper.HandleErrorExeption(task.Exception);

            OnError?.Invoke(error);
            Debug.LogError("AuthenticationManager//IGetUserProviders// error: " + error);
            yield break;
        }
        else
        {
            List<string> providerResults = new List<string>();

            foreach (string s in task.Result)
            {
                Debug.Log("AuthenticationManager//IGetUserProviders// " + s);
                providerResults.Add(s);
            }

            OnSuccess?.Invoke(providerResults);
        }

    } // END IGetUserProviders
#endregion

#region SOCIAL AUTHENTICATION
#if GOOGLE
    //----------------------------------//
    public Coroutine RequestGoogleCredentials(Action<GoogleSignInUser, Credential> OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        return StartCoroutine(IRequestGoogleCredentials(OnSuccess, OnError));

    } // END RequestGoogleCredentials

    //----------------------------------//
    private IEnumerator IRequestGoogleCredentials(Action<GoogleSignInUser, Credential> OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        Debug.Log("AthenticationManager//IGetGoogleCredentials//");

        yield return StartCoroutine(CheckDependecies());
        Debug.Log("AthenticationManager//IGetGoogleCredentials// Checked Dependecies");
        var signInTask = GoogleSignIn.DefaultInstance.SignIn();

        Debug.Log("AthenticationManager//IGetGoogleCredentials// waiting");

        yield return new WaitUntil(() => signInTask.IsCompleted);

        Debug.Log("AthenticationManager//IGetGoogleCredentials// task done");

        if (signInTask.IsCanceled || signInTask.IsFaulted)
        {
            Debug.Log("AuthenticationManager//IGetGoogleCredentials// error");

            using (IEnumerator<Exception> enumerator = signInTask.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.LogError("AuthenticationManager//IGetGoogleCredentials// " + error.Status + " " + error.Message);
                    OnError?.Invoke(error.Status + " " + error.Message);
                }
                else
                {
                    OnError?.Invoke(signInTask.Exception.ToString());
                }

                Debug.LogError("AuthenticationManager//IGetGoogleCredentials// Exception " + signInTask.Exception.ToString());

            }
        }
        else
        {
            yield return StartCoroutine(CheckDependecies());

            GoogleSignInUser user = signInTask.Result;

            Debug.Log("AuthenticationManager//IGetGoogleCredentials// have auth");

            Debug.Log("AuthenticationManager//IGetGoogleCredentials// have google id " + user.IdToken);
            Debug.Log("AuthenticationManager//IGetGoogleCredentials// email " + user.Email);

            //email = signInTask.Result.Email;

            Credential credential = GoogleAuthProvider.GetCredential(user.IdToken, null);

            OnSuccess?.Invoke(user, credential);
        }

    } // END IRequestGoogleCredentials
#endif

#if FACEBOOK
    //----------------------------------//
    public Coroutine RequestFacebookCredentials(Action<Credential> OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        return StartCoroutine(IRequestFacebookCredentials(OnSuccess, OnError));

    } // END RequestFacebookCredentials

    //----------------------------------//
    private IEnumerator IRequestFacebookCredentials(Action<Credential> OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        Debug.Log("AuthenticationManager//Facebook_Login// ");

        yield return StartCoroutine(CheckDependecies());

        var permission = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(permission, (result) =>
        {
            if (FB.IsLoggedIn)
            {
                Credential credentials = FacebookAuthProvider.GetCredential(AccessToken.CurrentAccessToken.TokenString);

                OnSuccess?.Invoke(credentials);

                Debug.Log("AuthenticationManager//AuthCallBack//IsLoggedIn");
            }
            else
            {
                OnError(result.Error);
                Debug.LogError("AuthenticationManager//AuthCallBack// " + result.Error);
            }
        });

    } // END IRequestFacebookCredentials
#endif

#if APPLE
    //----------------------------------//
    public void RequestAppleCredentials(Action<Credential, IAppleIDCredential> OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        Debug.Log("AuthenticationManager//RequestAppleCredentials// ");

        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

        this.appleAuthManager.LoginWithAppleId(
            loginArgs,
            (credential) =>
            {
                Debug.Log("AuthenticationManager//RequestAppleCredentials// cred user " + credential.User);

                // Obtained credential, cast it to IAppleIDCredential
                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                {
                    // Apple User ID
                    // You should save the user ID somewhere in the device
                    var userId = appleIdCredential.User;
                            PlayerPrefs.SetString("AppleUserIdKey", userId);

                    // Email (Received ONLY in the first login)
                    var email = appleIdCredential.Email;

                    // Full name (Received ONLY in the first login)
                    var fullName = appleIdCredential.FullName;

                    Debug.Log("AuthenticationManager//RequestAppleCredentials// fullName " + fullName);

                    // Identity token
                    var identityToken = Encoding.UTF8.GetString(
                                appleIdCredential.IdentityToken,
                                0,
                                appleIdCredential.IdentityToken.Length);

                    // Authorization code
                    var authorizationCode = Encoding.UTF8.GetString(
                                appleIdCredential.AuthorizationCode,
                                0,
                                appleIdCredential.AuthorizationCode.Length);

                    Debug.Log("AuthenticationManager//RequestAppleCredentials// making credentials");

                    // And now you have all the information to create/login a user in your system
                    Credential cred = Firebase.Auth.OAuthProvider.GetCredential("apple.com", identityToken, GenerateNonce(),  null);

                    Debug.Log("AuthenticationManager//RequestAppleCredentials// returning credentials");

                    OnSuccess?.Invoke(cred, appleIdCredential);
                    return;
                }

                Debug.LogError("AuthenticationManager//RequestAppleCredentials// credetials null");

                OnError?.Invoke("AuthenticationManager//RequestAppleCredentials// credetials null");
            },
            (error) =>
            {
                // Something went wrong
                var authorizationErrorCode = error.GetAuthorizationErrorCode();

                Debug.LogError("AuthenticationManager//RequestAppleCredentials// " + authorizationErrorCode.ToString());

                OnError?.Invoke(authorizationErrorCode.ToString());
            });

    } // END RequestAppleCredentials

    //----------------------------------//
    private string GenerateNonce(int length = 32)
    //----------------------------------//
    {
        var nonceString = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            nonceString.Append(validChars[random.Next(0, validChars.Length - 1)]);
        }

        return nonceString.ToString();

    } // END GenerateNonce

    //----------------------------------//
    private void Update()
    //----------------------------------//
    {
        if (this.appleAuthManager != null)
        {
            this.appleAuthManager.Update();
        }

    } // END Update
#endif
#endregion

#region HAS CREDENTIALS
    public bool HasCredentials()
    {
        if (credential != null)
            return true;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

#endregion

#region LOGIN 
    //----------------------------------//
    public Coroutine Login(string _email, string _password, Action OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        email = _email;
        password = _password;

        return StartCoroutine(ILogin(_email, _password, OnSuccess, OnError));

    } // END Login

    //----------------------------------//
    private IEnumerator ILogin(string _email, string _password, Action OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        //Debug.Log("AuthenticationManager//ILogin//");

        yield return StartCoroutine(CheckDependecies());

        var task = auth.SignInWithEmailAndPasswordAsync(_email, _password);

        yield return new WaitUntil(() => task.IsCompleted);

        //Debug.Log("AuthenticationManager//ILogin// task complete");

        if (task.Exception != null || task.IsCanceled)
        {
            string error = FirebaseExeptionHelper.HandleErrorExeption(task.Exception);

            OnError?.Invoke(error);
            //Debug.LogError("AuthenticationManager//ILogin// error: " + error);
        }
        else
        {
            user = task.Result.User;
            //email = _email;
            //password = _password;

            OnSuccess?.Invoke();
            
            //Debug.Log("UserDataManager is null");

            if (UserDataManager.Instance == null)
            {
                Debug.Log("UserDataManager is null");
                yield break;
            }

            UserDataManager userDataManager = FindObjectOfType<UserDataManager>();
            UserDataMeta profileData = UserDataManager.loadedData;
            
            Debug.Log("Checking if ProfileData is null");

            if (profileData == null)
            {
                Debug.Log("Grabbing ProfileData");
                //Grab User Data
                userDataManager.InitializeUserData((snapShot) =>
                {
                    Debug.Log("UserProfilePage.cs // ReceiveProfileData() // Successfully got data");
                }, (error) =>
                {
                    Debug.Log("UserProfilePage.cs // ReceiveProfileData() // ERROR: Was not able to initialize user data");
                });

                //UserDataManager.OnUserDataInitialized += SetData;
            }
            else
            {
                Debug.Log("We have profileData");
            }
            //Debug.Log("AuthenticationManager//ILogin// Logged In");
        }

    } // END ILogin

    //----------------------------------//
    public Coroutine SignInWithCredentials(Credential credential, Action<FirebaseUser> OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    { 
        return StartCoroutine(ISignInWithCredentials(credential, OnSuccess, OnError));

    } // END SignInWithCredentials

    //----------------------------------//
    private IEnumerator ISignInWithCredentials(Credential credential, Action<FirebaseUser> OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        //Debug.Log("AuthenticationManager//ISignInWithCredentials//");

        yield return StartCoroutine(CheckDependecies());

        this.credential = credential;

        var task = auth.SignInWithCredentialAsync(credential);

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsCanceled || task.IsFaulted)
        {
            string error = FirebaseExeptionHelper.HandleErrorExeption(task.Exception);

            //Debug.Log("AuthenticationManager//ISignInWithCredentials//error " + error);

            OnError?.Invoke(error);
        }
        else
        {
            //Debug.Log("AuthenticationManager//ISignInWithCredentials//success ");

            user = task.Result;

            if (sendVerificationEmailOnUserRegisterd == true && user.IsEmailVerified == false)
            {
                user.SendEmailVerificationAsync();
            }

            OnSuccess?.Invoke(task.Result);
        }

    } // END ISignInWithCredentials
#endregion

#region REGISTER USER
    //----------------------------------//
    public Coroutine Register(string _email, string _password, string _verifyPass,
                             Action<FirebaseUser> OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        return StartCoroutine(IRegister(_email, _password, _verifyPass, OnSuccess, OnError));

    } // END Register

    //----------------------------------//
    private IEnumerator IRegister(string _email, string _password, string _verifyPass,
                                 Action<FirebaseUser> OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        if (_password != _verifyPass)
        {
            OnError?.Invoke("Password Does Not Match!");
        }
        else
        {
            yield return StartCoroutine(CheckDependecies());

            var task = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
          
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.IsFaulted || task.IsCanceled)
            {
                string error = FirebaseExeptionHelper.HandleErrorExeption(task.Exception);

                OnError?.Invoke(error);
                Debug.LogError("AuthenticationManager//IRegister// error: " + error);
            }
            else
            { 
                //User has now been created
                user = task.Result.User;

                if (user != null)
                {
                   // UserProfile profile = new UserProfile { DisplayName = _username };

                    //UpdateUserProfile(profile);

                    if (sendVerificationEmailOnUserRegisterd == true)
                        SendEmailVerification();

                    OnSuccess?.Invoke(user);
                }
            } 
        }

    } // END IRegister
#endregion

#region UPDATE USER PROFILE
    //----------------------------------//
    public Coroutine UpdateUserProfile(UserProfile updatedUserProfile, Action<FirebaseUser> OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        return StartCoroutine(IUpdateUserProfile(updatedUserProfile, OnSuccess, OnError));

    } // END UpdateUserProfile

    //----------------------------------//
    private IEnumerator IUpdateUserProfile(UserProfile updatedUserProfile, Action<FirebaseUser> OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        var task = user.UpdateUserProfileAsync(updatedUserProfile);

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted || task.IsCanceled)
        {
            string error = FirebaseExeptionHelper.HandleErrorExeption(task.Exception);

            OnError?.Invoke(error);
            Debug.LogError("AuthenticationManager//IUpdateUserProfile// error: " + error);
        }
        else
        {
            //Username is now set
            OnSuccess?.Invoke(user);
        }

    } // END IUpdateUserProfile
#endregion

#region EMAIL LOGIC
    //----------------------------------//
    public Coroutine UpdateEmail(string email, Action OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        return StartCoroutine(IUpdateEmail(email, OnSuccess, OnError));

    } // END UpdateEmail

    //----------------------------------//
    private IEnumerator IUpdateEmail(string email, Action OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        Debug.Log("AuthenticationManager//ILoginWithGoogle// firebase account has no email");

        var updateEmailTask = user.UpdateEmailAsync(email);

        yield return new WaitUntil(() => updateEmailTask.IsCompleted);

        if (updateEmailTask.IsFaulted || updateEmailTask.IsCanceled)
        {
            string error = FirebaseExeptionHelper.HandleErrorExeption(updateEmailTask.Exception);

            Debug.LogError("AuthenticationManager//UpdateEmailAsync// " + error);

            OnError?.Invoke(error);
        }
        else
        {
            Debug.Log("AuthenticationManager//UpdateEmailAsync//OnSuccess " + user.Email);

            OnSuccess?.Invoke();
        }

    } // END IUpdateEmail

    //----------------------------------//
    public Coroutine SendEmailVerification(Action OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        Debug.Log("AuthenticationManager//SendEmailVerification");

        return StartCoroutine(ISendEmailVerification(OnSuccess, OnError));

    } // END SendEmailVerification

    //----------------------------------//
    private IEnumerator ISendEmailVerification(Action OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        var task = user.SendEmailVerificationAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted || task.IsCanceled)
        {
            string error = FirebaseExeptionHelper.HandleErrorExeption(task.Exception);

            Debug.LogError("AuthenticationManager//SendEmailVerificationAsync//error: " + error);
            OnError?.Invoke(error);
        }
        else
        {
            OnSuccess?.Invoke();
        }

    } // END ISendEmailVerification
#endregion

#region PASSWORD RESET
    //----------------------------------//
    public Coroutine SendResetPasswordEmail(string emailAddress, Action OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        Debug.Log("AuthenticationManager//SendResetPasswordEmail");

        return StartCoroutine(ISendResetPasswordEmail(emailAddress, OnSuccess, OnError));

    } // END SendResetPasswordEmail

    //----------------------------------//
    private IEnumerator ISendResetPasswordEmail(string emailAddress, Action OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        Debug.Log("AuthenticationManager//SendResetPasswordEmail// email " + emailAddress);

        if (string.IsNullOrEmpty(emailAddress) == true)
        {
            OnError?.Invoke("Missing Email");
            yield break;
        }

        yield return StartCoroutine(CheckDependecies());

        var task = auth.SendPasswordResetEmailAsync(emailAddress);

        yield return new WaitUntil(predicate: () => task.IsCompleted);

        if (task.IsFaulted || task.IsCanceled)
        {
            string error = FirebaseExeptionHelper.HandleErrorExeption(task.Exception);

            Debug.LogError("AuthenticationManager//SendResetPasswordEmail// encountered an error: " + error);
            OnError?.Invoke(error);
        }
        else
        {
            OnSuccess?.Invoke();

            Debug.Log("AuthenticationManager//SendResetPasswordEmail//Password reset email sent successfully.");
        }

    } // END SendResetPasswordEmail

    //----------------------------------//
    public Coroutine UpdatePassword(string password, Action OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        return StartCoroutine(IUpdatePassword(password, OnSuccess, OnError));

    } // END UpdatePassword

    //----------------------------------//
    private IEnumerator IUpdatePassword(string password, Action OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        Debug.Log("AuthenticationManager//ILoginWithGoogle// firebase account has no email");

        var task = user.UpdatePasswordAsync(password);

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted || task.IsCanceled)
        {
            string error = FirebaseExeptionHelper.HandleErrorExeption(task.Exception);

            Debug.LogError("AuthenticationManager//UpdateEmailAsync// " + error);

            OnError?.Invoke(error);
        }
        else
        {
            Debug.Log("AuthenticationManager//UpdateEmailAsync//OnSuccess ");
            this.password = password;
            OnSuccess?.Invoke();
        }

    } // END IUpdatePassword
#endregion

#region REAUTHENTICATION
    //----------------------------------//
    public Coroutine ReAuthenticateUser(string email, string password ,Action OnSucess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        return StartCoroutine(IReAuthenticateUser(email, password, OnSucess, OnError));

    } // END ReAuthenticateUser

    //----------------------------------//
    public Coroutine ReAuthenticateUser(Action OnSucess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        return StartCoroutine(IReAuthenticateUser("", "", OnSucess, OnError));
        
    } // END ReAuthenticateUser

    //----------------------------------//
    private IEnumerator IReAuthenticateUser(string email, string password, Action OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        if (this.credential == null)
        {
            Firebase.Auth.Credential credential =
                Firebase.Auth.EmailAuthProvider.GetCredential(email, password);

            this.credential = credential;
        }

        yield return StartCoroutine(CheckDependecies());

        if (auth.CurrentUser != null)
        {
            var task = auth.CurrentUser.ReauthenticateAsync(this.credential);

            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Exception != null || task.IsCanceled)
            {
                string error = FirebaseExeptionHelper.HandleErrorExeption(task.Exception);

                OnError?.Invoke(error);
                Debug.LogError("AuthenticationManager//IReAuthenticateUser// error: " + error);
                yield break;
            }
            else
            {
                OnSuccess?.Invoke();
            }
        }

    } // END IReAuthenticateUser
#endregion

#region SIGN OUT 
        //----------------------------------//
        public void SignOut()
        //----------------------------------//
        {
            Debug.Log("AuthenticationManager//SignOut");
        
            auth?.SignOut();

#if GOOGLE && !UNITY_EDITOR
            GoogleSignIn.DefaultInstance.SignOut();
#endif

#if FACEBOOK
            FB.LogOut();
#endif


            user = null;
        UserDataManager.loadedData = null;
        SceneManager.LoadScene(0);
        } // END SignUp
#endregion

#region DELETE USER
    //----------------------------------//
    public Coroutine DeleteUser(Action OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        return StartCoroutine(IDeleteUser(OnSuccess, OnError));

    } // END DeleteUser

    //----------------------------------//
    private IEnumerator IDeleteUser(Action OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        yield return StartCoroutine(CheckDependecies());

        yield return ReAuthenticateUser();

        var task = user.DeleteAsync();

        yield return new WaitUntil(predicate: () => task.IsCompleted);

        if (task.IsFaulted || task.IsCanceled)
        {
            string error = FirebaseExeptionHelper.HandleErrorExeption(task.Exception);

            Debug.LogError("AuthenticationManager//DeleteUser// error: " + error);

            OnError?.Invoke(error);
        }
        else
        {
            Debug.Log("AuthenticationManager//DeleteUser// complete ");
            OnSuccess?.Invoke();
        }

    } // END IDownloadToMemmory
#endregion

#region CHECK DEPENDENCIES
    //----------------------------------//
    private IEnumerator CheckDependecies()
    //----------------------------------//
    {
        Debug.Log("AuthenticationManger.cs // CheckDependencies() // Start . . .");
        if (auth == null)
        {
            Debug.Log("AuthenticationManger.cs // CheckDependencies() // auth null");
            FirebaseDependencyHelper.CheckAndFixDependenciesAsync(() =>
            {
                auth = FirebaseAuth.DefaultInstance;
            });
        }
           

        while (auth == null) { Debug.Log("AuthenticationManger.cs // CheckDependencies() // auth STILL null"); yield return null; }

        auth.StateChanged -= OnAuthStateChanged;
        auth.StateChanged += OnAuthStateChanged;

    } // END CheckDependecies
#endregion

#region ON AUTH STATE CHANGE
    //----------------------------------//
    private void OnAuthStateChanged(object sender, EventArgs eventArgs)
    //----------------------------------//
    {
        Debug.Log("AuthenticationManager//OnAuthStateChanged");

       // if (auth.CurrentUser != user)
        //{
        bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

        user = auth.CurrentUser;

        if (!signedIn && user != null)
        {
            Debug.Log("Signed out " + user.UserId);
            Debug.Log("IsEmailVerified " + user.IsEmailVerified);
            Debug.Log("Email " + user.Email);
            Debug.Log("DisplayName " + user.DisplayName);
        }
        if (signedIn)
        {
            Debug.Log("Signed in " + user.UserId);
            Debug.Log("IsEmailVerified " + user.IsEmailVerified);
            Debug.Log("Email " + user.Email);
            Debug.Log("DisplayName " + user.DisplayName);
        }
        
        OnAuthenticationStateChanged?.Invoke(user);
        //}

    } // END OnAuthStateChanged
#endregion

#region ON DESTROY / ON APP QUIT
    //----------------------------------//
    protected override void OnDestroy()
    //----------------------------------//
    {
        if (auth != null)
        {
            //auth.StateChanged -= OnAuthStateChanged;

            //auth = null;
        }

    } // END Destroy

    //----------------------------------//
    protected override void OnApplicationQuit()
    //----------------------------------//
    {
        //SignOut();

    } // END OnApplicationQuit
#endregion

} // END Class
#endif