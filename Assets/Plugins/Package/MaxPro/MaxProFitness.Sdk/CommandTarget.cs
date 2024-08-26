using UnityEngine.Scripting;

namespace MaxProFitness.Sdk
{
    [Preserve]
    public enum CommandTarget : byte
    {
        Left = 0x01,
        Right = 0x02,
        Both = 0x03,
    }
}
