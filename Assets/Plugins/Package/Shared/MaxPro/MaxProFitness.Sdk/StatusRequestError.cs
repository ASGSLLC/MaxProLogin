using UnityEngine.Scripting;

namespace maxprofitness.shared
{
    [Preserve]
    public enum StatusRequestError : byte
    {
        None = 0x00,
        LowBattery = 0x01,
    }
}
