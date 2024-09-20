using UnityEngine;
using UnityEngine.Serialization;

namespace maxprofitness.shared
{
    /// <summary>
    /// Used to store the exercise video data
    /// </summary>
    [CreateAssetMenu(fileName = "MinigameExercise", menuName = "Application/Minigames/MinigameExercise", order = 0)]
    public class MinigameExerciseData : ScriptableObject
    {
        public string ExerciseName;

        [TextArea(1, 200)]
        public string Description;
        [TextArea(1, 200)]
        public string VideoUrl;

        public ExercisePlacement Placement;
        public ExerciseBodyArea BodyArea;
        public ExerciseGoal Goal;
        public ExerciseAccessory Accessory;
    }
}
