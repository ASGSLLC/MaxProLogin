//using PlayFab.ClientModels;
using System.Collections.Generic;
using _Project.App.Scripts.UI.Leaderboard;
using App.Scripts.UI.Leaderboard;
using MaxProFitness.SharedSound;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MaxProFitness.App.UI
{
    public class LeaderboardCanvasController : CanvasControllerBase
    {
        #region VARIABLES


        [SerializeField] private Button _closeButton;
        [SerializeField] private LeaderboardView leaderboardView;

        public Sprite[] difficultyBackgrounds;



        #endregion


        #region MONOBEHAVIOURS


        //------------------//
        private void Start()
        //------------------//
        {
            leaderboardView = FindObjectOfType<LeaderboardView>();

        } // END Start


        #endregion


        #region INITIALIZE


        //-------------------------------//
        public override void Initialize()
        //-------------------------------//
        {
            base.Initialize();

            Setup();

        } // END Initialize
        
        
        #endregion


        #region SETUP


        //-----------------//
        private void Setup()
        //-----------------//
        {
            _closeButton.onClick.AddListener(HandleBackButtonClicked);

        } // END Setup
        
        
        #endregion


        #region HANDLE BACK BUTTON CLICKED


        //----------------------------------//
        public void HandleBackButtonClicked()
        //---------------------------------//
        {
            SoundManager.PlaySound(SharedGameSound.APP_UI_BUTTON_CLICK);
            leaderboardView.TurnOffLeaderboardCanvas();

        } // END HandleBackButtonClicked
        
        
        #endregion

    }
}
