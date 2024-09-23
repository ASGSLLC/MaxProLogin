#if FIREBASE
using Firebase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using maxprofitness.shared;

public class FirebaseDependencyHelper : MonoBehaviour
{
    public static void CheckAndFixDependenciesAsync(Action OnSuccess = null, Action<string> OnError = null)
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith((task) => 
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                string error = FirebaseExeptionHelper.HandleErrorExeption(task.Exception);

                Debug.LogError("FirebaseDependencyHelper//CheckAndFixDependenciesAsync// " + error);
                OnError?.Invoke(error);
            }
            else
            {
                Debug.Log("FirebaseDependencyHelper//CheckAndFixDependenciesAsync// success ");

                OnSuccess?.Invoke();
            }
        });
    }
}
#endif