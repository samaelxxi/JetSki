using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "JetSkiStats", menuName = "ScriptableObjects/JetSkiStats", order = 1)]
public class JetSkiStats : ScriptableObject
{
    [field: Header("Forward movement"),
            SerializeField] public float MaxSpeed { get; private set; } = 10;
    [field: SerializeField] public float MinSpeed { get; private set; } = 5;
    [field: SerializeField] public float Acceleration { get; private set; } = 5f;
    [field: SerializeField] public float Deceleration { get; private set; } = 5f;

    [field: Header("Turning"),
            SerializeField] public float TurnTorque { get; private set; } = 50f;
    [field: SerializeField] public float MaxTurnAngle { get; private set; } = 45f;
    
    [field: Header("Lean"),
            SerializeField] public float MaxLeanAngle { get; private set; } = 30f;
    [field: SerializeField] public float LeanSpeed  { get; private set; } = 20f;
}
