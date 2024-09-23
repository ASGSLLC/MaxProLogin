#if DOTWEEN
using DG.Tweening;
#endif
using UnityEngine;

namespace maxprofitness.shared
{
    /// <summary>
    /// This class controls the size of the difficulty buttons while doing the animation
    /// </summary>
    public sealed class ButtonAnchorController : MonoBehaviour
    {
        [SerializeField] private Vector2 _defaultPositionAnchor;
        [SerializeField] private Vector2 _pressedPositionAnchor;
        [SerializeField] private float _movementDuration = 0.5f;

        public void ExpandButtonMinSize()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
#if DOTWEEN
            rectTransform.DOAnchorMin(_pressedPositionAnchor, _movementDuration);
#endif
        }

        public void ContractButtonMinSize()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
#if DOTWEEN
            rectTransform.DOAnchorMin(_defaultPositionAnchor, _movementDuration);
#endif
        }
    }
}
