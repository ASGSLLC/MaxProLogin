using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GenericDropDown : MonoBehaviour
{
    private TMP_Dropdown dropdown;

    [TextArea(1,10)]
    [SerializeField] private string valueOptions;

    private void Awake()
    {
        dropdown = GetComponentInChildren<TMP_Dropdown>();

        valueOptions = valueOptions.Replace(", ", ",");

        string[] _values = valueOptions.Split(',');

        dropdown.AddOptions(_values.ToList());

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
