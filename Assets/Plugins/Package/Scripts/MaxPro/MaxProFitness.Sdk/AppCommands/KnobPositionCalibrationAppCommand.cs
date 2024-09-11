using System;
using UnityEngine.Scripting;
using MaxProFitness.Sdk;

namespace maxprofitness.login
{
    /// <summary>
    ///     Before executing this command you must first select the correct knob position. Be sure to do it in the same order as the
    ///     <see cref="KnobPositionCalibrationStep"/> values! It is expected to receive a <see cref="KnobPositionCalibrationMaxProCommand"/> as response.
    /// </summary>
    [Preserve]
    [Serializable]
    public struct KnobPositionCalibrationAppCommand : IAppCommand
    {
        public KnobPositionCalibrationStep Step;

        public KnobPositionCalibrationAppCommand(KnobPositionCalibrationStep step)
        {
            Step = step;
        }

        public CommandType CommandType => CommandType.KnobPositionCalibration;

        public bool Deserialize(byte[] data)
        {
            Step = (KnobPositionCalibrationStep)data[2];

            return true;
        }

        public string ToHexData()
        {
            return $"{(byte)Step:x2}";
        }
    }
}
