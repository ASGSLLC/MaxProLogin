using TMPro;
using UnityEngine;

namespace maxprofitness.shared
{
    public class RowingSpeedView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _currentSpeedText;

        public void UpdateCurrentSpeed(float speed)
        {
            if (float.IsNaN(speed))
            {
                return;
            }

            _currentSpeedText.text = $"{speed:0.0}";
        }
    }
}
