using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenderDropDown : DropDownBase
{
    private string genders = "Male, Female";

    protected override void Awake()
    {
        base.Awake();

        this.genders = this.genders.Replace(", ", ",");

        string[] gends = this.genders.Split(',');

        dropdown.AddOptions(gends.ToList());
    }

    public override void OnClicked()
    {
        base.OnClicked();
    }

    protected override void OnValueChanged(int value)
    {
        base.OnValueChanged(value);

    }

}
