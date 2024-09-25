using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace maxprofitness.shared
{
    public class RowingCanoeMatchMetricsScreenController : MonoBehaviour
    {
        #region VARIABLES


        [Header("Graph")]
        [SerializeField] private MetricGraph _workGraph;
        [SerializeField] private MetricGraph _powerGraph;
        [SerializeField] private RowingMetricsView _rowingMetrics;

        [Header("Results Metrics")]
        [SerializeField] private TextMeshProUGUI _gameMode;
        [SerializeField] private TextMeshProUGUI _gameDifficulty;
        [SerializeField] private TextMeshProUGUI _calories;
        [SerializeField] private TextMeshProUGUI _score;
        [SerializeField] private TextMeshProUGUI _scorePostGame;

        [Header("Rowing Metrics")] 
        [SerializeField] private TMP_Text _paceText;
        [SerializeField] private TMP_Text _strokesText;
        [SerializeField] private TMP_Text _cadenceText;
        [SerializeField] private TMP_Text _avgSpeedText;
        [SerializeField] private TMP_Text _maxSpeedText;

        [Header("Normal Button Data")]
        [SerializeField] private Color _normalButtonTextColor;
        [SerializeField] private Sprite _normalButtonSprite;
        [SerializeField] private Vector2 _normalButtonSize;

        [Header("Selected Button Data")]
        [SerializeField] private Color _selectedButtonTextColor;
        [SerializeField] private Sprite _selectedButtonSprite;
        [SerializeField] private Vector2 _selectedButtonSize;

        [Header("Tab System")]
        [SerializeField] private Button[] _tabButtons;
        [SerializeField] private TMP_Text[] _tabButtonsText;
        [SerializeField] private GameObject[] _tabPanels;

        [SerializeField] private DifficultySelectionView difficultySelection;

        public int endScore;

        #endregion


        #region MONOBHEAVIOURS


        //------------------//
        private void Start()
        //------------------//
        {
            Setup();

        } // END Start


        #endregion


        #region SETUP


        //-----------------//
        public void Setup()
        //-----------------//
        {
            difficultySelection = FindObjectOfType<DifficultySelectionView>();
            _normalButtonSize = _tabButtons[1].GetComponent<RectTransform>().sizeDelta;
            _selectedButtonSize = _tabButtons[0].GetComponent<RectTransform>().sizeDelta;

            for (int i = 0; i < _tabButtons.Length; i++)
            {
                int index = i;
                _tabButtons[i].onClick.AddListener(() => HandleTabButtonPressed(index));
            }

            HandleTabButtonPressed(0);

        } // END Setup


        #endregion


        #region HANDLE TAB BUTTON PRESSED


        //--------------------------------------------//
        private void HandleTabButtonPressed(int index)
        //--------------------------------------------//
        {
            for (int i = 0; i < _tabButtons.Length; i++)
            {
                Button button = _tabButtons[i];
                RectTransform rect = button.GetComponent<RectTransform>();

                if (button == _tabButtons[index])
                {
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _selectedButtonSize.y);

                    button.image.sprite = _selectedButtonSprite;
                    _tabButtonsText[index].color = _selectedButtonTextColor;

                    _tabPanels[index].SetActive(true);
                }
                else
                {
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _normalButtonSize.y);

                    button.image.sprite = _normalButtonSprite;
                    _tabButtonsText[i].color = _normalButtonTextColor;
                    _tabPanels[i].SetActive(false);
                }
            }
        } // END HandleTabButtonPressed


        #endregion


        #region SET MATCH METRICS SCREEN


        //------------------------------------------------------------//
        public void SetMatchMetricsScreen(MatchResult minigameResult)
        //------------------------------------------------------------//
        {
            string time = $"{minigameResult.TimeToFinish.Minutes:00}:{minigameResult.TimeToFinish.Seconds:00}";

            RowingMetrics rowingMetrics = minigameResult.RowingMetrics;
            Minigame minigame = minigameResult.Minigame;

            _workGraph.InitializeGraph(minigameResult.WorkByRepetitions, time);
            _powerGraph.InitializeGraph(minigameResult.PowerList, time);
            _rowingMetrics.Initialize(rowingMetrics);

            _avgSpeedText.SetText($"{rowingMetrics.AverageSpeed:0.0}");
            _maxSpeedText.SetText($"{rowingMetrics.MaxSpeed:0.0}");
            _cadenceText.SetText($"{rowingMetrics.Cadence}");

            //_gameMode.SetText(minigame.Type.GetName());
            _gameMode.SetText(minigame.Type.ToString());
            _gameDifficulty.SetText($"{"Racing"} - {minigame.Difficulty}");
            _score.SetText($"{minigameResult.Score}");
            _scorePostGame.SetText($"{minigameResult.Score}");
            endScore = minigameResult.Score;
            _calories.SetText($"{minigameResult.CaloriesBurned}");

        } // END SetMatchMetricsScreen


        #endregion


    } // END RowingCanoeMatchScreejController.cs

} // END Namespace
