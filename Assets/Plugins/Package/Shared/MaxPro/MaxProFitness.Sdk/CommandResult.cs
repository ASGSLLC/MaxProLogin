using UnityEngine.Scripting;

namespace maxprofitness.shared
{
    [Preserve]
    public enum CommandResult : byte
    {
        Completed = 0x01,
        Received = 0x02,
        Timeout = 0xFC,
        CantAcceptCommand = 0xFD,
        NoneActuatorVersion = 0xFE,
        GenericError = 0xFF,
    }
}
