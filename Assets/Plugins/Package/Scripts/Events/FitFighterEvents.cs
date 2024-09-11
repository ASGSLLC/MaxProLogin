//using _Project.App.Scripts.Application.Account.History;
//using _Project.FitFighter.RhythmRevamp.Scripts.RhythmGamemode;
//using FitFighter.RhythmRevamp.Scripts.Combat;
//using FitFighter.RhythmRevamp.Scripts.Rounds;
//using FitFighter.RhythmRevamp.Scripts.Score;
//using MaxProFitness.History;
//using MaxProFitness.Shared.Inputs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using maxprofitness.login;
public static class FitFighterEvents 
{
    //-------------------------------------------------------//
    // Hit 
    //-------------------------------------------------------//
    [Tooltip("side, characteType, points")]
    public static Action<ActionSide, CharacterType, int> RhythmHitExecutedEvent;

    //-------------------------------------------------------//
    // Intro 
    //-------------------------------------------------------//
    public static Action RhythmIntroFinishedEvent;

    //-------------------------------------------------------//
    // Called to start the match 
    //-------------------------------------------------------//
    public static Action<RhythmChallengeLevelConfig> RhythmMatchStartedEvent;

    //-------------------------------------------------------//
    // Match Officially Started and Ended
    //-------------------------------------------------------//
    public static Action RhythmStartMatchEvent;
    public static Action RhythmMatchEndedEvent;


    //-------------------------------------------------------//
    // Match Rounds Started and Ended
    //-------------------------------------------------------//
    [Tooltip("turnType, turnNumber, turnRepetitionsGoal")]
    public static Action<RhythmTurnType, int, int> RhythmTurnStartedEvent;

    [Tooltip("turnType, roundEnded")]
    public static Action<RhythmTurnType, int> RhythmTurnEndedEvent;



    [Tooltip("playerWon")]
    public static Action<bool> MatchResultEvent;



    //-------------------------------------------------------//
    // Tutorial 
    //-------------------------------------------------------//
    public static Action RhythmTutorialStartedEvent;
    public static Action RhythmTutorialFinishedEvent;


    //-------------------------------------------------------//
    // Gameplay Events
    //-------------------------------------------------------//
    public static Action PlayerIsIdleEvent;

    public static Action SpecialAttackFinishEvent;

    [Tooltip("time")]
    public static Action<int> RhythmTimerUpdatedEvent;

    [Tooltip("wasGoalReached")]
    public static Action<bool> RepetitionGoalReachEvent;

    public static Action<Repetition> OnRepetitionExecuted;

    public static Action<MatchResult> OnMatchResultsReady;

    public static Action<PunchingRoundsMetrics> RhythmSendRestScoreEvent;

    [Tooltip("playerWon")]
    public static Action<bool> RhythmTurnResultEvent;

    public static Action<MatchResult> MatchResultsCreatedEvent;

    /// <summary>
    /// punchingRoundsMetrics
    /// </summary>
    public static Action<PunchingRoundsMetrics> PunchingRoundsScoreUpdatedEvent;

    public class Results
    {
        public int Round;
        public int TotalReps;
        public int RoundReps;
        public int TotalScore;
        public int RoundScore;
    }

    public static Action JumbotronAnimationEndEvent;

    public static Action<ActionSide> RhythmInputDisabledPunchEvent;

    public static Action SkipRestTurnEvent;

}
