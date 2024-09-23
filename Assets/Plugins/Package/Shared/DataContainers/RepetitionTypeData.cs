using System;

namespace maxprofitness.shared
{
    /// <summary>
    /// Used to store the repetition data for the match
    /// </summary>
    [Serializable]
    public sealed class RepetitionTypeData
    {
        public int BasicRepetitionPoints;
        public int SpecialRepetitionPoints;
        public int ExtraRepetitionPoints;
    }
}
