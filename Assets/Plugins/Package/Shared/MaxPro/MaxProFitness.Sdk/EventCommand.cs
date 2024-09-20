using System;
using UnityEngine.Scripting;
using MaxProFitness.Sdk;

namespace maxprofitness.shared
{
    /// <summary>
    ///     Sent by the device automatically after connecting while game mode is off. App must respond to the received data as it is.
    /// </summary>
    [Preserve]
    [Serializable]
    public struct EventCommand : IAppCommand, IMaxProCommand, IEquatable<EventCommand>
    {
        public const byte FirstHalfSize = 17;

        public const byte SecondHalfSize = 12;

        public const byte TotalSize = 29;

        public EventType EventType;

        public MaxProModel Model;

        public StatusRequestError Error;

        public byte BatteryPercent;

        /// <summary>
        ///     From 10 to 255.
        /// </summary>
        public byte LeftKnobPosition;

        /// <summary>
        ///     In millimeters.
        /// </summary>
        public ushort LeftDistance;

        /// <summary>
        ///     In centiseconds (0.01s).
        /// </summary>
        public ushort LeftTime;

        /// <summary>
        ///     In millimeters per second.
        /// </summary>
        public ushort LeftVelocity;

        /// <summary>
        ///     In millimeters per second squared.
        /// </summary>
        public ushort LeftAcceleration;

        public ushort LeftRepsCount;

        /// <summary>
        ///     From 10 to 255.
        /// </summary>
        public byte RightKnobPosition;

        /// <summary>
        ///     In millimeters.
        /// </summary>
        public ushort RightDistance;

        /// <summary>
        ///     In centiseconds (0.01s).
        /// </summary>
        public ushort RightTime;

        /// <summary>
        ///     In millimeters per second.
        /// </summary>
        public ushort RightVelocity;

        /// <summary>
        ///     In millimeters per second squared.
        /// </summary>
        public ushort RightAcceleration;

        public ushort RightRepsCount;

        public CommandType CommandType => CommandType.Event;

        /// <summary>
        ///     The left exercise count will be changed.
        /// </summary>
        public bool LeftExerciseCountChanged => (EventType & EventType.LeftExerciseCount) != 0;

        /// <summary>
        ///     The right exercise count will be changed.
        /// </summary>
        public bool RightExerciseCountChanged => (EventType & EventType.RightExerciseCount) != 0;

        /// <summary>
        ///     The left knob position will be changed,
        /// </summary>
        public bool LeftKnobPositionChanged => (EventType & EventType.LeftKnobPosition) != 0;

        /// <summary>
        ///     The right knob position will be changed,
        /// </summary>
        public bool RightKnobPositionChanged => (EventType & EventType.RightKnobPosition) != 0;

        /// <summary>
        ///     User wants to power off with touch switch.
        /// </summary>
        public bool UserPowerOff => (EventType & EventType.UserPowerOff) != 0;

        /// <summary>
        ///     Auto power off due to low batter.
        /// </summary>
        public bool BatteryPowerOff => (EventType & EventType.BatteryPowerOff) != 0;

        /// <summary>
        ///     Exercise time is changed (occurs every 1 minute).
        /// </summary>
        public bool ExerciseTimeChanged => (EventType & EventType.ExerciseTime) != 0;

        private bool _filledFirstHalf;
        private bool _filledSecondHalf;

        public static bool operator ==(EventCommand left, EventCommand right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EventCommand left, EventCommand right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            return obj is EventCommand other && Equals(other);
        }

        public bool Equals(EventCommand other)
        {
            return EventType == other.EventType
                && Model == other.Model
                && Error == other.Error
                && BatteryPercent == other.BatteryPercent
                && LeftKnobPosition == other.LeftKnobPosition
                && LeftDistance == other.LeftDistance
                && LeftTime == other.LeftTime
                && LeftVelocity == other.LeftVelocity
                && LeftAcceleration == other.LeftAcceleration
                && LeftRepsCount == other.LeftRepsCount
                && RightKnobPosition == other.RightKnobPosition
                && RightDistance == other.RightDistance
                && RightTime == other.RightTime
                && RightVelocity == other.RightVelocity
                && RightAcceleration == other.RightAcceleration
                && RightRepsCount == other.RightRepsCount;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)EventType;
                hashCode = (hashCode * 397) ^ (int)Model;
                hashCode = (hashCode * 397) ^ (int)Error;
                hashCode = (hashCode * 397) ^ BatteryPercent.GetHashCode();
                hashCode = (hashCode * 397) ^ LeftKnobPosition.GetHashCode();
                hashCode = (hashCode * 397) ^ LeftDistance.GetHashCode();
                hashCode = (hashCode * 397) ^ LeftTime.GetHashCode();
                hashCode = (hashCode * 397) ^ LeftVelocity.GetHashCode();
                hashCode = (hashCode * 397) ^ LeftAcceleration.GetHashCode();
                hashCode = (hashCode * 397) ^ LeftRepsCount.GetHashCode();
                hashCode = (hashCode * 397) ^ RightKnobPosition.GetHashCode();
                hashCode = (hashCode * 397) ^ RightDistance.GetHashCode();
                hashCode = (hashCode * 397) ^ RightTime.GetHashCode();
                hashCode = (hashCode * 397) ^ RightVelocity.GetHashCode();
                hashCode = (hashCode * 397) ^ RightAcceleration.GetHashCode();
                hashCode = (hashCode * 397) ^ RightRepsCount.GetHashCode();

                return hashCode;
            }
        }

        public bool Deserialize(byte[] data)
        {
            if (data.Length == TotalSize || data.Length == FirstHalfSize)
            {
                _filledFirstHalf = true;
                EventType = (EventType)data[2];
                Model = (MaxProModel)data[3];
                Error = (StatusRequestError)data[4];
                BatteryPercent = data[5];
                LeftKnobPosition = data[6];
                LeftDistance = ConversionUtility.ToUshort(data, 7);
                LeftTime = ConversionUtility.ToUshort(data, 9);
                LeftVelocity = ConversionUtility.ToUshort(data, 11);
                LeftAcceleration = ConversionUtility.ToUshort(data, 13);
                LeftRepsCount = ConversionUtility.ToUshort(data, 15);
            }

            if (data.Length == TotalSize || data.Length == SecondHalfSize)
            {
                int diff = data.Length == SecondHalfSize ? FirstHalfSize : 0;
                _filledSecondHalf = true;
                RightKnobPosition = data[FirstHalfSize];
                RightDistance = ConversionUtility.ToUshort(data, FirstHalfSize - diff + 1);
                RightTime = ConversionUtility.ToUshort(data, FirstHalfSize - diff + 3);
                RightVelocity = ConversionUtility.ToUshort(data, FirstHalfSize - diff + 5);
                RightAcceleration = ConversionUtility.ToUshort(data, FirstHalfSize - diff + 7);
                RightRepsCount = ConversionUtility.ToUshort(data, FirstHalfSize - diff + 9);
            }

            return _filledFirstHalf && _filledSecondHalf;
        }

        public string ToHexData()
        {
            unchecked
            {
                return $"{(byte)EventType:x2}{(byte)Model:x2}{(byte)Error:x2}{BatteryPercent:x2}"
                     + $"{LeftKnobPosition:x2}{LeftDistance:x4}{LeftTime:x4}{LeftVelocity:x4}{LeftAcceleration:x4}{LeftRepsCount:x4}"
                     + $"{RightKnobPosition:x2}{RightDistance:x4}{RightTime:x4}{RightVelocity:x4}{RightAcceleration:x4}{RightRepsCount:x4}";
            }
        }
    }
}
