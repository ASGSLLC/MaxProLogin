//using App.Scripts.UI.DifficultySelection.ScriptableObjects;
using UnityEngine;

namespace maxprofitness.login
{
    /// <summary>
    /// 
    /// </summary>

    [CreateAssetMenu(menuName = "DifficultySelection/Difficulty Screen Data")]
    public class DifficultySelectionScreenDataSO : ScriptableObject
    {
        [Header("Work Summary Information")]
        public string FirstLineInformationText;
        public string SecondLineInformationText;
        public string KnobLevelSuggestionText;

        [Header("Difficulty Description")]
        [TextArea]
        public string DifficultyDescription;

        [Header("Difficulty Data Information")]
        public DifficultyDataSO FirstDifficultyData;
        public DifficultyDataSO SecondDifficultyData;
        public DifficultyDataSO ThirdDifficultyData;
        public DifficultyDataSO FourthDifficultyData;
        public DifficultyDataSO FifthDifficultyData;
    }
}
