using UnityEngine;
using UnityEngine.UI;
using maxprofitness.login;

namespace App.Scripts.UI.BodyInfo
{
    public class BodyInfoView : MonoBehaviour
    {
        [SerializeField] private Toggle _remindMeLaterToggle;
        [SerializeField] private Button _goToSettingsButton;
        [SerializeField] private Button _notNowButton;

        private bool _remindLaterToggleState;

        private void OnEnable()
        {
            ListenToEvents();
        }

        private void OnDisable()
        {
            RemoveEventListeners();
        }

        private void ListenToEvents()
        {
            _goToSettingsButton.onClick.AddListener(HandleGoToSettingsButtonClick);
            _notNowButton.onClick.AddListener(HandleNotNowButtonClick);
            _remindMeLaterToggle.onValueChanged.AddListener(HandleRemindToggleValueChange);
        }

        private void RemoveEventListeners()
        {
            _goToSettingsButton.onClick.RemoveListener(HandleGoToSettingsButtonClick);
            _notNowButton.onClick.RemoveListener(HandleNotNowButtonClick);
            _remindMeLaterToggle.onValueChanged.RemoveListener(HandleRemindToggleValueChange);
        }

        private void HandleGoToSettingsButtonClick()
        {
            SoundManager.PlaySound(SharedGameSound.APP_UI_BUTTON_CLICK);

            SaveSelectionStateOnExit();

            //new ChangeCanvasFromMainMenuEvent(CanvasType.Settings).TryInvokeShared(this);
            gameObject.SetActive(false);
        }

        private void HandleNotNowButtonClick()
        {
            SoundManager.PlaySound(SharedGameSound.APP_UI_BUTTON_CLICK);

            SaveSelectionStateOnExit();
            gameObject.SetActive(false);
        }

        private void HandleRemindToggleValueChange(bool isToggleOn)
        {
            SoundManager.PlaySound(SharedGameSound.APP_UI_BUTTON_CLICK);

            _remindLaterToggleState = isToggleOn;
        }

        private void SaveSelectionStateOnExit()
        {
            //new SaveUserIgnoreBodyInfoScreenEvent(_remindLaterToggleState).TryInvokeShared(this);
        }
    }
}
