using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using maxprofitness.login;

public class RepeatingBackground : MonoBehaviour
{
    [SerializeField] private RectTransform backgroundRT;
    [SerializeField] private float scrollDuration;
    private Vector2 scrollTarget;
    private Vector2 scrollStart;
    private Vector2 currentPos;
    private float lerpProgress;


    private void Update()
    {
        backgroundRT.anchoredPosition = currentPos;
    }

    private void Awake()
    {
        Image _backgroundImage = backgroundRT.GetComponent<Image>();
        backgroundRT.localScale = new Vector3(_backgroundImage.sprite.texture.width / Screen.width, _backgroundImage.sprite.texture.height / Screen.height, 1);

        scrollStart = new Vector2(Screen.width, Screen.height);
        scrollTarget = new Vector2(-Screen.width, -Screen.height);
        currentPos = scrollStart;
        StartCoroutine(ScrollBackground());
    }

    IEnumerator ScrollBackground()
    {
        if (lerpProgress > 0)
        {
            lerpProgress--;
        }

        while (lerpProgress < 1)
        {
            currentPos = Vector2.Lerp(scrollStart, scrollTarget, lerpProgress);
            lerpProgress += Time.deltaTime / scrollDuration;
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(ScrollBackground());
    }

}
