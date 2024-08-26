using System;
using UnityEngine.Scripting;

namespace MaxProFitness.Sdk
{
    /// <summary>
    ///     Command to disconnect from the device. It is expected to receive a <see cref="DisconnectMaxProCommand"/> as response.
    /// </summary>
    [Preserve]
    [Serializable]
    public struct DisconnectAppCommand : IAppCommand
    {
        public CommandType CommandType => CommandType.Disconnect;

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
