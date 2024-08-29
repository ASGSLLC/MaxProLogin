using UnityEngine;
using UnityEngine.UI;
using maxprofitness.login;

public class MeasurementSystemToggle : MonoBehaviour
{
    #region VARIABLES


    public bool isMetric;

    [SerializeField] private Image imperialSelectionImage;
    [SerializeField] private Image metricSelectionImage;

    [SerializeField] private CanvasGroupUIBase imperialStatsCanvas;
    [SerializeField] private CanvasGroupUIBase metricStatsCanvas;
    
    
    #endregion


    #region MONOBEHAVIOURS


    //------------------//
    private void Awake()
    //------------------//
    {
        Init();

    } // END Awake
    
    
    #endregion


    #region INIT


    //-----------------//
    private void Init()
    //-----------------//
    {
        if (PlayerPrefs.HasKey("measurement") == true)
        {
            int _measurementSystem = PlayerPrefs.GetInt("measurement");

            if (_measurementSystem == 0)
            {
                isMetric = false;
            }
            else
            {
                isMetric = true;
            }
        }
        else
        {
            PlayerPrefs.SetInt("measurement", 0);
        }

        if (isMetric)
        {
            OnMetricSelected();
        }
        else if (!isMetric)
        {
            OnImperialSelected();
        }

    } // END Init


    #endregion


    #region TOGGLE MEASUREMENTS


    //------------------------------//
    public void OnImperialSelected()
    //-----------------------------//
    {
        imperialSelectionImage.enabled = true;
        metricSelectionImage.enabled = false;

        isMetric = false;
        PlayerPrefs.SetInt("measurement", 0);

        imperialStatsCanvas.ForceShow();
        metricStatsCanvas.ForceHide();

    } // END OnImperialSelected


    //----------------------------//
    public void OnMetricSelected()
    //----------------------------//
    {
        imperialSelectionImage.enabled = false;
        metricSelectionImage.enabled = true;

        isMetric = true;
        PlayerPrefs.SetInt("measurement", 1);

        imperialStatsCanvas.ForceHide();
        metricStatsCanvas.ForceShow();

    } // END OnMetricsSelected


    #endregion


} // END Class
