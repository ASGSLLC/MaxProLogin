using System;
using UnityEngine.Scripting;
using MaxProFitness.Sdk;

namespace maxprofitness.login
{
    /// <summary>
    ///     Causes the device bluetooth module to reset, requiring to re-connect.
    /// </summary>
    [Preserve]
    [Serializable]
    public struct SetWifiModuleNameAppCommand : IAppCommand
    {
        /// <summary>
        ///     Will be clamped to 16 bytes.
        /// </summary>
        public string Name;

        public SetWifiModuleNameAppCommand(string name)
        {
            Name = name;
        }

        public CommandType CommandType => CommandType.SetWifiModuleName;

        public bool Deserialize(byte[] data)
        {
            Name = ConversionUtility.ToAscii(data, 3, data[2]);

            return true;
        }

        public string ToHexData()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                Name = MaxProController.DefaultDeviceName;
            }

            if (Name.Length > 16)
            {
                Name = Name.Substring(0, 16);
            }

            return $"{Name.Length:x2}{ConversionUtility.ToHex(Name)}";
        }
    }
}
