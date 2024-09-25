using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using maxprofitness.login;

namespace maxprofitness.shared
{
    public static class CanoeEvents
    {
        /// <summary>
        /// float updatedAnimatorSpeed is passed in from Event
        /// </summary>
        public static Action<float> CanoeAnimationSpeedUpdateEvent;

        /// <summary>
        /// float animatorSpeed is passed in from Event
        /// </summary>
        public static Action<float> CanoeAnimationSpeedEvent;

#if MAXPRO_LOGIN
        /// <summary>
        /// 
        /// </summary>
        public static Action<MinigameDifficulty> DifficultySelectionChangeEvent;
#endif
        public static Action MinigameFinishedEvent;

        public static Action<Vector3> PlayerMovementDirectionEvent;

        public static Action WinRaceTimelineEvent;

        public static Action LoseRaceTimelineEvent;

        public static Action RowingRaceStartedEvent;

        public static Action RowingRaceEndedEvent;
    }
}