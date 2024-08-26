using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System;
using UnityEngine.InputSystem;

public static class UserEnteredData  
{
    //public static string birthday;
    public static int challenge;            // acctually goal
    public static string country;
    public static string displayName;

    public static string firstName;
    public static string lastName;

    public static string email;

    public static int fitnessLevel;
    public static int gender;
    
    public static int height;
    public static int heightFeet;
    public static int heightInch;

    public static bool imperialMeasurement;
    
    public static string nickname;
    public static int weight;
    public static int weightLbs;

    public static int day;
    public static int month;
    public static int year;

    public static void LoadCachedData()
    {
        displayName = PlayerPrefs.GetString("displayName");

        day = PlayerPrefs.GetInt("day");
        month = PlayerPrefs.GetInt("month");
        year = PlayerPrefs.GetInt("year");

        challenge = PlayerPrefs.GetInt("challenge");
        country = PlayerPrefs.GetString("country");
        email = PlayerPrefs.GetString("email");

        fitnessLevel = PlayerPrefs.GetInt("fitnessLevel");
        gender = PlayerPrefs.GetInt("gender");
        height = PlayerPrefs.GetInt("height");
        heightFeet = PlayerPrefs.GetInt("heightFeet");
        heightInch = PlayerPrefs.GetInt("heightInch");

        bool.TryParse(PlayerPrefs.GetString("imperialMeasurement"), out imperialMeasurement);

        nickname = PlayerPrefs.GetString("nickname");
        weight = PlayerPrefs.GetInt("weight");
        weightLbs = PlayerPrefs.GetInt("weightLbs");

    }

    public static void SaveDisplayName(string value)
    {
        displayName = value;
        PlayerPrefs.SetString("displayName", displayName);
    }

    /// <summary>
    /// Ex: 1967-02-09T01:56:21.844Z
    /// </summary>
    public static string GetBirthday() 
    {
        DateTime date = DateTime.Now; // store 2011-06-27 12:00:00

        if (DateTime.TryParse(year + "-" + month + "-" + day, out date))
        {
            Debug.Log("UserEnteredData//GetBirthday// bday parsed " + date.ToString("yyyy/MM/dd T hh:mm:ss z"));
        }
        else
        {
            Debug.Log("UserEnteredData//GetBirthday// bday not parsed");
        }

        return date.ToString("yyyy/MM/dd T hh:mm:ss z");

    }

    public static void SaveDay(int value)
    {
        day = value;
        PlayerPrefs.SetInt("day", day);
    }

    public static void SaveMonth(int value)
    {
        month = value;
        PlayerPrefs.SetInt("month", month);
    }

    public static void SaveYear(int value)
    {
        year = value;
        PlayerPrefs.SetInt("year", year);
    }

    public static void SaveGoal(int value)
    {
        challenge = value;
        PlayerPrefs.SetInt("challenge", challenge);
    }

    public static void SaveEmail(string value)
    {
        email = value;
        PlayerPrefs.SetString("email", email);
    }

    public static void SaveNickname(string value)
    {
        nickname = value;
        PlayerPrefs.SetString("nickname", nickname);
    }

    public static void SaveFitnessLevel(int value)
    {
        fitnessLevel = value;
        PlayerPrefs.SetInt("fitnessLevel", fitnessLevel);

    } 
    
    public static void SaveIsImperial(bool value)
    {
        imperialMeasurement = value;
        PlayerPrefs.SetString("imperialMeasurement", imperialMeasurement.ToString());
    }

    public static void SaveWeight(int value)
    {
        weight = value;
        PlayerPrefs.SetInt("weight", weight);
    }

    public static void SaveWeightLbs(int value)
    {
        weightLbs = value;
        PlayerPrefs.SetInt("weightLbs", weightLbs);
    }

    public static void SaveHeight(int value)
    {
        height = value;
        PlayerPrefs.SetInt("height", height);
    }

    public static void SaveHeightFeet(int value)
    {
        heightFeet = value;
        PlayerPrefs.SetInt("heightFeet", heightFeet);
    }

    public static void SaveHeightInch(int value)
    {
        heightInch = value;
        PlayerPrefs.SetInt("heightInch", heightInch);
    }

    public static void SaveGender(int value)
    {
        gender = value;
        PlayerPrefs.SetInt("gender", gender);
    }

    public static void SaveCountry(string value)
    {
        country = value;
        PlayerPrefs.SetString("country", country);
    }
}
