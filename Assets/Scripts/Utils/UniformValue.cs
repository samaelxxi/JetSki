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
        return Value.WithVariation(Variation);
    }
}
