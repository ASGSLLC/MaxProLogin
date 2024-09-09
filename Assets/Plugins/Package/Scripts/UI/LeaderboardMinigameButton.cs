using MaxProFitness;
//using MaxProFitness.App.TrainingRoutines;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using maxprofitness.login;

public class LeaderboardMinigameButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [FormerlySerializedAs("minigameType")] [FormerlySerializedAs("_minigame")] [SerializeField] private MinigameType _minigameType;
    [SerializeField] private MinigameMode _mode;

    [SerializeField] private TMP_Text _minigameText;
    [SerializeField] private Image _minigameImage;

    public Button Button => _button;
    public MinigameType MinigameType => _minigameType;
    public MinigameMode Mode => _mode;

    public void SetColors(Color newColor)
    {
        _minigameImage.color = newColor;
        _minigameText.color = newColor;
    }
}
