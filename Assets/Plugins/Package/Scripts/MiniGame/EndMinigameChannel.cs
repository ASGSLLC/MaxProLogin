using System;
using UnityEngine;

namespace maxprofitness.login
{
    [CreateAssetMenu(fileName = "EndMinigameChannel", menuName = "Channel/End Minigame")]
    public class EndMinigameChannel : ScriptableObject
    {
        public Action<MatchResult> OnReceiveMinigameSelected;

        
        public void SendMatchResult(MatchResult matchResult)
        {
            if (OnReceiveMinigameSelected == null)
            {
                Debug.Log("There is no receiver for the Selected Minigame");

                return;
            }

            OnReceiveMinigameSelected.Invoke(matchResult);
        }
    }
}
