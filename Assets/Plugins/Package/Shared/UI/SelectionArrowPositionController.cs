#if DOTWEEN
using DG.Tweening;
#endif
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace maxprofitness.shared
{
    /// <summary>
    /// This class controls the position of the selection arrow while choosing between the difficulties
    /// </summary>
    public sealed class SelectionArrowPositionController : MonoBehaviour
    {
        [SerializeField] private List<Vector2> _arrowPositions = new List<Vector2>();
        [SerializeField] private float _movementDuration = 0.5f;
        [SerializeField] private CanvasGroup _scoreWithBackgroundCanvasGroup;
        [SerializeField] private CanvasGroup _scoreWithoutBackgroundCanvasGroup;
        public TextMeshProUGUI _scoreWithBackgroundValueText;
        public TextMeshProUGUI _scoreWithoutBackgroundValueText;

        private const float ScoreFadeDuration = 0.10f;

        public void MoveArrowToPosition(int positionIndex)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
#if DOTWEEN
            rectTransform.DOAnchorPos(_arrowPositions[positionIndex], _movementDuration);
#endif
        }

        public void ActivateLockedScoreView(string scoreText)
        {
#if DOTWEEN
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_scoreWithoutBackgroundCanvasGroup.DOFade(0f, ScoreFadeDuration)).SetEase(Ease.Linear);
            _scoreWithBackgroundValueText.SetText(scoreText);
            sequence.Append(_scoreWithBackgroundCanvasGroup.DOFade(1f, ScoreFadeDuration)).SetEase(Ease.Linear);
#endif
        }

        public void DeactivateLockedScoreView(string scoreText)
        {
#if DOTWEEN
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_scoreWithBackgroundCanvasGroup.DOFade(0f, ScoreFadeDuration)).SetEase(Ease.Linear);
            _scoreWithoutBackgroundValueText.SetText(scoreText);
            sequence.Append(_scoreWithoutBackgroundCanvasGroup.DOFade(1f, ScoreFadeDuration)).SetEase(Ease.Linear);
#endif
        }
    }
}
