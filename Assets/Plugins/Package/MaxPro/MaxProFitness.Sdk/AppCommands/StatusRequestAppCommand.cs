using System;
using UnityEngine.Scripting;

namespace MaxProFitness.Sdk
{
    /// <summary>
    ///     It is expected to receive a <see cref="StatusRequestMaxProCommand"/> as response. Does not work while game mode is on!
    /// </summary>
    [Preserve]
    [Serializable]
    public struct StatusRequestAppCommand : IAppCommand
    {
        public CommandType CommandType => CommandType.StatusRequest;

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
