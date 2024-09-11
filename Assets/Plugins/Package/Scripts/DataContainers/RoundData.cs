using System;
using UnityEngine.Serialization;

namespace maxprofitness.login
{
    /// <summary>
    /// Used to store the round system data for the current match
    /// </summary>
    [Serializable]
    public sealed class RoundData
    {
        public int AttackTurnTime;
        public int RestTurnTime;
        public int RoundPointsPerAttack;
        [FormerlySerializedAs("AttackRepetitionsPerTurn")] public int RepetitionsGoal;
    }
}
