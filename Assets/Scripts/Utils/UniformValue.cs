using UnityEngine;

[System.Serializable]
public struct UniformValue
{
    public float Value;
    public float Variation;

    public UniformValue(float value, float variation)
    {
        Value = value;
        Variation = variation;
    }

    public readonly float GetRandomValue()
    {
        return Value + Random.Range(-Variation, Variation);
    }
}


[System.Serializable]
public struct RangeValue
{
    public float Min;
    public float Max;

    public RangeValue(float min, float max)
    {
        Min = min;
        Max = max;
    }

    public readonly float GetRandomValue()
    {
        return Random.Range(Min, Max);
    }
}
