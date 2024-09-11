/**********************************************
* MenuButton.cs
* 
* Controls the animation state for UI buttons
* 
**********************************************/
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using maxprofitness.login;

public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    #region VARIABLES
    public CanvasGroup canvasGroup;
    public Button button;
    public Image imageIcon;

    public RectTransform rectTransform;
    private Vector2 anchorPosition;
    private Vector2 anchorMin;
    private Vector2 anchorMax;
    private bool isPressed;

    public enum Animation
    {
        ForceHide,
        ForceShow,
        Hide,
        Show,
        Shake,
        Punch
    }
    protected Animation animationClip = Animation.ForceHide;
    #endregion

    #region STARTUP LOGIC
    //-------------------------------------//
    public virtual void Awake()
    //-------------------------------------//
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (imageIcon == null)
            imageIcon = GetComponent<Image>();

        if (button == null)
            button = GetComponent<Button>();

        anchorPosition = rectTransform.anchoredPosition;
        anchorMin = rectTransform.anchorMin;
        anchorMax = rectTransform.anchorMax;

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        if (button != null)
        {
            Navigation navigation = new Navigation { mode = Navigation.Mode.None };
            button.navigation = navigation;
        }

        //ForceHide();

    } //END Awake
    #endregion

    #region ANIMATION LOGIC
    public virtual void ForceHide(float delay = 0f, Action onComplete = null, float overrideAnimationLength = -99f) { SetAnimation(Animation.ForceHide, delay, onComplete, overrideAnimationLength); }
    public virtual void ForceShow(float delay = 0f, Action onComplete = null, float overrideAnimationLength = -99f) { SetAnimation(Animation.ForceShow, delay, onComplete, overrideAnimationLength); }
    public virtual void Hide(float delay = 0f, Action onComplete = null, float overrideAnimationLength = -99f) { SetAnimation(Animation.Hide, delay, onComplete, overrideAnimationLength); }
    public virtual void Show(float delay = 0f, Action onComplete = null, float overrideAnimationLength = -99f) { SetAnimation(Animation.Show, delay, onComplete, overrideAnimationLength); }
    public virtual void Shake(float delay = 0f, Action onComplete = null, float overrideAnimationLength = -99f) { SetAnimation(Animation.Shake, delay, onComplete, overrideAnimationLength); }
    public virtual void Punch(float delay = 0f, Action onComplete = null, float overrideAnimationLength = -99f) { SetAnimation(Animation.Punch, delay, onComplete, overrideAnimationLength); }

    //--------------------------------------------------//
    public virtual void SetAnimation(Animation clip, float delay = 0f, Action onComplete = null, float overrideAnimationLength = -99f)
    //--------------------------------------------------//
    {
        if (gameObject.activeInHierarchy == true)
        {
            this.animationClip = clip;
            StartCoroutine(_SetAnimation(clip, delay, onComplete, overrideAnimationLength));
        }

    } //END SetAnimation

    //--------------------------------------------------//
    protected virtual IEnumerator _SetAnimation(Animation clip, float delay = 0f, Action onComplete = null, float overrideAnimationLength = -99f)
    //--------------------------------------------------//
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();

            if (canvasGroup == null)
            {
                Debug.LogWarning(gameObject.name + " UIButton//_SetAnimation// missing canvas group");
                yield break;
            }
        }

        DOTween.Kill(canvasGroup);
        DOTween.Kill(rectTransform);
        DOTween.Kill(transform);

        float animLength = 0f;

        if (overrideAnimationLength != -99) animLength = overrideAnimationLength;

        yield return new WaitForSeconds(delay);

        if (clip == Animation.ForceHide)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else if (clip == Animation.ForceShow)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else if (clip == Animation.Hide)
        {
            animLength = .5f;
            if (overrideAnimationLength != -99) animLength = overrideAnimationLength;

            canvasGroup.DOFade(0f, animLength);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else if (clip == Animation.Show)
        {
            animLength = .5f;
            if (overrideAnimationLength != -99) animLength = overrideAnimationLength;
            canvasGroup.DOFade(1f, animLength);
            yield return new WaitForSeconds(animLength);
            animLength = 0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else if (clip == Animation.Shake)
        {
            animLength = .3f;

            rectTransform.anchoredPosition = anchorPosition;
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.DOShakeAnchorPos(animLength, new Vector2(50f, 0f), 50, 90f, true, true);
        }
        else if (clip == Animation.Punch)
        {
            animLength = .25f;

            this.transform.localScale = Vector3.one;
            this.transform.DOPunchScale(new Vector3(.5f, .5f, .5f), animLength, 5, .5f);
        }

        yield return new WaitForSeconds(animLength);
        onComplete?.Invoke();

    } //END SetAnimation
    #endregion

    #region BUTTON PRESSED LOGIC
    //---------------------------------//
    public virtual void ButtonPressed()
    //---------------------------------//
    {
        SetAnimation(Animation.Punch);

    } //END ButtonPressed
    #endregion

    #region ENABLE DISABLE BUTTON
    //--------------------------------------------------//
    public void EnableButton(bool enabled)
    //--------------------------------------------------//
    {
        button.enabled = enabled;
        button.image.enabled = enabled;
        button.image.raycastTarget = enabled;

    } // END EnableButton
    #endregion

    #region SET COLORS
    //----------------------------------------//
    public void SetColors(Color main, Color hover)
    //----------------------------------------//
    {
        ColorBlock block = new ColorBlock();
        block.colorMultiplier = 1f;
        block.fadeDuration = .1f;
        block.normalColor = main;
        block.highlightedColor = hover;
        block.pressedColor = Color.Lerp(block.highlightedColor, Color.black, .5f);
        block.selectedColor = block.normalColor;
        block.disabledColor = new Color(block.normalColor.r, block.normalColor.g, block.normalColor.b, .5f);

        button.colors = block;

    } //END SetColors
    #endregion

    #region POINTER EVENTS

    //----------------------------------------//
    public void OnPointerDown(PointerEventData eventData)
    //----------------------------------------//
    {
        isPressed = true;
        StartCoroutine(ResetImageTimer());

    } // END OnPointerDown

    //----------------------------------------//
    public void OnPointerUp(PointerEventData eventData)
    //----------------------------------------//
    {
        isPressed = false;

    } // END OnPointerUp

    //----------------------------------------//
    public void OnPointerExit(PointerEventData eventData)
    //----------------------------------------//
    {
        if (isPressed)
            ResetImage();

    } // END OnPointerExit

    #endregion

    #region RESET IMAGE
    //----------------------------------------//
    public void ResetImage()
    //----------------------------------------//
    {
        Image image = GetComponent<Image>();

        if (image != null)
        {
            image.enabled = false;
            image.enabled = true;

            isPressed = false;
        }

    } // END ResetTheImage 
    #endregion

    #region RESET IMAGE TIMER
    //----------------------------------------//
    IEnumerator ResetImageTimer()
    //----------------------------------------//
    {
        StopCoroutine(ResetImageTimer());

        yield return new WaitForSeconds(3f);

        ResetImage();

    } // END ResetColor 
    #endregion

} //END class