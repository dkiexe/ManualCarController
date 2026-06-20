using UnityEngine;
using UnityEngine.InputSystem;

public class GearSystemScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PedalScript ClutchPedal;

    [Header("Gear Settings")]
    [SerializeField] private float[] gearRatios;
    [SerializeField] private float finalDriveRatio;

    [Header("Variables")]
    [SerializeField] private int gearIndex = 0;

    [Header("Player Input")]
    [SerializeField] private InputAction shiftToNeutralInput;

    // Class Properties
    internal float currentGearRatio => gearRatios[gearIndex];
    internal float finalDrive => finalDriveRatio;
    internal int GearCount => gearRatios.Length;
    internal int currentGearIndex => gearIndex;
    internal bool InNeutral => gearIndex == 0;
    internal bool InFirst => gearIndex == 1;

    internal bool InLast => gearIndex == GearCount - 1;


    // Unity Methods
    private void OnEnable()
    {
        shiftToNeutralInput.Enable();

        ClutchPedal.playerPress.performed += ShiftUpInputHandler;
        shiftToNeutralInput.performed += ShiftNInputHandler;
    }

    private void OnDisable()
    {
        shiftToNeutralInput.Disable();

        ClutchPedal.playerPress.performed -= ShiftUpInputHandler;
        shiftToNeutralInput.performed -= ShiftNInputHandler;
    }

    private void OnDestroy()
    {
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
            ClutchPedal.ImitatePress();
        }
    }

    internal void ShiftToNeutral() 
    { 
        gearIndex = 0;
    }

    internal void ShiftUP() 
    {
        gearIndex = Mathf.Min(gearIndex + 1, gearRatios.Length - 1);
    }

    internal void ShiftDown()
    {
        gearIndex = Mathf.Max(gearIndex - 1, 0);
    }

    public float GetGearRatio(int gearIndex) => gearRatios[gearIndex];
}
