using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using _Project.RowingCanoe.Scripts;
using DG.Tweening;

public class YScaleUIBase : MonoBehaviour
{
    public virtual Tween Show() 
    {
        DOTween.Kill(transform);

        Tween tween = transform.DOScaleY(1, 0f);
#if AIR_RUNNER
        if (AirRunnerMainMenu.canvasGroup != null) 
        {
            AirRunnerMainMenu.canvasGroup.alpha = 1;
        }
        
        tween.onComplete += () =>
        {
            AirRunnerMainMenu.EnableCollider();
        };

#endif
        return tween;
    }

    public virtual Tween Hide()
    {
        //Debug.Log("MaxProConnectionCanvas//Hide//");

        DOTween.Kill(transform);

        Tween tween = transform.DOScaleY(0, 0f);
#if AIR_RUNNER
        tween.onComplete += () =>
        {
            AirRunnerMainMenu.DisableCollider();

            if(AirRunnerMainMenu.canvasGroup != null)
                AirRunnerMainMenu.canvasGroup.alpha = 1;
        };
#endif
        return tween;
    }



}
