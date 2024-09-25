using maxprofitness.login;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace maxprofitness.shared
{
   /// <summary>
   /// This class is used to display the rowing metrics when the race finishes
   /// </summary>
    public sealed class RowingMetricsView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _maxSpeed;
        [SerializeField] private TextMeshProUGUI _averageSpeed;
        [SerializeField] private TextMeshProUGUI _strokes;
        [FormerlySerializedAs("_paceMinutes")] [SerializeField] private TextMeshProUGUI _pace;
        [SerializeField] private TextMeshProUGUI _cadence;

        public void Initialize(RowingMetrics rowingMetrics)
        {
            _maxSpeed.SetText($"{rowingMetrics.MaxSpeed:0.0}");
            _averageSpeed.SetText($"{rowingMetrics.AverageSpeed:0.0}");
            _strokes.SetText($"{rowingMetrics.Strokes}");
            _pace.SetText($"{rowingMetrics.PaceMinutes:00}:{rowingMetrics.PaceSeconds:00}");
            _cadence.SetText($"{(int)rowingMetrics.Cadence}");
        }

        public void SetState(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}
