using TMPro;
using UnityEngine;
using maxprofitness.login;

public class FirmwareSettingsController : MonoBehaviour
{
    #region Variables

    public bool isFirmwareUpToDate;

    [SerializeField] private CanvasGroupUIBase firmwareWarningCanvas;
    [SerializeField] private TextMeshProUGUI checkingText;
    [SerializeField] private string[] checkingTextMessages;

    #endregion


    #region Monobehaviors


    private void Awake()
    {
        CheckIfUpToDateOnStartup();
    }


    #endregion


    #region Methods


    private void CheckIfUpToDateOnStartup()
    {
        if (isFirmwareUpToDate == true)
        {
            return;
        }
        else
        {
            firmwareWarningCanvas.ForceShow();
        }
    }


    private void UpdateCheckingText(int _messageID)
    {
        checkingText.text = checkingTextMessages[(_messageID)];
    }


    public void OnCheckForUpdatesButtonPressed()
    {
        UpdateCheckingText(3);
    }


    #endregion


}
