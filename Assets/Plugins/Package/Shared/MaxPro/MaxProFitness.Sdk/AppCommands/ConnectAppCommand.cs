using System;
using UnityEngine.Scripting;

namespace maxprofitness.shared
{
    /// <summary>
    ///     Initial command needed to connect to the device. It is expected to receive a <see cref="ConnectMaxProCommand"/> as response.
    /// </summary>
    [Preserve]
    [Serializable]
    public struct ConnectAppCommand : IAppCommand
    {
        public CommandType CommandType => CommandType.Connect;

        public bool Deserialize(byte[] data)
        {
            return true;
        }

        public string ToHexData()
        {
            return string.Empty;
        }
    }
}
