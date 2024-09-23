//using MaxProFitness.SharedSound;
using System.Collections;
using UnityEngine;

namespace maxprofitness.shared
{
    public sealed class SelectedSoundPlayer : MonoBehaviour
    {
        [SerializeField] private bool _playOnStart;

        [SerializeField] private SharedGameSound _gameSound;

        [SerializeField] private float _delay;

        public void PlaySound()
        {
            StartCoroutine(DelayPlaySound());
        }

        private void Start()
        {
            if (_playOnStart)
            {
                PlaySound();
            }
        }

        private IEnumerator DelayPlaySound()
        {
            yield return new WaitForSeconds(_delay);

            SoundManager.PlaySound(_gameSound);
        }
    }
}