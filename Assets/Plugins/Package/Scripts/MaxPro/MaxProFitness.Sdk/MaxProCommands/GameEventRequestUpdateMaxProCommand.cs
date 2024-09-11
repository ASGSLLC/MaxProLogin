using System;
using UnityEngine.Scripting;
using MaxProFitness.Sdk;
namespace maxprofitness.login
{
    /// <summary>
    ///     Sent by the device automatically every 0.04 seconds while game mode is on.
    /// </summary>
    [Preserve]
    [Serializable]
    public struct GameEventRequestUpdateMaxProCommand : IMaxProCommand
    {
        /// <summary>
        ///     From 10 to 255.
        /// </summary>
        public byte LeftKnobPosition;

        /// <summary>
        ///     In millimeters.
        /// </summary>
        public ushort LeftDistance;

        /// <summary>
        ///     From 10 to 255.
        /// </summary>
        public byte RightKnobPosition;

        /// <summary>
        ///     In millimeters.
        /// </summary>
        public ushort RightDistance;

        public CommandType CommandType => CommandType.GameEventRequestUpdate;

        public bool Deserialize(byte[] data)
        {
            LeftKnobPosition = data[2];
            LeftDistance = ConversionUtility.ToUshort(data, 3);
            RightKnobPosition = data[5];
            RightDistance = ConversionUtility.ToUshort(data, 6);

            return true;
        }

        public string ToHexData()
        {
            return $"{LeftKnobPosition:x2}{LeftDistance:x4}{RightKnobPosition:x2}{RightDistance:x4}";
        }
    }
}
