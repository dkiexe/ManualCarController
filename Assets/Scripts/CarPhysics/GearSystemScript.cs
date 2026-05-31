using System;
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

    // Unity Methods
    private void OnEnable()
    {
        shiftGearInput.Enable();
        shiftToNeutralInput.Enable();

        shiftGearInput.performed += ShiftUpInputHandler;
        shiftToNeutralInput.performed += ShiftNInputHandler;
    }
    
    private void OnDisable()
    {
        shiftGearInput.Disable();
        shiftToNeutralInput.Disable();

        shiftGearInput.performed -= ShiftUpInputHandler;
        shiftToNeutralInput.performed -= ShiftNInputHandler;
    }

    private void OnDestroy()
    {
        shiftGearInput.Dispose();
        shiftToNeutralInput.Dispose();
    }

    // Class Methods
    private void ShiftUpInputHandler(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ShiftUP();
        }
    }
    private void ShiftNInputHandler(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ShiftToNeutral();
        }
    }

    internal void ShiftToNeutral() => gearIndex = 0;

    internal void ShiftUP() => gearIndex = Mathf.Min(gearIndex + 1, gearRatios.Length - 1);

    internal void ShiftDown() => gearIndex = Mathf.Max(gearIndex - 1, 0);
}
