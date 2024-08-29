using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if DOTWEEN
using DG.Tweening;
#endif

namespace maxprofitness.login
{
    public class CanvasGroupUIBase : MonoBehaviour
    {
        protected CanvasGroup canvasGroup;

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual void Show()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
#if DOTWEEN
            canvasGroup.DOKill();
#endif
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
#if DOTWEEN
            canvasGroup.DOFade(1, 0.25f);
#endif
        }

        public virtual void Hide()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
#if DOTWEEN
            canvasGroup.DOKill();
#endif
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
#if DOTWEEN
            canvasGroup.DOFade(0, 0.25f);
#endif
        }

        public virtual void ForceShow()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
#if DOTWEEN
            canvasGroup.DOKill();
#endif
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            canvasGroup.alpha = 1;
        }

        public virtual void ForceHide()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
#if DOTWEEN
            canvasGroup.DOKill();
#endif
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            canvasGroup.alpha = 0;
        }
    }
}


