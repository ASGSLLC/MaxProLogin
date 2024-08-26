using System;
using UnityEngine.Scripting;

namespace MaxProFitness.Sdk
{
    /// <summary>
    ///     Response to <see cref="GameEventRequestAppCommand"/>.
    /// </summary>
    [Preserve]
    [Serializable]
    public struct GameEventRequestMaxProCommand : IMaxProCommand
    {
        public bool IsGameModeOn;

        public CommandType CommandType => CommandType.GameEventRequest;

        public bool Deserialize(byte[] data)
        {
            IsGameModeOn = data[2] == 1;

            return true;
        }

        public string ToHexData()
        {
            return $"{(IsGameModeOn ? "01" : "00")}";
        }
    }
}
