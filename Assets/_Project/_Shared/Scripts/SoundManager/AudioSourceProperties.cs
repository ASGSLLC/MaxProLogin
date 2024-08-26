using UnityEngine.Audio;
using UnityEngine;

namespace MaxProFitness.SharedSound
{
    [CreateAssetMenu(menuName = "Sound Manager/Shared Audio Source Properties")]
    public sealed class AudioSourceProperties : ScriptableObject
    {
        public SharedGameSound Sound;

        [Space(10)]
        public AudioClip[] AudioClips;

        [Space(10)]
        public AudioMixerGroup AudioMixerGroup;

        [Space(10)]
        [Range(0f, 1f)]
        public float Volume = 0.5f;

        [Range(0f, 3f)]
        public float Pitch = 1f;

        [Range(0f, 1f)]
        public float SpatialBlend = 0.5f;

        [Space(10)]
        public bool Loop;

        public bool PersistentSound;

        [SerializeField]
        private bool _randomizePitch = false;
    
        [SerializeField]
        private float _minPitch = 0.8f;
    
        [SerializeField]
        private float _maxPitch = 1.2f;

        private bool _isPlaying;

        public bool IsPlaying
        {
            get => _isPlaying;
            set => _isPlaying = value;
        }

        public bool RandomizePitch => _randomizePitch;
        public float MinPitch => _minPitch;
        public float MaxPitch => _maxPitch;
    }
}
