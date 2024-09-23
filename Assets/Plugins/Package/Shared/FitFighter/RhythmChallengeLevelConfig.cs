//using _Project.App.Scripts.Minigames;
using System.Collections.Generic;
using MaxProFitness;
using UnityEngine;
using UnityEngine.Serialization;
using maxprofitness.login;

namespace maxprofitness.shared
{
    /// <summary>
    /// Used by the new fit fighter mode to configure the round's challenge level
    /// </summary>
    //[CreateAssetMenu(menuName = "Rhythm/Config/RhythmRoundConfig", fileName = "RhythmChallengeLevelConfig")]
    public class RhythmChallengeLevelConfig : ScriptableObject
    {
        [FormerlySerializedAs("relatedMinigameDifficulty")]
        [FormerlySerializedAs("RelatedDifficultyType")]
        [Header("Difficulty")]
        [Tooltip("Difficulty related to this challenge level")]
        public MinigameDifficulty RelatedMinigameDifficulty;

        [Header("Challenge name")]
        [Tooltip("Challenge level's name")]
        public string ChallengeLevelName;

        [Header("Round configs")]
        public RepetitionTypeData RepetitionTypeData;
        public RhythmRoundWinSettings RhythmRoundWinSettings;

        [Tooltip("Delay to start the game")]
        public int DelayToStartGameTime;
        [Tooltip("Delays to initiate the new turn")]
        public int DelayToChangeTurnTime;
        [Tooltip("Delays to initiate the new round")]
        public int DelayToChangeRoundTime;

        [Tooltip("Number of rounds per match")]
        public List<RoundData> RoundData;

        [Header("MaxPRO configs")]
        [Range(1, 25)]
        [Tooltip("Minimum recommended weight on the MaxPRO device") ]
        public int MinRecommendedMaxProWeight;
        [Tooltip("Maximum recommended weight on the MaxPRO device")]
        [Range(1, 25)]
        public int MaxRecommendedMaxProWeight;

        public int TotalRounds => RoundData.Count;

        public int GetTotalRoundTimeSeconds()
        {
            int duration = 0;
            foreach (RoundData roundData in RoundData)
            {
                duration += roundData.AttackTurnTime;
                duration += roundData.RestTurnTime;
            }

            duration += DelayToChangeRoundTime;
            duration += DelayToChangeTurnTime;

            return duration;
        }
    }
}
