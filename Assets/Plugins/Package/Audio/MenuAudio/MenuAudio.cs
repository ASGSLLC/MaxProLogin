using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using maxprofitness.login;


public class MenuAudio : MonoBehaviour
{
    private AudioSource menuButtonSFX;


    private void Awake()
    {
        menuButtonSFX = GetComponent<AudioSource>();
    }


    public void PlayMenuSound(int _menuSound)
    {
        switch (_menuSound)
        {
            case 0:

                SoundManager.PlaySound(SharedGameSound.APP_UI_BUTTON_CLICK);
                break;

            case 1:
                if (menuButtonSFX == null)
                {
                    menuButtonSFX = GetComponent<AudioSource>();
                }
                menuButtonSFX.Play();
                break;

        }

    }
}
