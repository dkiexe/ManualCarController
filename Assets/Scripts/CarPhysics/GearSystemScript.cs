using UnityEngine;

public class GearSystemScript : MonoBehaviour
{
    [Header("Gear Settings")]
    [SerializeField] private float[] gearRatios;
    [SerializeField] private float finalDriveRatio;

    [Header("Variables")]
    [SerializeField] private int gearIndex;

    // Class Properties
    internal float currentGearRatio => gearRatios[gearIndex];
    internal float currentGearIndex => gearIndex;
}
