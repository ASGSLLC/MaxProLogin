using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace maxprofitness.shared
{
    public class MaxProMobileControlSelection : MonoBehaviour
    {
        public bool isUsingMobileControls;
        public bool isUsingMaxProControls;
        public bool isUsingVRControls;


        public void OnMobileSelected()
        {
            isUsingMobileControls = true;
            isUsingMaxProControls = false;
            isUsingVRControls = false;
        }

        public void OnMaxProSelected()
        {
            isUsingMobileControls = false;
            isUsingMaxProControls = true;
            isUsingVRControls = false;
        }

        public void OnVRSelected()
        {
            isUsingMobileControls = false;
            isUsingMaxProControls = false;
            isUsingVRControls = true;
        }
    }
}