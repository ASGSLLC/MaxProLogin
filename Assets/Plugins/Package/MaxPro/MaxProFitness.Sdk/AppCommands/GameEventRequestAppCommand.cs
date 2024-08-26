using System;
using UnityEngine.Scripting;

namespace MaxProFitness.Sdk
{
    /// <summary>
    ///     Command to turn game mode on and off. It is expected to receive a <see cref="GameEventRequestMaxProCommand"/> as response immediately and then a
    ///     <see cref="GameEventRequestUpdateMaxProCommand"/> every 0.04 seconds while it is on.
    /// </summary>
    [Preserve]
    [Serializable]
    public struct GameEventRequestAppCommand : IAppCommand
    {
        public bool IsGameModeOn;

        public GameEventRequestAppCommand(bool isGameModeOn)
        {
            IsGameModeOn = isGameModeOn;
        }

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
