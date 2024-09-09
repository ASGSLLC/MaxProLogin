using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using maxprofitness.login;

public class LoginMusicManager : MonoBehaviour
{
    public AudioClip[] audioClip;

    private AudioSource audioSource;
    private int previousClip = 0;
    private bool playMusic = false;


    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        StartPlayMusic();
    }

    void Update()
    {
        if (playMusic == true)
        {
            if (audioSource.isPlaying == false)
            {
                PlayRandom();
            }
        }
    }

    public void StartPlayMusic()
    {
        playMusic = true;
        Play(0);
    }


    public void StopPlayMusic()
    {
        playMusic = false;
        audioSource.Stop();
    }


    public void Play(int i)
    {
        audioSource.clip = audioClip[i];
        audioSource.Play();
    }

    private void PlayRandom()
    {
        int _ran = Random.Range(1, audioClip.Length);

        if (_ran != previousClip)
        {
            previousClip = _ran;
            //Debug.Log("LoginMusicManager.cs // PlayRandom // Playing audio clip " + _ran);
            Play(_ran);
        }
        else if (_ran == previousClip)
        {
            PlayRandom();
        }
    }
}
