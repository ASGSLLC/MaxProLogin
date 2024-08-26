using System;
using UnityEngine.Scripting;

namespace MaxProFitness.Sdk
{
    /// <summary>
    ///     Response to <see cref="MinimumLengthToPullAppCommand"/>.
    /// </summary>
    [Preserve]
    [Serializable]
    public struct MinimumLenghtToPullMaxProCommand : IMaxProCommand
    {
        public CommandResult Result;

        public CommandType CommandType => CommandType.MinimumLengthToPull;

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
