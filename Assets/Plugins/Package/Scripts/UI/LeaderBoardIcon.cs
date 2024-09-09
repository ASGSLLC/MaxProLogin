using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using maxprofitness.login;
using TMPro;

public class LeaderBoardIcon : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;

    public void Init(int rank, string name, int score)
    {
        rankText.text = rank.ToString();
        nameText.text = name;
        scoreText.text = score.ToString();
    }
}
