using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarningMessageHelper : MonoBehaviour
{
    [SerializeField] RectTransform parentRT;
    private RectTransform rt;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] TMP_Text errorText;
    [SerializeField] float textLengthMult;
    [SerializeField] float sizeMod;

    private void Awake()
    {
        GetRefs();
    }

    private void GetRefs()
    {
        canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
        rt = this.GetComponent<RectTransform>();
    }

    public void HideWarningMessage()
    {
        if (canvasGroup == null)
        {
            GetRefs();
        }
        canvasGroup.alpha = 0;
    }

    public IEnumerator UpdateSize()
    {
        if (canvasGroup == null)
        {
            GetRefs();
        }

        // Wait until end of frame to ensure fontSize is correct
        yield return new WaitForEndOfFrame();
        rt.sizeDelta = new Vector2(((errorText.text.Length * errorText.fontSize * textLengthMult) - parentRT.sizeDelta.x) + sizeMod, rt.sizeDelta.y);
        canvasGroup.alpha = 1;
    }

}
