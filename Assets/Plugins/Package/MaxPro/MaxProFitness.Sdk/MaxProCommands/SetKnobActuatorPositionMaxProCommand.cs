using System;
using UnityEngine.Scripting;

namespace MaxProFitness.Sdk
{
    /// <summary>
    ///     Response to <see cref="SetKnobActuatorPositionAppCommand"/>.
    /// </summary>
    [Preserve]
    [Serializable]
    public struct SetKnobActuatorPositionMaxProCommand : IMaxProCommand
    {
        public CommandResult Result;

        public CommandType CommandType => CommandType.SetKnobActuatorPosition;

        public bool Deserialize(byte[] data)
        {
            Result = (CommandResult)data[2];

            return true;
        }

        public string ToHexData()
        {
            return $"{(byte)Result:x2}";
        }
    }
}
