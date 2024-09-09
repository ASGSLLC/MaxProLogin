using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using maxprofitness.login;

public class DropDownBase : MonoBehaviour
{
    protected TMP_Dropdown dropdown;

    protected int selectedValue = 0;

    protected virtual void Awake()
    {
        dropdown = GetComponentInChildren<TMP_Dropdown>();

        dropdown.onValueChanged.AddListener(OnValueChanged);
    }

    public virtual void OnClicked()
    {

    }

    protected virtual void OnValueChanged(int value)
    {
        selectedValue = value;
    }

    protected virtual void OnDestroy()
    {
        dropdown.onValueChanged.RemoveListener(OnValueChanged);
    }
}
