using UnityEngine;

namespace maxprofitness.login
{
    /// <summary>
    /// 
    /// </summary>

    [CreateAssetMenu(menuName = "DifficultySelection/Difficulty Selection Data")]
    public class DifficultyDataSO : ScriptableObject
    {
        public string DifficultyName;
        public string FirstLineParameter;
        public string SecondLineParameter;
        public string KnobLevelSuggestion;
        public int requiredScore;
    }
}
