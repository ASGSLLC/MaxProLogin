using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MaxProFitness.Sdk
{
    [DisallowMultipleComponent]
    [AddComponentMenu("MAXPRO Fitness/Simulator Slider")]
    [RequireComponent(typeof(Slider))]
    public sealed class SimulatorSlider : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        public event Action<SimulatorSlider> OnPressed;

        public event Action<SimulatorSlider> OnReleased;

        [SerializeField] [HideInInspector]
        private Slider _slider;

        public Slider Slider => _slider;

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnPressed?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnReleased?.Invoke(this);
        }

        private void OnValidate()
        {
            _slider = GetComponent<Slider>();
        }
    }
}
