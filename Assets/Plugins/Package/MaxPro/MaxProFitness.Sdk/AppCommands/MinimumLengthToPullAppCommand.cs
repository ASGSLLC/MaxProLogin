using System;
using UnityEngine.Scripting;
using MaxProFitness.Sdk;

namespace maxprofitness.login
{
    /// <summary>
    ///     It is expected to receive a <see cref="MinimumLenghtToPullMaxProCommand"/> as response.
    /// </summary>
    [Preserve]
    [Serializable]
    public struct MinimumLengthToPullAppCommand : IAppCommand
    {
        public CommandTarget Target;

        /// <summary>
        ///     1 unit equals to 32mm
        /// </summary>
        public byte Length;

        public MinimumLengthToPullAppCommand(CommandTarget target, byte length)
        {
            Target = target;
            Length = length;
        }

        public CommandType CommandType => CommandType.MinimumLengthToPull;

        public bool Deserialize(byte[] data)
        {
            Target = (CommandTarget)data[2];
            Length = data[3];

            return true;
        }

        public string ToHexData()
        {
            return $"{(byte)Target:x2}{Length:x2}";
        }
    }
}
