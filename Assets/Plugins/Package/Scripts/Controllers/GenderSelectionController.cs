using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using maxprofitness.login;

public class GenderSelectionController : MonoBehaviour
{
    [SerializeField] private Toggle _maleSelectionToggle;
    [SerializeField] private Toggle _femaleSelectionToggle;
    [SerializeField] private Image _backgroundAnimation;

    [SerializeField] private Vector2 _rightOffsetPosition;
    [SerializeField] private Vector2 _leftOffsetPosition;

    [SerializeField] private float _animationDuration;

    private bool _isFirstSelection = true;

    private void OnEnable()
    {
        _maleSelectionToggle.onValueChanged.AddListener(HandleMaleToggleValueChange);
        _femaleSelectionToggle.onValueChanged.AddListener(HandleFemaleToggleValueChange);
    }

    private void OnDisable()
    {
        _maleSelectionToggle.onValueChanged.AddListener(HandleMaleToggleValueChange);
        _femaleSelectionToggle.onValueChanged.AddListener(HandleFemaleToggleValueChange);
    }

    private void HandleMaleToggleValueChange(bool isToggleOn)
    {
        if (!isToggleOn)
        {
            return;
        }

        RectTransform rectTransform = _backgroundAnimation.GetComponent<RectTransform>();

        if (_isFirstSelection)
        {
            rectTransform.DOLocalMove(_leftOffsetPosition, 0f);
            _backgroundAnimation.DOFade(1, 0.1f);
            _isFirstSelection = false;
        }

        _femaleSelectionToggle.isOn = false;
        rectTransform.DOLocalMove(_leftOffsetPosition, _animationDuration);

        //AppEvents.OnGenderSelectSendEvent?.Invoke(GenderType.Male);
    }

    private void HandleFemaleToggleValueChange(bool isToggleOn)
    {
        if (!isToggleOn)
        {
            return;
        }

        RectTransform rectTransform = _backgroundAnimation.GetComponent<RectTransform>();

        if (_isFirstSelection)
        {
            rectTransform.DOLocalMove(_rightOffsetPosition, 0f);
            _backgroundAnimation.DOFade(1, 0.1f);
            _isFirstSelection = false;
        }

        _maleSelectionToggle.isOn = false;
        rectTransform.DOLocalMove(_rightOffsetPosition, _animationDuration);

        //new GenderSelectSendEvent(GenderType.Female).TryInvokeShared(this);
    }
}
