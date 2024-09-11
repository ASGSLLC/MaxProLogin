using UnityEngine;
using UnityEngine.UI;

namespace MaxProFitness.Sdk
{
    [DisallowMultipleComponent]
    [AddComponentMenu("MAXPRO Fitness/Simulator Canvas")]
    public sealed class SimulatorCanvas : MonoBehaviour
    {
        [SerializeField] private SimulatorSlider _leftHandSlider;
        [SerializeField] private SimulatorSlider _leftKnobSlider;
        [SerializeField] private Text _leftKnobValue;
        [SerializeField] private SimulatorSlider _rightHandSlider;
        [SerializeField] private SimulatorSlider _rightKnobSlider;
        [SerializeField] private Text _rightKnobValue;

        public string LeftKnobValue
        {
            get => _leftKnobValue.text;
            set => _leftKnobValue.text = value;
        }

        public string RightKnobValue
        {
            get => _rightKnobValue.text;
            set => _rightKnobValue.text = value;
        }

        public SimulatorSlider LeftHandSlider
        {
            get => _leftHandSlider;
            set => _leftHandSlider = value;
        }

        public SimulatorSlider LeftKnobSlider
        {
            get => _leftKnobSlider;
            set => _leftKnobSlider = value;
        }

        public SimulatorSlider RightHandSlider
        {
            get => _rightHandSlider;
            set => _rightHandSlider = value;
        }

        public SimulatorSlider RightKnobSlider
        {
            get => _rightKnobSlider;
            set => _rightKnobSlider = value;
        }
    }
}
