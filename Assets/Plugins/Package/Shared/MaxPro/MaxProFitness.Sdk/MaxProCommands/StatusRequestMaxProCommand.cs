using System;
using UnityEngine.Scripting;

namespace maxprofitness.shared
{
    /// <summary>
    ///     Response to <see cref="StatusRequestAppCommand"/>.
    /// </summary>
    [Preserve]
    [Serializable]
    public struct StatusRequestMaxProCommand : IMaxProCommand
    {
        public const byte FirstHalfSize = 17;

        public const byte SecondHalfSize = 12;

        public const byte TotalSize = 29;

        public CommandResult Result;

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

        public CommandType CommandType => CommandType.StatusRequest;

        private bool _filledFirstHalf;
        private bool _filledSecondHalf;

        public bool Deserialize(byte[] data)
        {
            if (data.Length == TotalSize || data.Length == FirstHalfSize)
            {
                _filledFirstHalf = true;
                Result = (CommandResult)data[2];
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
            return $"{(byte)Result:x2}{(byte)Model:x2}{(byte)Error:x2}{BatteryPercent:x2}"
                 + $"{LeftKnobPosition:x2}{LeftDistance:x4}{LeftTime:x4}{LeftVelocity:x4}{LeftAcceleration:x4}{LeftRepsCount:x4}"
                 + $"{RightKnobPosition:x2}{RightDistance:x4}{RightTime:x4}{RightVelocity:x4}{RightAcceleration:x4}{RightRepsCount:x4}";
        }
    }
}
