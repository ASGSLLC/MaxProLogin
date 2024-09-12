//using App.Scripts.Application.ExerciseVideoSystem;
using TMPro;
using UnityEngine;
using maxprofitness.login;

public class ScrollableExercise : MonoBehaviour
{
    [SerializeField] private TMP_Text _exerciseName;

    public MinigameExerciseData ExerciseData { get; private set; }

    public void Initialize(MinigameExerciseData exerciseData)
    {
        ExerciseData = exerciseData;

        _exerciseName.SetText(ExerciseData.ExerciseName);
    }
}
