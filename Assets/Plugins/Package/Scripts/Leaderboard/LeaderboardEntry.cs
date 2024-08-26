//using MaxProFitness.App.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace maxprofitness.login
{
    public class LeaderboardEntry : MonoBehaviour
    {
        public TextMeshProUGUI entryRankTMP;
        public TextMeshProUGUI entryNameTMP;
        public TextMeshProUGUI entryScoreTMP;
        public TextMeshProUGUI entryDifficultyTMP;
        public Image entryDifficultyBackground;

        public string entryRank;
        public string entryName;
        public int entryScore;
        public int entryGameID;

        public Difficulty difficulty;

        public enum Difficulty
        {
            beginner,
            regular,
            veteran,
            expert,
            pro
        }

        private LeaderboardCanvasController leaderboardController;

        private void Awake()
        {
            leaderboardController = GetComponentInParent<LeaderboardCanvasController>();
            SetEntry();
        }

        public void SetEntry()
        {
            entryRankTMP.text = entryRank;
            entryNameTMP.text = entryName;
            entryScoreTMP.text = entryScore.ToString();

            switch (difficulty)
            {

                case Difficulty.beginner:
                    entryDifficultyTMP.text = "Beginner";
                    entryDifficultyBackground.sprite = leaderboardController.difficultyBackgrounds[0];
                    break;

                case Difficulty.regular:
                    entryDifficultyTMP.text = "Regular";
                    entryDifficultyBackground.sprite = leaderboardController.difficultyBackgrounds[1];
                    break;

                case Difficulty.veteran:
                    entryDifficultyTMP.text = "Veteran";
                    entryDifficultyBackground.sprite = leaderboardController.difficultyBackgrounds[2];
                    break;

                case Difficulty.expert:
                    entryDifficultyTMP.text = "Expert";
                    entryDifficultyBackground.sprite = leaderboardController.difficultyBackgrounds[3];
                    break;

                case Difficulty.pro:
                    entryDifficultyTMP.text = "Pro";
                    entryDifficultyBackground.sprite = leaderboardController.difficultyBackgrounds[4];
                    break;

            }
        }

    }
}



