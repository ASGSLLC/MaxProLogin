using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using MaxProFitness.UI;

namespace maxprofitness.login
{
    public class IntroAnimHelper : MonoBehaviour
    {
        #region VARIABLES

#if MAXPRO_LOGIN
    [SerializeField] private IntroCountdownController introController;
    [SerializeField] private HUD hud;
#endif

        #endregion


        #region MONOBEHAVIOURS


        //-----------------//
        private void Start()
        //----------------//
        {
            Init();

        } // END Start


        #endregion


        #region INIT


        //----------------//
        public void Init()
        //-----------------//
        {
#if MAXPRO_LOGIN
        if(hud == null)
        {
            hud = FindObjectOfType<HUD>();
        }
        hud.gameObject.SetActive(false);
#endif
        } // END Init


        #endregion


        #region  TURN ON/OFF INTRO


        //-----------------------//
        public void TurnOnCountdownIntro()
        //----------------------//
        {
#if MAXPRO_LOGIN
        StartCoroutine(introController.IPlayIntroCountdownAnimation(4));
        introController.GetComponent<CanvasGroup>().alpha = 1f;
        introController.GetComponent<Animator>().enabled = true;
        
        if(hud == null)
        {
            hud = FindObjectOfType<HUD>();
        }
        if(hud != null)
        {
            hud.gameObject.SetActive(true);
        }
#endif
        } // END TurnOnIntro



        #endregion


        //-----------------------//
        public void TurnOffCountdownIntro()
        //----------------------//
        {
#if !MAXPRO_LOGIN
        introController.GetComponent<CanvasGroup>().alpha = 0f;
        introController.GetComponent<Animator>().enabled = false;
#endif
        }


    } // END IntroAnimHelper.cs
}