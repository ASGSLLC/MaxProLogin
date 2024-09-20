using System;
using UnityEngine.Scripting;

namespace maxprofitness.shared
{
    /// <summary>
    ///     Response to <see cref="DisconnectAppCommand"/>.
    /// </summary>
    [Preserve]
    [Serializable]
    public struct DisconnectMaxProCommand : IMaxProCommand
    {
        public CommandResult Result;

        public CommandType CommandType => CommandType.Disconnect;

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
