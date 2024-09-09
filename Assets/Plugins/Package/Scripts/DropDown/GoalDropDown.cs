using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using maxprofitness.login;

public class GoalDropDown : MonoBehaviour
{
    private TMP_Dropdown dropdown;

    private string goalOptions = "Burn, Tone, Build";

    private void Awake()
    {
        dropdown = GetComponentInChildren<TMP_Dropdown>();

        goalOptions = goalOptions.Replace(", ", ",");

        string[] activityLevels = goalOptions.Split(',');

        dropdown.AddOptions(activityLevels.ToList());

        dropdown.value = 0;

        dropdown.onValueChanged.AddListener(OnValueChanged);
    }

    public void OnClicked()
    {

    }

    private void OnValueChanged(int arg0)
    {

    }



}
