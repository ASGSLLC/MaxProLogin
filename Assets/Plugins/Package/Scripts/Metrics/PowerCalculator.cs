using System.Collections.Generic;
using UnityEngine;

public class PowerCalculator : MonoBehaviour
{
    private Dictionary<int, float> _knobsWeight;

    private float _workPeak = 0;
    private float _totalWork = 0;
    private float _averageWork = 0;

    private float _timeBetweenInputs = 0.04f;

    private List<float> _distances = new List<float>();
    private List<float> _velocities = new List<float>();
    private List<float> _averageWorks = new List<float>();

    public const float GravityAcceleration = 9.8f;

    public List<float> AverageWorks => _averageWorks;

    public int AverageWork => (int)_averageWork;
    public int WorkPeak => (int)_workPeak;
    public int TotalWork => (int)_totalWork;

    //    CALCULATION FORMULAS:
    //Work = mass x Gravity x distance, where Gravity = 9.8
    //Power = Work /Time
    //Calories burned = (4w + 0.35t) /4.2
    //velocity = position variation / time variation
    //medium acceleration = velocity variation / time variation
    //force = mass x acceleration(9.8m/s)

    public void Initialize()
    {
        LoadWeights();
    }

    public void GenerateVelocity(float distance1, float distance2)
    {
        float distanceVariation = (distance2) - (distance1);
        float velocity = distanceVariation / _timeBetweenInputs;
        _distances.Add(distance2);
        _velocities.Add(velocity);
        GetAverageWork(1);
    }

    public float GetCurrentVelocity()
    {
        if (_velocities.Count < 1)
        {
            return 0;
        }
        return _velocities[_velocities.Count - 1];
    }

    public float GetAverageVelocity()
    {
        float averageVelocity = 0;

        if (_distances.Count > 0)
        {
            averageVelocity = (_distances[_distances.Count - 1] - _distances[0]) / (_timeBetweenInputs * _distances.Count);
        }
        return averageVelocity;
    }

    public float GetPower(float weight)
    {
        float timeVariation = _timeBetweenInputs;
        float power = GetWork(weight) / timeVariation;
        return power; // Return the result in Watts
    }

    public float GetPower(float weight, float timeVariation, float initialDistance, float finalDistance)
    {
        float power = GetWork(weight, initialDistance, finalDistance) / timeVariation;
        return power; // Return the result in Watts
    }

    public float GetActualAcceleration()
    {
        if (_velocities.Count < 1)
        {
            return 0;
        }
        return GenerateAcceleration(_velocities[_velocities.Count - 1], _timeBetweenInputs);
    }

    public float GetAverageAcceleration()
    {
        float averageAcceleration = 0;

        if (_velocities.Count > 0)
        {
            averageAcceleration = (_velocities[_velocities.Count - 1] - _velocities[0]) / (_timeBetweenInputs * _velocities.Count);
        }

        if (averageAcceleration < 0)
        {
            averageAcceleration *= -1;
        }

        return averageAcceleration;
    }

    public float GetAveragePower(float weight)
    {
        // Force in Newtons, distance in Meters and Time in seconds
        float timeVariation = _velocities.Count * _timeBetweenInputs;
        float power = 0;
        power = GetAverageWork((int)weight) / timeVariation;

        // Return the result in Watts
        return power;
    }

    public float GetAverageWork(float currentResistance)
    {
        if (_distances.Count < 1)
        {
            return 0;
        }

        float resistance = (int)(currentResistance / 10);
        resistance = Mathf.Clamp(resistance, 1, _knobsWeight.Count - 1);

        resistance = _knobsWeight[(int)resistance];

        float work = resistance * GravityAcceleration * (_distances[_distances.Count - 1]);
        if (work > 0)
        {
            _averageWork += work / 10;
            _averageWorks.Add(work);
            return _averageWork;
        }

        return _averageWork;
    }

    public float GetWork(float currentResistance)
    {
        if (_distances.Count < 2)
        {
            return 0;
        }

        float resistance = (int)(currentResistance / 10);
        resistance = Mathf.Clamp(resistance, 1, _knobsWeight.Count - 1);

        resistance = _knobsWeight[(int)resistance];

        float work = resistance * GravityAcceleration * (_distances[_distances.Count - 1] - _distances[_distances.Count - 2]);

        return work;
    }

    public float GetWork(float currentResistance, float initialDistance, float finalDistance)
    {
        if (_distances.Count < 2)
        {
            return 0;
        }

        float resistance = (int)(currentResistance / 10);
        resistance = Mathf.Clamp(resistance, 1, _knobsWeight.Count - 1);

        resistance = _knobsWeight[(int)resistance];

        float work = resistance * GravityAcceleration * finalDistance - initialDistance;
        return work;
    }

    public void UpdateTotalWorkAndWorkPeak(float currentResistance)
    {
        float work = GetWork(currentResistance);
        if (work > 0)
        {
            if (work > _workPeak)
            {
                _workPeak = work;
            }
            _totalWork += work;
        }
    }

    public void ClearData()
    {
        _distances.Clear();
        _velocities.Clear();
    }

    private float GenerateAcceleration(float velocityVariation, float timeVariation)
    {
        return velocityVariation / timeVariation;
    }

    private void LoadWeights()
    {
        _knobsWeight = new Dictionary<int, float>()
            {
                {1, 3f},
                {2, 3.6f},
                {3, 4.1f},
                {4, 4.5f},
                {5, 5f},
                {6, 5.5f},
                {7, 6.4f},
                {8, 7.7f},
                {9, 9.1f},
                {10, 10f},
                {11, 12.7f},
                {12, 15f},
                {13, 17.3f},
                {14, 20f},
                {15, 23.6f},
                {16, 27.3f},
                {17, 30.9f},
                {18, 35.5f},
                {19, 40f},
                {20, 45.5f},
                {21, 50.9f},
                {22, 56.4f},
                {23, 62.3f},
                {24, 67.3f},
                {25, 71.8f},
                {26, 75f},
                {27, 80f},
            };
    }

}
