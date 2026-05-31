using UnityEngine;
using UnityEngine.InputSystem;

public class GearSystemScript : MonoBehaviour
{
    [Header("Gear Settings")]
    [SerializeField] private float[] gearRatios;
    [SerializeField] private float finalDriveRatio;

    [Header("Variables")]
    [SerializeField] private int gearIndex = 0;

    [Header("Player Input")]
    [SerializeField] private InputAction shiftGearInput; // Note. Player always shifts up.
    [SerializeField] private InputAction shiftToNeutralInput;

    // Class Properties
    internal float currentGearRatio => gearRatios[gearIndex];
    internal float currentGearIndex => gearIndex;
    internal bool InNeutral => gearIndex == 0;

    private void Update()
    {
        if (shiftGearInput.IsPressed())
        {
            ShiftUP();
        }
        else if (shiftToNeutralInput.IsPressed())
        {
            ShiftToNeutral();
        }
    }

    internal void ShiftToNeutral() => gearIndex = 0;

    internal void ShiftUP() => gearIndex = Mathf.Min(gearIndex + 1, gearRatios.Length);

    internal void ShiftDown() => gearIndex = Mathf.Max(gearIndex - 1, 0);
}
