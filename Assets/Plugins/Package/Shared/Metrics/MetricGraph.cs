using System.Collections.Generic;
using System.Linq;
using System;
using TMPro;
using UnityEngine;
using maxprofitness.login;

namespace maxprofitness.shared
{
    /// <summary>
    /// Used to create the metrics graphs in the project
    /// </summary>
    public sealed class MetricGraph : MonoBehaviour
    {
        public event Action OnClearGraph;
        public event Action OnInitializeGraph;

        [Header("Prefabs")]
        [SerializeField] private GameObject _graphCirclePrefab;
        [SerializeField] private GameObject _dotConnectorPrefab;
        [SerializeField] private GameObject _yLabelPrefab;

        [Header("References")]
        [SerializeField] private RectTransform _graphContainer;

        [Header("Parents")]
        [SerializeField] private RectTransform _graphCircleParent;
        [SerializeField] private RectTransform _dotConnectorParent;
        [SerializeField] private RectTransform _yLabelParent;

        [Header("Text")]
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private RectTransform _peakLine;
        [SerializeField] private TMP_Text _peakText;

        [Header("Parameters")]
        [SerializeField] private float _modifierValueLabelY = 1;
        [SerializeField] private int _graphPointsCount;

        [SerializeField] private int _separatorsCount;
        [SerializeField] private float _lineWidth;
        [SerializeField] private bool _showZeroValues = false;

        private int _amountValuesPerCircle;
        private List<int> _valuesPerGroup;
        private int _circlesNeededForGroup;
        
        private readonly List<GameObject> _instantiatedGraphComponents = new List<GameObject>();
        private readonly List<GameObject> _instantiatedGraphCircles = new List<GameObject>();


        public RectTransform GraphCircleParent => _graphCircleParent;
        public int CirclesNeededForGroup => _circlesNeededForGroup;
        public List<GameObject> InstantiatedGraphCircles => _instantiatedGraphCircles;
        
        public void InitializeGraph(List<int> valueList, string time, List<int> valuesPerGroup = null)
        {
            if (valueList == null || valueList.Count == 0)
            {
                Debug.Log($"[{typeof(MetricGraph)}] - No values found on the metrics graph!! That's a grande problemo.");
                return;
            }

            ClearGraph();
            _valuesPerGroup = valuesPerGroup;

            valueList = CondenseList(valueList);

            _timeText.text = time;
            int peakValue = valueList.Max();

            Vector2 sizeDelta = _graphContainer.sizeDelta;
            float graphHeight = sizeDelta.y;
            float graphWidth = sizeDelta.x;

            float yMaximum = peakValue;
            const float MinimumY = 0;
            const float MarginX = 10;
            float xSize = graphWidth / (valueList.Count - 0.75f);

            yMaximum += (yMaximum - MinimumY) * 0.2f;

            RectTransform lastCircleTransform = null;

            for (int i = 0; i < valueList.Count; i++)
            {
                int value = valueList[i];

                if (!_showZeroValues && value == 0)
                {
                    continue;
                }

                value = Mathf.Clamp(value, 0, int.MaxValue);

                float xPosition = MarginX + i * xSize;
                float yPosition = ((value - MinimumY) / (yMaximum - MinimumY)) * graphHeight;
                RectTransform circleRectTransform = CreateCircle(new Vector2(xPosition, yPosition));

                if (lastCircleTransform != null)
                {
                    CreateDotConnection(lastCircleTransform.anchoredPosition, circleRectTransform.anchoredPosition);
                }

                lastCircleTransform = circleRectTransform;

                if (value == peakValue)
                {
                    SetPeakLine(value, circleRectTransform);
                }
            }

            for (int i = 0; i < _separatorsCount; i++)
            {
                RectTransform labelY = Instantiate(_yLabelPrefab, _yLabelParent, false).GetComponent<RectTransform>();
                _instantiatedGraphComponents.Add(labelY.gameObject);

                float normalizedValue = i * 1f / (_separatorsCount - 1);
                decimal labelValue = (decimal)((MinimumY + (normalizedValue * (yMaximum - MinimumY))) * _modifierValueLabelY);
                string labelText = labelValue.ToString("0.00");

                labelY.anchoredPosition = new Vector2(labelY.anchoredPosition.x, normalizedValue * graphHeight);
                labelY.GetComponentInChildren<TextMeshProUGUI>().SetText(labelText);

                labelY.lossyScale.Set(1, 1, 1);
            }
            
            OnInitializeGraph?.Invoke();
        }

