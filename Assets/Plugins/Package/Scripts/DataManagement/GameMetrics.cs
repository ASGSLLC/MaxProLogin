// Class for holding data specific to each game in this app
using System;
using System.Collections.Generic;
using UnityEngine;
#if FIT_FIGHTER
using _Project.FitFighter.RhythmRevamp.Scripts.Score;
#endif
[SerializeField]
public class GameMetrics
{
    #region AIR RUNNER GAME METRICS


    [System.Serializable]
    public class AirRunnerGameMetrics
    {
#if AIR_RUNNER
        public enum Game
        {
            Hills = 0,
            DarkSky = 1,
            Desert = 2,
            Forest = 3,
        }

        public string date;

        public string[] leaderboardScore = new string[2];
        public int longestStreak;
        public int highScore;
        public int highestDifficultyLevel;
        public int bestScore;
        public int currentScore;
        public int ringsCollected;
        public Game game;
#endif
    } // END AirRunnerGameMetrics.cs


#endregion


    #region FIT FIGHTER METRICS


    [System.Serializable]
    public class FitFighterGameMetrics
    {
#if FIT_FIGHTER
        public string gradeMessage;
        public string date;
        public string timeToFinish;
        public string[] leaderboardScore = new string[2];

        public int roundsWon;
        public int repetitionsExecuted;
        public int extraRepetitions;
        public int score;
        public int matchDuration;
        public int currentRound;
        public int peakWork;
        public int totalWork;
        public int repetitions;
        public int averageWork;
        public int caloriesBurned;

        public int roundNumber;
        public int hits;
        public int extraHits;
        public int roundDuration;
        public int scorePerRound;

        public int[] workByRepetitions;
        public int[] powerList;
        public int[] repetitionsPerRound;


        public List<PunchingRoundsRoundScore> roundScore;
#endif
    } // END FitFighterGameMetrics.cs


#endregion


    #region ROWING METRICS


    [System.Serializable]
    public class RowingCanoeGameMetrics
    {
#if ROWING_CANOE
        public string timeToFinish;
        public string date;
        public string[] leaderboardScore = new string[2];

        public int highScore;
        public int strokes;
        public int averageWork;
        public int totalWork;
        public int peakWork;
        public int caloriesBurned;
        public int repetitions;

        public double maxSpeed;
        public double averageSpeed;
        public double paceMinutes;
        public double paceSeconds;
        public double cadence;

        public int[] workByRepetitions;
        public int[] powerList;
#endif
    } // END RowingCanoeGameMetrics


#endregion


} // END GameMetrics.cs
