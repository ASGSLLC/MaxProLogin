//using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace maxprofitness.shared
{
    public sealed class SoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        private AudioSourceProperties _audioSourceProperties;

        public AudioClip Clip => _audioSource.clip;
        public float Time => _audioSource.time;

        private void Start()
        {
            if (_audioSource == null)
            {
                _audioSource = new AudioSource();
            }
        }

        public void PlaySound(AudioSourceProperties audioSourceProperties)
        {
            SetAndPlayAudioSource(audioSourceProperties);
        }

        public void PlaySound(AudioSourceProperties audioSourceProperties, Vector3 position)
        {
            SetAndPlayAudioSource(audioSourceProperties);

            transform.position = position;
        }

        public void PlaySound(AudioSourceProperties audioSourceProperties, float soundDuration)
        {
            SetAndPlayAudioSource(audioSourceProperties, soundDuration);
        }

        public void StopPlaying()
        {
            _audioSource.Stop();

            if (_audioSourceProperties != null)
            {
                _audioSourceProperties.IsPlaying = false;
            }
        }

        public void PauseSound()
        {
            _audioSource.Pause();
        }

        public void ResumeSound()
        {
            _audioSource.UnPause();
        }

        public void SetPitch(float newPitch)
        {
            _audioSource.pitch = newPitch;
        }

        public void FadeVolume(float targetValue, float duration)
        {
            float currentTime = 0;
            float timeDelay = 0.1f;
            float targetVolume = Mathf.Clamp(targetValue, 0, 1);
            float initialVolume = _audioSource.volume;

            DoFade();
            async void DoFade()
            {
                float newVolume = Mathf.Lerp(initialVolume, targetVolume, currentTime / duration);
                _audioSource.volume = newVolume;
                
                //await UniTask.Delay(TimeSpan.FromSeconds(timeDelay), false, cancellationToken: this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();
                if (newVolume != targetValue && _audioSource != null)
                {
                    currentTime += timeDelay;
                    DoFade();
                }
            }
        }
        
        private void OnDisable()
        {
            if (_audioSourceProperties != null)
            {
                _audioSourceProperties.IsPlaying = false;
            }
        }

        private void SetAndPlayAudioSource(AudioSourceProperties audioSourceProperties, float duration = 0)
        {
            SetAudioSourceProperties(audioSourceProperties, duration);

            _audioSource.Play();

            if (!_audioSource.loop)
            {
                StartCoroutine(DeactivateSoundGameObject());
            }
        }

        private IEnumerator DeactivateSoundGameObject()
        {
            if (_audioSource != null)
            {
                if (_audioSource.clip != null)
                {
                    yield return new WaitForSeconds(_audioSource.clip.length);
                }
            }
            else
            { 
                yield return null;
            }

           // gameObject.SetActive(false);
            Destroy(gameObject);
        }

        private void SetAudioSourceProperties(AudioSourceProperties audioSourceProperties, float duration = 0)
        {
            _audioSourceProperties = audioSourceProperties;
            int randomIndex;

            if (duration > 0)
            {
                randomIndex = RandomizeMusicAccordingToDuration(audioSourceProperties, duration);
            }
            else
            {
                randomIndex= UnityEngine.Random.Range(0, audioSourceProperties.AudioClips.Length);
            }

            _audioSource.clip = audioSourceProperties.AudioClips[randomIndex];
            _audioSource.volume = audioSourceProperties.Volume;
            _audioSource.spatialBlend = audioSourceProperties.SpatialBlend;
            _audioSource.loop = audioSourceProperties.Loop;
            _audioSource.outputAudioMixerGroup = audioSourceProperties.AudioMixerGroup;

            if (audioSourceProperties.RandomizePitch)
            {
                _audioSource.pitch = UnityEngine.Random.Range(audioSourceProperties.MinPitch, audioSourceProperties.MaxPitch);
            }
            else
            {
                _audioSource.pitch = audioSourceProperties.Pitch;
            }

            if (audioSourceProperties.PersistentSound)
            {
                DontDestroyOnLoad(_audioSource.gameObject);
            }
        }

        private int RandomizeMusicAccordingToDuration(AudioSourceProperties audioSourceProperties, float duration)
        {
            List<int> availableIndexes = new List<int>();
            const double tolerance = 0.1f;

            for (int i = 0; i < audioSourceProperties.AudioClips.Length; i++)
            {
                if (Math.Abs(audioSourceProperties.AudioClips[i].length - duration) < tolerance)
                {
                    availableIndexes.Add(i);
                }
            }

            int randomIndex = UnityEngine.Random.Range(0, availableIndexes.Count);

            return availableIndexes[randomIndex];
        }
    }
}
