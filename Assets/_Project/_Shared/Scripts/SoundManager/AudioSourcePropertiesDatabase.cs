using System;
using System.Collections.Generic;
using UnityEngine;

namespace MaxProFitness.SharedSound
{
    [CreateAssetMenu(menuName = "Sound Manager/Audio Source Properties Database")]
    public class AudioSourcePropertiesDatabase : ScriptableObject
    {
        [SerializeField]private List<MinigameAudioSources> _minigameAudioSourcesList;

        public List<MinigameAudioSources> MinigameAudioSourcesList => _minigameAudioSourcesList;
    }

    [Serializable]
    public struct MinigameAudioSources
    {
        public AudioSourceCategory AudioSourceCategory;
        public List<AudioSourceProperties> AudioSources;
    }

    public enum AudioSourceCategory
    {
        SHARED = 0,
        APP = 1,
        FIT_FIGHTER = 2,
        ROWING_CANOE = 3,
        SKIING_TRACK = 4,
        GUNSLINGER = 5,
    }
}
