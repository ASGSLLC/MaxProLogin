using System;
using UnityEngine.Scripting;

namespace MaxProFitness.Sdk
{
    /// <summary>
    ///     Response to <see cref="ConnectAppCommand"/>.
    /// </summary>
    [Preserve]
    [Serializable]
    public struct ConnectMaxProCommand : IMaxProCommand
    {
        public CommandResult Result;

        /// <summary>
        ///     The device firmware version information.
        /// </summary>
        public string Version;

        public CommandType CommandType => CommandType.Connect;

        public bool Deserialize(byte[] data)
        {
            Result = (CommandResult)data[2];
            Version = ConversionUtility.ToAscii(data, 3, data.Length - 4);

            return true;
        }

        public string ToHexData()
        {
            return $"{(byte)Result:x2}{ConversionUtility.ToHex(Version)}";
        }
    }
}
