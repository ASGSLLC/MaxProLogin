//using _Project.App.Scripts.Application.Account;
//using MaxProFitness.App.UI;
//using MaxProFitness.Backend;
//using MaxProFitness.SharedSound;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using maxprofitness.login;

    public class RegisterCanvasController : CanvasControllerBase
    {
        public delegate void SendAccountHandler(AccountBasic account);

        public event SendAccountHandler OnPressRegisterButton;

        [Header("Sub-Screens")]
        [SerializeField] private GameObject _registerPanel;
        [SerializeField] private GameObject _emailSentPanel;

        [Header("Input Fields")]
        [SerializeField] private TMP_InputField _usernameField;
        [SerializeField] private TMP_InputField _emailField;
        [SerializeField] private TMP_InputField _passwordField;
        [SerializeField] private TMP_InputField _passwordConfirmationField;

        [Header("Input Field Error Messages")]
        [SerializeField] private TMP_Text _usernameErrorMessageText;
        [SerializeField] private TMP_Text _emailErrorMessageText;
        [SerializeField] private TMP_Text _passwordErrorMessageText;
        [SerializeField] private TMP_Text _passwordConfirmationErrorMessageText;

        [Header("API Error Messages")]
        [SerializeField] private TMP_Text _apiErrorMessage;

        [Header("Buttons")]
        [SerializeField] private Button _registerButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _emailSentLoginButton;

        private const string EmailFieldEmptyMessage = "Email field is empty";
        private const string UsernameFieldEmptyMessage = "Username field is empty";
        private const string WeakPasswordMessage = "Password must contain at least 6 characters";
        private const string PasswordsDoNotMatchMessage = "Passwords do not match";

        private void Start()
        {
            SubscribeToEvents();
            Setup();
        }

        private void OnEnable()
        {
            _emailSentPanel.SetActive(false);
            _registerPanel.SetActive(true);

            ResetRegisterTexts();
            ResetErrorMessages();
        }

        private void Setup()
        {
            ResetErrorMessages();
        }

        private void SubscribeToEvents()
        {
            _registerButton.onClick.AddListener(HandleRegisterButtonClicked);
            _backButton.onClick.AddListener(HandleBackButtonPressed);
            _emailSentLoginButton.onClick.AddListener(HandleEmailSentLoginButtonPressed);
        }

        public void HandleRegisterAttemptResult(PlayfabResultCallback registerResult)
        {
            bool registerSuccessful = registerResult.ActionSucceded;
            string registerMessage = registerResult.ReportMessage;

            UnityEngine.Debug.Log($"[{typeof(RegisterCanvasController)}] - success: {registerSuccessful} - {registerMessage}");

            _registerButton.interactable = true;

            if (!registerSuccessful)
            {
                _apiErrorMessage.text = registerMessage;
                return;
            }

            _emailSentPanel.SetActive(true);
            _registerPanel.SetActive(false);
        }

        private void ResetRegisterTexts()
        {
            _emailField.text = string.Empty;
            _usernameField.text = string.Empty;
            _passwordField.text = string.Empty;
            _passwordConfirmationField.text = string.Empty;
        }

        private void ResetErrorMessages()
        {
            _usernameErrorMessageText.text = string.Empty;
            _emailErrorMessageText.text = string.Empty;
            _passwordErrorMessageText.text = string.Empty;
            _passwordConfirmationErrorMessageText.text = string.Empty;

            _apiErrorMessage.text = string.Empty;
        }

        private void HandleEmailSentLoginButtonPressed()
        {
            ChangeCanvas(CanvasType, CanvasType.Login);
            SoundManager.PlaySound(SharedGameSound.APP_UI_BUTTON_CLICK);
        }

        private void HandleBackButtonPressed()
        {
            ChangeCanvas(CanvasType, CanvasType.Login);
            SoundManager.PlaySound(SharedGameSound.APP_UI_BUTTON_CLICK);
        }

        private void HandleRegisterButtonClicked()
        {
            SoundManager.PlaySound(SharedGameSound.APP_UI_BUTTON_CLICK);
            ResetErrorMessages();

            if (!ValidateRegisterInputFields())
            {
                return;
            }
            OnPressRegisterButton?.Invoke(new AccountBasic
            {
                Username = _usernameField.text,
                Email = _emailField.text,
                Password = _passwordField.text
            });
            _registerButton.interactable = false;

            ResetRegisterTexts();
        }

        private bool ValidateRegisterInputFields()
        {
            if (string.IsNullOrEmpty(_emailField.text))
            {
                _emailErrorMessageText.text = EmailFieldEmptyMessage;
                return false;
            }
            _emailErrorMessageText.text = string.Empty;

            if (string.IsNullOrEmpty(_usernameField.text))
            {
                _usernameErrorMessageText.text = UsernameFieldEmptyMessage;
                return false;
            }
            _usernameErrorMessageText.text = string.Empty;

            if (_passwordField.text.Length < 6)
            {
                _passwordErrorMessageText.text = WeakPasswordMessage;
                return false;
            }
            _passwordErrorMessageText.text = string.Empty;

            if (_passwordField.text != _passwordConfirmationField.text)
            {
                _passwordConfirmationErrorMessageText.text = PasswordsDoNotMatchMessage;
                return false;
            }
            _passwordConfirmationErrorMessageText.text = string.Empty;

            return true;
        }
    }