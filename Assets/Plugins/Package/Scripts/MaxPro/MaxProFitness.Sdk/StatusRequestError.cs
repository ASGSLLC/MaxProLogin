using UnityEngine.Scripting;

namespace MaxProFitness.Sdk
{
    [Preserve]
    public enum StatusRequestError : byte
    {
        None = 0x00,
        LowBattery = 0x01,
    }
}
