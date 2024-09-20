using System;
using UnityEngine.Scripting;

namespace maxprofitness.shared
{
    /// <summary>
    ///     It is expected to receive a <see cref="SetKnobActuatorPositionMaxProCommand"/> as response.
    /// </summary>
    [Preserve]
    [Serializable]
    public struct SetKnobActuatorPositionAppCommand : IAppCommand
    {
        public CommandTarget Target;

        /// <summary>
        ///     From 10 to 255;
        /// </summary>
        public byte KnobPosition;

        public SetKnobActuatorPositionAppCommand(CommandTarget target, byte knobPosition)
        {
            Target = target;
            KnobPosition = knobPosition;
        }

        public CommandType CommandType => CommandType.SetKnobActuatorPosition;

        public bool Deserialize(byte[] data)
        {
            Target = (CommandTarget)data[2];
            KnobPosition = data[3];

            return true;
        }

        public string ToHexData()
        {
            return $"{(byte)Target:x2}{KnobPosition:x2}";
        }
    }
}
