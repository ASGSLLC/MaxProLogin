//using _Project.App.Scripts.Minigames;
//using MaxProFitness.App.TrainingRoutines;
//using MaxProFitness.Shared.Inputs;
using System;
using UnityEngine;

namespace maxprofitness.login
{
    [CreateAssetMenu(fileName = "StartMinigameChannel", menuName = "Channel/Start Minigame")]
    public class StartMinigameChannel : ScriptableObject
    {
        public Action<Minigame, InputsCalibration?> OnReceiveMinigameSelected;

        public void SendSelectedMinigame(Minigame minigame, InputsCalibration? calibration)
        {
            if (OnReceiveMinigameSelected == null)
            {
                Debug.Log("There is no receiver for the Selected Minigame");

                return;
            }

            OnReceiveMinigameSelected.Invoke(minigame, calibration);
            Debug.Log("Selected minigame sent");
        }
    }
}
