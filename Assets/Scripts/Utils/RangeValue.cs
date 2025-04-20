using UnityEngine;

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
