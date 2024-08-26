using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SystemOfMeasurementHelper;

public class SystemOfMeasurementHelper : MonoBehaviour
{
    public class FeetInchContainer 
    {
        public int feet;
        public int inches;
    }

    public static double ConvertKilogramsToPounds(double kgs) 
    {
        return kgs * 2.20462262185d;
    }

    public static double ConvertPoundsToKilograms(double lbs)
    {
        return lbs * 0.45359237d;
    }

    public static FeetInchContainer ConvertCentimetersToFeet(double cm)
    {
        double centimeters = 187.96;
        double inches = centimeters / 2.54;
        double feet = Math.Floor(inches / 12);
        inches -= (feet * 12);

        FeetInchContainer feetInchContainer = new FeetInchContainer();
        feetInchContainer.feet = (int)feet;
        feetInchContainer.inches = (int)inches;

        return feetInchContainer;
    }

    public static double ConvertFeetToCentimeters(double ft)
    {
        return ft * 30.48d;
    }
}
