using System;
using UnityEngine.Scripting;

namespace MaxProFitness.Sdk
{
    /// <summary>
    ///     It is expected to receive a <see cref="SetWindingStartPositionMaxProCommand"/> as response.
    /// </summary>
    [Preserve]
    [Serializable]
    public struct SetWindingStartPositionAppCommand : IAppCommand
    {
        public CommandTarget Target;

        /// <summary>
        ///     In millimeters
        /// </summary>
        public ushort StartPosition;

        public SetWindingStartPositionAppCommand(CommandTarget target, ushort startPosition)
        {
            Target = target;
            StartPosition = startPosition;
        }

        public CommandType CommandType => CommandType.SetWindingStartPosition;

        public bool Deserialize(byte[] data)
        {
            Target = (CommandTarget)data[2];
            StartPosition = ConversionUtility.ToUshort(data, 3);

            return true;
        }

        public string ToHexData()
        {
            return $"{(byte)Target:x2}{StartPosition:x4}";
        }
    }
}
