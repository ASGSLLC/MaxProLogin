using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using maxprofitness.login;

public class UITabHelper : MonoBehaviour
{

    #region Variables

    [SerializeField] private bool isTabActive = false;

    [HideInInspector] public int tabID;

    private UITabManager tabManager;
    private RectTransform rt;
    private TextMeshProUGUI text;
    private Image image;


    #endregion


    #region Methods


    public void Init(int _tabID)
    {
        tabManager = GetComponentInParent<UITabManager>();
        rt = GetComponent<RectTransform>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        image = GetComponentInChildren<Image>();

        tabID = _tabID;
    }


    public void OnTabPressed()
    {
        tabManager.ToggleTabs(tabID);
    }


    public void ActivateTab()
    {
        rt.sizeDelta = tabManager.tabSizeActive;
        image.sprite = tabManager.tabSpriteActive;
        text.color = tabManager.tabTextColorActive;
        isTabActive = true;
    }


    public void DeactivateTab()
    {
        rt.sizeDelta = tabManager.tabSizeInactive;
        image.sprite = tabManager.tabSpriteInactive;
        text.color = tabManager.tabTextColorInactive;
        isTabActive = false;
    }


    #endregion


}

