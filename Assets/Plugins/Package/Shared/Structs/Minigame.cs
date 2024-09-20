using System;
using UnityEngine.Serialization;

namespace maxprofitness.shared
{
    [Serializable]
    public struct Minigame
    {
        public MinigameType Type;
        public MinigameMode Mode;
        public MinigameDifficulty Difficulty;

        public Minigame(MinigameType type, MinigameMode mode, MinigameDifficulty difficulty)
        {
            Type = type;
            Mode = mode;
            Difficulty = difficulty;
        }
    }
}
