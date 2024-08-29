using System;
using UnityEngine.Scripting;
using MaxProFitness.Sdk;
namespace maxprofitness.login
{
    /// <summary>
    ///     Response to <see cref="KnobPositionCalibrationAppCommand"/>.
    /// </summary>
    [Preserve]
    [Serializable]
    public struct KnobPositionCalibrationMaxProCommand : IMaxProCommand
    {
        public CommandResult Result;

        public KnobPositionCalibrationStep Step;

        public CommandType CommandType => CommandType.KnobPositionCalibration;

        public bool Deserialize(byte[] data)
        {
            Result = (CommandResult)data[2];
            Step = (KnobPositionCalibrationStep)data[3];

            return true;
        }

        public string ToHexData()
        {
            return $"{(byte)Result:x2}{(byte)Step:x2}";
        }
    }
}
