using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class UserDataMeta
{
    public string birthday;
    public Goal challenge;
    public string country;
    public Timestamp creationTime;     // Ex: June 4, 2023 at 6:35:05PM UTC-4

    public string displayName;
    public string email;
    public string firstName;

    public bool firstTimeInApp;

    public FitnessLevel fitnessLevel;

    public Gender gender;

    public int height;

    public int heightFeets;
    public int heightInches;

    public bool imperialMeasurement;

    public string instructor;

    public bool isGrandFathered;

    public string lastLogin;   // "06/04/23"

    public string lastName;
    public string nickname;

    public string photo;
    public string photoURL;
    public string photoUrl;

    public string uid;
    public int weight;
    public int weightLbs;

    public int rowingHighScore;
}
