//using _Project.FitFighter.RhythmRevamp.Scripts.Score;
//using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace maxprofitness.shared
{
    [Serializable]
    public struct PunchingRoundsMetrics
    {
        public string GradeMessage;
        public int RoundsWon;
        public int RepetitionsExecuted;
        public int ExtraRepetitions;
        public int Score;
        public int MatchDuration;

        public int CurrentRound;
        public List<PunchingRoundsRoundScore> RoundScore;
        public List<int> RepetitionsPerRound;

        public int CurrentRepetitions => RoundScore != null ? RoundScore[CurrentRound].Hits : 0;
        public int CurrentScore => RoundScore != null ? RoundScore[CurrentRound].Score : 0;

        public float PunchSpeed => GetPunchSpeed();
        public float Cadence => GetCadence();
        public float PunchRate => GetPunchRate();

        public PunchingRoundsMetrics(int totalRounds)
        {
            RoundScore = new List<PunchingRoundsRoundScore>();
            RepetitionsPerRound = new List<int>();

            for (int i = 0; i < totalRounds; i++)
            {
                RoundScore.Add(new PunchingRoundsRoundScore());
                RepetitionsPerRound.Add(0);
            }

            RoundsWon = 0;
            RepetitionsExecuted = 0;
            ExtraRepetitions = 0;
            Score = 0;
            CurrentRound = 0;
            GradeMessage = string.Empty;
            MatchDuration = 0;
        }

        public void AddRepetition(int round, int value, bool isPlayerBlocking)
        {
            //Debug.Log("PunchingRoundsMetrics//AddRepetition//");

            RoundScore ??= new List<PunchingRoundsRoundScore>();

            Score += value;

            PunchingRoundsRoundScore roundsRoundScore = RoundScore[round];

            if (isPlayerBlocking == false)
            {
                RepetitionsExecuted++;
                RepetitionsPerRound[round]++;
                roundsRoundScore.Hits++;
            }

            roundsRoundScore.Score += value;

            RoundScore[round] = roundsRoundScore;

            InvokeScoreUpdatedEvent();
        }

        public void RemoveScore(int round, int value)
        {
            Score += value;

            PunchingRoundsRoundScore roundsRoundScore = RoundScore[round];

            if (roundsRoundScore.Hits > 0)
            {
                roundsRoundScore.Hits--;
                roundsRoundScore.ExtraHits = 0;
            }

            roundsRoundScore.Score += value;

            RoundScore[round] = roundsRoundScore;

            InvokeScoreUpdatedEvent();
        }

        public void AddExtraRepetition(int round)
        {
            PunchingRoundsRoundScore punchingRoundsRoundScore = RoundScore[round];
            punchingRoundsRoundScore.ExtraHits++;

            RoundScore[round] = punchingRoundsRoundScore;

            InvokeScoreUpdatedEvent();
        }

        public void SetLastRound(int round)
        {
            CurrentRound = round;
        }

        public void AddRoundWon()
        {
            RoundsWon++;

            InvokeScoreUpdatedEvent();
        }

        public void SetRoundExtraRepetitionsPerformed(int round)
        {
            ExtraRepetitions += RoundScore[round].ExtraHits;
        }

        public void ResetExtraRepetitionsStreak(int round, int valueToReset)
        {
            PunchingRoundsRoundScore punchingRoundsRoundScore = RoundScore[round];
            punchingRoundsRoundScore.Hits = valueToReset + 1; // adding one here as a workaround because now we always subtract one when receiving a punch
            RoundScore[round] = punchingRoundsRoundScore;
        }

        private float GetPunchSpeed()
        {
            if (RoundScore.Count == 0 || RoundScore == null)
            {
                return 0;
            }

            float repetitionsExecuted = RepetitionsExecuted / (float)MatchDuration;

            return repetitionsExecuted;
        }

        private float GetCadence()
        {
            if (RoundScore.Count == 0 || RoundScore == null)
            {
                return 0;
            }

            float minutes = MatchDuration / 60f;

            return RepetitionsExecuted / minutes;
        }

        private float GetPunchRate()
        {
            if (RoundScore.Count == 0 || RoundScore == null)
            {
                return 0;
            }

            return RepetitionsExecuted / (float)RoundScore.Count;
        }

        private void InvokeScoreUpdatedEvent()
        {
            FitFighterEvents.PunchingRoundsScoreUpdatedEvent?.Invoke(this);
        }
    }
}
