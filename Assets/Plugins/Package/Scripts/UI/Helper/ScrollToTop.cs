using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollToTop : MonoBehaviour
{
    private ScrollRect scroll;

    private void Awake()
    {
        scroll = GetComponent<ScrollRect>();
    }

    public void ResetScroll()
    {
        scroll.velocity = Vector2.zero;
        scroll.verticalNormalizedPosition = 1;
    }
}
