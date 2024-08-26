using UnityEngine.Scripting;

namespace MaxProFitness.Sdk
{
    [Preserve]
    public enum MaxProModel : byte
    {
        MaxProSimulator = 0x00,
        MaxProElectronicsBasic = 0x01,
        MaxProActuator = 0x11,
    }
}
