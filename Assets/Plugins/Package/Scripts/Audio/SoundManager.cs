using System;
using System.Collections.Generic;
using UnityEngine;

namespace maxprofitness.login
{
    public static class SoundManager
    {
        private static List<AudioSourceProperties> _audioSourceProperties = new List<AudioSourceProperties>();

        private static Dictionary<SharedGameSound, AudioSourceProperties> _audioSourcePropertiesDictionary;

        private static GameObject _soundPlayerPrefab;

        private static bool _initialized;

        private static List<GameObject> SpawnedSounds = new List<GameObject>();

        public static void Initialize(AudioSourcePropertiesDatabase audioSourcePropertiesDatabase)
        {
            if (_initialized)
            {
                return;
            }

            foreach (MinigameAudioSources minigameAudioSources in audioSourcePropertiesDatabase.MinigameAudioSourcesList)
            {
                foreach (AudioSourceProperties audio in minigameAudioSources.AudioSources)
                {
                    _audioSourceProperties.Add(audio);
                }
            }

            _audioSourcePropertiesDictionary = new Dictionary<SharedGameSound, AudioSourceProperties>();

            foreach (AudioSourceProperties audioSourceProperties in _audioSourceProperties)
            {
                if (!_audioSourcePropertiesDictionary.ContainsKey(audioSourceProperties.Sound))
                {
                    _audioSourcePropertiesDictionary.Add(audioSourceProperties.Sound, audioSourceProperties);
                }
                else
                {
                    Debug.LogError("Trying to add dictionary entry with repeated key: " + audioSourceProperties.Sound + ", this shouldn't happen");
                }
            }

            _initialized = true;
        }

        public static void PlaySound(SharedGameSound sound)
        {
            if (_audioSourcePropertiesDictionary.TryGetValue(sound, out AudioSourceProperties audioSourceProperties))
            {
                if (CanPlaySound(audioSourceProperties))
                {
                    SoundPlayer soundPlayer = GetSoundPlayer();
                    soundPlayer.PlaySound(audioSourceProperties);
                    audioSourceProperties.IsPlaying = true;
                }
            }
        }

        public static void PlaySound(SharedGameSound sound, SoundPlayer soundPlayer)
        {
            if (_audioSourcePropertiesDictionary.TryGetValue(sound, out AudioSourceProperties audioSourceProperties))
            {
                if (CanPlaySound(audioSourceProperties))
                {
                    soundPlayer.PlaySound(audioSourceProperties);

                    audioSourceProperties.IsPlaying = true;
                }
            }
        }
        public static void PlaySound(SharedGameSound sound, Vector3 position , Transform parent = null, bool isLocalPosition = false)
        {
            if (_audioSourcePropertiesDictionary.TryGetValue(sound, out AudioSourceProperties audioSourceProperties))
            {
                if (CanPlaySound(audioSourceProperties))
                {
                    SoundPlayer soundPlayer = GetSoundPlayer();
                    soundPlayer.PlaySound(audioSourceProperties, position);

                    if (parent != null)
                    {
                        soundPlayer.transform.SetParent(parent);

                        if (isLocalPosition)
                        {
                            soundPlayer.transform.localPosition = position;
                        }
                    }
                    audioSourceProperties.IsPlaying = true;
                }
            }
        }
        
        public static SoundPlayer PlaySound(SharedGameSound sound, float duration) // this is to play audio only with the same duration
        {
            if (_audioSourcePropertiesDictionary.TryGetValue(sound, out AudioSourceProperties audioSourceProperties))
            {
                if (CanPlaySound(audioSourceProperties))
                {
                    SoundPlayer soundPlayer = GetSoundPlayer();
                    soundPlayer.PlaySound(audioSourceProperties, duration);

                    audioSourceProperties.IsPlaying = true;

                    return soundPlayer;
                }
            }

            return null;
        }

        private static bool CanPlaySound(AudioSourceProperties audioSourceProperties)
        {
            return !audioSourceProperties.PersistentSound || !audioSourceProperties.IsPlaying;
        }

        private static SoundPlayer GetSoundPlayer()
        {
            GameObject soundPlayerGameObject = GameObject.Instantiate(_soundPlayerPrefab);

            SpawnedSounds.Add(soundPlayerGameObject);

            if (SpawnedSounds.Count > 5)
            {
                GameObject objectToRemove = SpawnedSounds[0];
                SpawnedSounds.Remove(objectToRemove);
                GameObject.Destroy(objectToRemove);
            }

            SoundPlayer soundPlayer = soundPlayerGameObject.GetComponent<SoundPlayer>();

            return soundPlayer;
        }

        public static void SetSoundPlayerPrefab(GameObject soundPlayerPrefab)
        {
            _soundPlayerPrefab = soundPlayerPrefab;
        }
    }
}
