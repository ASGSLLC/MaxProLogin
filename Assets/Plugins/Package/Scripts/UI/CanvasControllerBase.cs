using DG.Tweening;
using UnityEngine;

namespace maxprofitness.login
{
    public class CanvasControllerBase : MonoBehaviour
    {
        public delegate void ChangeCanvasHandler(CanvasType currentCanvas, CanvasType nextCanvas);

        public event ChangeCanvasHandler OnChangeCanvas;

        [SerializeField] protected CanvasType _canvasType;

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Transform _scaleHolder;

        private const float FadeDuration = .15f;
        private const float FadeScaleIn = 1.9f;
        private const Ease FadeEase = Ease.OutExpo;


        public CanvasType CanvasType => _canvasType;

        public virtual void Initialize() {}

        public virtual void OnEnableCanvas(){}

        protected virtual void OnShow() {}

        protected virtual void OnHide() {}

        public void Show()
        {
            _canvasGroup.alpha = 0;
            _scaleHolder.localScale = new Vector3(FadeScaleIn, FadeScaleIn, FadeScaleIn);
            //gameObject.SetActive(true);
#if DOTWEEN
            _canvasGroup.DOFade(1, FadeDuration).SetEase(FadeEase);
            _scaleHolder.DOScale(1, FadeDuration).SetEase(FadeEase).OnComplete(OnShow);
#endif
        }

        public void Hide()
        {
            OnHide();
            //gameObject.SetActive(false);
        }

        protected void ChangeCanvas(CanvasType currentCanvas, CanvasType nextCanvas)
        {
            OnChangeCanvas?.Invoke(currentCanvas, nextCanvas);
        }
    }
}
