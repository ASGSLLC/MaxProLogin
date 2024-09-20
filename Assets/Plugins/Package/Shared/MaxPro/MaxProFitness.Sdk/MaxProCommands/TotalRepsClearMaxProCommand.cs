using System;
using UnityEngine.Scripting;

namespace maxprofitness.shared
{
    /// <summary>
    ///     Response to <see cref="TotalRepsClearAppCommand"/>.
    /// </summary>
    [Preserve]
    [Serializable]
    public struct TotalRepsClearMaxProCommand : IMaxProCommand
    {
        public CommandResult Result;

        public CommandType CommandType => CommandType.TotalRepsClear;

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
