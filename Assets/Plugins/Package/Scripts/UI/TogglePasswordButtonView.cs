//using MaxProFitness.SharedSound;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using maxprofitness.login;
using maxprofitness.shared;

namespace _Project.App.Scripts.UI.Buttons
{
    public sealed class TogglePasswordButtonView : MonoBehaviour
    {
        [SerializeField] private Toggle _passwordViewButton;
        [SerializeField] private TMP_InputField _passwordInputField;

        [SerializeField] private bool _disableSound = true;

        private void Start()
        {
            TogglePasswordView(true);
        }

        private void OnEnable()
        {
            TogglePasswordView(true);

            _passwordViewButton.onValueChanged.AddListener(HandleTogglePressed);
        }

        private void OnDisable()
        {
            _passwordViewButton.onValueChanged.RemoveListener(HandleTogglePressed);
        }

        private void HandleTogglePressed(bool state)
        {
            if (!_disableSound)
            {
                SoundManager.PlaySound(SharedGameSound.APP_UI_BUTTON_CLICK);
            }

            TogglePasswordView(state);
        }

        private void TogglePasswordView(bool state)
        {
            _passwordInputField.contentType = !state ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;

            _passwordInputField.ForceLabelUpdate();
        }
    }
}
