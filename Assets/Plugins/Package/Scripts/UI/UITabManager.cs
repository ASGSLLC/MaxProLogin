using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITabManager : MonoBehaviour
{

    #region Variables

    [SerializeField] private int startingTab;
    [SerializeField] private UITabHelper[] tabs;


    [Header("Tab Active Specs")]
    public Vector2 tabSizeActive;
    public Sprite tabSpriteActive;
    public Color tabTextColorActive;

    [Header("Tab Inactive Specs")]
    public Vector2 tabSizeInactive;
    public Sprite tabSpriteInactive;
    public Color tabTextColorInactive;


    private SettingsPage settingsPage;


    #endregion


    #region Monobehaviors


    private void Awake()
    {
        Init();
    }


    #endregion


    #region Methods


    public void Init()
    {
        settingsPage = GetComponentInParent<SettingsPage>();

        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].Init(i);
        }

        ToggleTabs(startingTab);
    }

    public void ToggleTabs(int _tabToActivate)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (i == _tabToActivate)
            {
                tabs[i].ActivateTab();
                if (settingsPage != null)
                {
                    settingsPage.ShowPanel(i);
                }
            }

            else if (i != _tabToActivate)
            {
                tabs[i].DeactivateTab();
                if (settingsPage != null)
                {
                    settingsPage.HidePanel(i);
                }
            }
        }
    }



    #endregion


}