        public void ClearGraph()
        {
            if (_instantiatedGraphComponents.Count != 0)
            {
                foreach (GameObject graphContent in _instantiatedGraphComponents)
                {
                    Destroy(graphContent.gameObject);
                }
                _instantiatedGraphComponents.Clear();
            }

            if (_instantiatedGraphCircles.Count != 0)
            {
                foreach (GameObject graphCircle in _instantiatedGraphCircles)
                {
                    Destroy(graphCircle.gameObject);
                }
                _instantiatedGraphCircles.Clear();
            }

            OnClearGraph?.Invoke();
        }

        private List<int> CondenseList(List<int> valuesList)
        {
            List<int> condensedList = new List<int>();
            List<int> listAverage = new List<int>();

            if (_valuesPerGroup == null)
            {
                CalculateAmountValuesPerCircle(valuesList.Count);
                
                for (int i = 0; i < valuesList.Count; i++)
                {
                    listAverage.Add(valuesList[i]);

                    if (i % _amountValuesPerCircle != 0)
                    {
                        continue;
                    }

                    condensedList.Add((int)listAverage.Average());
                    listAverage.Clear();
                }

                return condensedList;
            }

            _showZeroValues = true;
            int valuesCalculated = 0;
            _circlesNeededForGroup =  _graphPointsCount / _valuesPerGroup.Count;

            int totalValues = 0;
            foreach (int groupOfValue in _valuesPerGroup)
            {
                totalValues += groupOfValue;
            }

            totalValues /= valuesList.Count;

            if (totalValues == 0)
            {
                totalValues = 1;
            }

            foreach (int groupOfValue in _valuesPerGroup)
            {
                int condensedGroupOfValues = Mathf.Clamp(groupOfValue / totalValues, 0,  valuesList.Count - valuesCalculated);
                CalculateAmountValuesPerCircle(condensedGroupOfValues, _circlesNeededForGroup);

                int valuesAdded = 0;
                for (int i = 0; i < condensedGroupOfValues; i++)
                {

                    listAverage.Add(valuesList[valuesCalculated]);
                    valuesCalculated++;
            
                    if (i % _amountValuesPerCircle != 0 || valuesAdded >= _circlesNeededForGroup)
                    {
                        continue;
                    }
                    condensedList.Add((int)listAverage.Average());
                    listAverage.Clear();
                    valuesAdded++;
                }

                for (int i = condensedGroupOfValues; i < _circlesNeededForGroup; i++)
                {
                    condensedList.Add(0);
                }
            }

            return condensedList;
        }

        private void SetPeakLine(int peakValue, RectTransform circleRectTransform)
        {
            _peakLine.anchoredPosition = new Vector2(_peakLine.anchoredPosition.x, circleRectTransform.anchoredPosition.y);
            _peakText.SetText((peakValue * _modifierValueLabelY).ToString("0.00"));
        }

        private RectTransform CreateCircle(Vector2 anchoredPosition)
        {
            RectTransform circle = Instantiate(_graphCirclePrefab, _graphCircleParent, false).GetComponent<RectTransform>();

            _instantiatedGraphCircles.Add(circle.gameObject);

            circle.anchorMin = Vector2.zero;
            circle.anchorMax = Vector2.zero;
            circle.anchoredPosition = anchoredPosition;

            circle.lossyScale.Set(1, 1, 1);

            return circle;
        }

        private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
        {
            RectTransform dotConnection =
                Instantiate(_dotConnectorPrefab, _dotConnectorParent, false).GetComponent<RectTransform>();

            _instantiatedGraphComponents.Add(dotConnection.gameObject);

            Vector2 dir = (dotPositionB - dotPositionA).normalized;
            float distance = Vector2.Distance(dotPositionA, dotPositionB);

            dotConnection.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * 180 / Mathf.PI);
            dotConnection.anchoredPosition = dotPositionA + dir * (distance * 0.5f);

            dotConnection.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, distance);
            dotConnection.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _lineWidth);

            dotConnection.lossyScale.Set(1, 1, 1);
        }

        private void CalculateAmountValuesPerCircle(int valueListCount)
        {
            _amountValuesPerCircle = valueListCount / (_graphPointsCount - 1);
            _amountValuesPerCircle = Mathf.Clamp(_amountValuesPerCircle, 1, int.MaxValue);
        }
        
        private void CalculateAmountValuesPerCircle(int valueListCount, int totalCirclesNeeded)
        {
            _amountValuesPerCircle = valueListCount / totalCirclesNeeded;
            _amountValuesPerCircle = Mathf.Clamp(_amountValuesPerCircle, 1, int.MaxValue);
        }
    }
}
