//using _Project.App.Scripts.DifficultyUnlocking;
//using _Project.App.Scripts.Minigames;
//using Coimbra;
using MaxProFitness;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace maxprofitness.login
{
    public class MinigameUnlockingSettings : ScriptableObject
    {
        [FormerlySerializedAs("DifficultyUnlockingData")] public List<MinigameUnlockingData> UnlockingData;

        public List<MinigameUnlockingData> GetUnlockingData(Minigame minigame)
        {
            return UnlockingData.Where(data =>
            {
                return data.Minigame.Equals(minigame);
            }).ToList();
        }

        public bool TryGetNext(Minigame current, out MinigameUnlockingData next)
        {
            int lastUnlockedIndex = 0;
            foreach (MinigameUnlockingData data in UnlockingData.Where(data => data.Minigame.Equals(current)))
            {
                lastUnlockedIndex = UnlockingData.IndexOf(data);
            }

            // ++ to unlock the next difficulty in the difficulty list
            lastUnlockedIndex++;
            lastUnlockedIndex = Mathf.Clamp(lastUnlockedIndex, 0, UnlockingData.Count);

            next = UnlockingData[lastUnlockedIndex];
            MinigameDifficulty nextDifficulty = next.Minigame.Difficulty;

            bool hasNextDifficulty = nextDifficulty != current.Difficulty;

            return hasNextDifficulty;
        }
    }

    [Serializable]
    public struct MinigameUnlockingData
    {
        public Minigame Minigame;
        [FormerlySerializedAs("RequiredHighscorePointsToUnlock")] public int RequiredPointsToUnlock;
    }
}
