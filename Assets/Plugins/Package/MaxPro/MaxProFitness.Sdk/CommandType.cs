using UnityEngine.Scripting;

namespace MaxProFitness.Sdk
{
    [Preserve]
    public enum CommandType : byte
    {
        Connect = 0x01,
        StatusRequest = 0x02,
        TotalRepsClear = 0x03,
        SetKnobActuatorPosition = 0x04,
        SetWindingStartPosition = 0x05,
        MinimumLengthToPull = 0x06,
        Event = 0x07,
        KnobPositionCalibration = 0x08,
        Disconnect = 0x09,
        GameEventRequest = 0x0A,
        GameEventRequestUpdate = 0x8A,
        SetWifiModuleName = 0x0B,
    }
}
