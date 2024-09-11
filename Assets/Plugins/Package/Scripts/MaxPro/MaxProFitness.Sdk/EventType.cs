using System;
using UnityEngine.Scripting;
using MaxProFitness.Sdk;

namespace maxprofitness.login
{
    [Flags]
    [Preserve]
    public enum EventType
    {
        /// <summary>
        ///     The left exercise count will be changed.
        /// </summary>
        LeftExerciseCount = 1 << 0,
        /// <summary>
        ///     The right exercise count will be changed.
        /// </summary>
        RightExerciseCount = 1 << 1,
        /// <summary>
        ///     The left knob position will be changed,
        /// </summary>
        LeftKnobPosition = 1 << 2,
        /// <summary>
        ///     The right knob position will be changed,
        /// </summary>
        RightKnobPosition = 1 << 3,
        /// <summary>
        ///     User wants to power off with touch switch.
        /// </summary>
        UserPowerOff = 1 << 4,
        /// <summary>
        ///     Auto power off due to low batter.
        /// </summary>
        BatteryPowerOff = 1 << 5,
        /// <summary>
        ///     Exercise time is changed (occurs every 1 minute).
        /// </summary>
        ExerciseTime = 1 << 6,
    }
}
