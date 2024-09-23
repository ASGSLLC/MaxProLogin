using System;
using UnityEngine;

namespace maxprofitness.shared
{
    /// <summary>
    /// Used to store the round win data in the match
    /// </summary>
    //[CreateAssetMenu(fileName = "Rhythm/Config/RhythmRoundConfig", menuName = "RhythmRoundWinSettings", order = 0)]
    public class RhythmRoundWinSettings : ScriptableObject
    {
        public RhythmRoundWinConditions[] RhythmRoundWinConditions;
    }

    [Serializable]
    public struct RhythmRoundWinConditions
    {
        public int RoundsWon;
        public string GradeMessage;
        public int BonusScore;
    }
}
