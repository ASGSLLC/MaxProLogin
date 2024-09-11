//using _Project.App.Scripts.Minigames;
//using FitFighter.RhythmRevamp.Scripts.Score;
//using MaxProFitness.App.TrainingRoutines;
//using MaxProFitness.History;
//using MaxProFitness.Shared.Utilities;
using System;
using System.Collections.Generic;

namespace maxprofitness.login
{
    [Serializable]
    public struct MatchResult : IMetric
    {
        public Minigame Minigame;
        public int Score;
        public int AverageWork;
        public int TotalWork;
        public int PeakWork;
        public int CaloriesBurned;
        public int Repetitions;
        public List<int> WorkByRepetitions;
        public List<int> PowerList;
        public TimeStruct TimeToFinish;
        public DateStruct Date;

        public RowingMetrics RowingMetrics;
        public PunchingRoundsMetrics PunchingRoundsMetrics;
    }
}
