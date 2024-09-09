using UnityEngine;
using UnityEngine.UI;
using maxprofitness.login;

/// <summary>
/// This class is used to create custom hitbox for buttons that uses personalized shapes
/// </summary>
[RequireComponent(typeof(Image))]
public sealed class CustomHitButton : MonoBehaviour
{
    private const float MinimumThreshold = 0.5f;

    private void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = MinimumThreshold;
    }
}
