using UnityEngine;
using UnityEngine.UI;
using maxprofitness.login;

public class MultiImageTargetGraphics : MonoBehaviour
{
    [SerializeField] private Graphic[] targetGraphics;

    public Graphic[] GetTargetGraphics => targetGraphics;
}

