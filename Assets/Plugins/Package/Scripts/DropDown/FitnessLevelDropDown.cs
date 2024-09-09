using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using maxprofitness.login;

public class FitnessLevelDropDown : DropDownBase
{    
    private string fitnessLevels = "Starter, Casual (1 - 2 Days), Serious (3 - 5 Days), Elite (6 - 7 Days)";

    //private Input

    protected override void Awake()
    {
        base.Awake();

        this.fitnessLevels = this.fitnessLevels.Replace(", ", ",");

        string[] fitnessLevels = this.fitnessLevels.Split(',');

        dropdown.AddOptions(fitnessLevels.ToList());

        dropdown.value = 0;
    }

    public override void OnClicked()
    {
        base. OnClicked();
    }

    protected override void OnValueChanged(int value)
    {
        base.OnValueChanged(value);

    }


}
