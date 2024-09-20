using UnityEngine.Scripting;

namespace maxprofitness.shared
{
    [Preserve]
    public enum MaxProModel : byte
    {
        MaxProSimulator = 0x00,
        MaxProElectronicsBasic = 0x01,
        MaxProActuator = 0x11,
    }
}
