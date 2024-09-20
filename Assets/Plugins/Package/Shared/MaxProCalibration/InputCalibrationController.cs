//using MaxProFitness.Shared.Inputs;
//using MaxProFitness.Shared.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

namespace maxprofitness.shared
{
    public class InputCalibrationController : MonoBehaviour
    {
        private InputsCalibration _calibration = new InputsCalibration{Left = new InputCalibration{MaximumDistance = 0,MediumDistance = 0,MinimumDistance = 0}, Right = new InputCalibration{MaximumDistance = 0,MediumDistance = 0,MinimumDistance = 0}, Both = new InputCalibration{MaximumDistance = 0,MediumDistance = 0,MinimumDistance = 0}};

        public void SetNewCalibration(CombatInput inputToCalibrate,List<int> newCalibrationValues)
        {
            Debug.Log("## Setting new calibration Input Received: " + inputToCalibrate);
            
            int calibratedValuesMedia = 0;

            foreach (int calibrationValue in newCalibrationValues)
            {
                calibratedValuesMedia += calibrationValue;
            }

            if (calibratedValuesMedia < 1)
            {
                Debug.Log("There was not any calibration value to save this: " + inputToCalibrate.ActionType + "::" + inputToCalibrate.InputSide);
                return;
            }
            calibratedValuesMedia = calibratedValuesMedia / newCalibrationValues.Count;
            Debug.Log("#3 calibration value saved: " + calibratedValuesMedia);

            if (inputToCalibrate.InputSide == ActionSide.BOTH)
            {
                if (inputToCalibrate.ActionType == ActionType.HIT)
                {
                    _calibration.Left.MaximumDistance = calibratedValuesMedia;
                    _calibration.Right.MaximumDistance = calibratedValuesMedia;
                    return;
                }
            
                if (inputToCalibrate.ActionType == ActionType.DEFENSE)
                {
                    _calibration.Left.MediumDistance = calibratedValuesMedia;
                    _calibration.Right.MediumDistance = calibratedValuesMedia;
                    return;
                }
            
                _calibration.Left.MinimumDistance = calibratedValuesMedia;
                _calibration.Right.MinimumDistance = calibratedValuesMedia;
            }
            
            if (inputToCalibrate.InputSide == ActionSide.LEFT)
            {
                if (inputToCalibrate.ActionType == ActionType.HIT)
                {
                    _calibration.Left.MaximumDistance = calibratedValuesMedia;
                    return;
                }
                if (inputToCalibrate.ActionType == ActionType.DEFENSE)
                {
                    _calibration.Left.MediumDistance = calibratedValuesMedia;
                    return;
                }
            
                _calibration.Left.MinimumDistance = calibratedValuesMedia;
                return;
            }
            
            if (inputToCalibrate.ActionType == ActionType.HIT)
            {
                _calibration.Right.MaximumDistance = calibratedValuesMedia;
                return;
            }
            if (inputToCalibrate.ActionType == ActionType.DEFENSE)
            {
                _calibration.Right.MediumDistance = calibratedValuesMedia;
                return;
            }
            
            _calibration.Right.MinimumDistance = calibratedValuesMedia;
        }

        public void SetRestDistances(int leftRestDistance, int rightRestDistance)
        {
            _calibration.Left.MinimumDistance = leftRestDistance;
            _calibration.Right.MinimumDistance = rightRestDistance;
        }

        public InputsCalibration CalibratedInputs
        {
            get => _calibration;
            set => _calibration = value;
        }
    }
}
