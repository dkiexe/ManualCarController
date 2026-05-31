using System;
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

    [Header("Gear Tracking Data Channels")]
    [SerializeField] private IntChannel ChannelGearIndex;

    [Header("Player Input")]
    [SerializeField] private InputAction shiftToNeutralInput;

    // Class Properties
    internal float currentGearRatio => gearRatios[gearIndex];
    internal int currentGearIndex => gearIndex;
    internal float finalDrive => finalDriveRatio;
    internal bool InNeutral => gearIndex == 0;

    // Unity Methods
    private void OnEnable()
    {
        shiftToNeutralInput.Enable();

        ClutchPedal.playerPress.performed += ShiftUpInputHandler;
        shiftToNeutralInput.performed += ShiftNInputHandler;
    }

    private void Start()
    {
        ChannelGearIndex.InitializeChannel(gearIndex, gearRatios.Length - 1, 0);
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
        }
    }

    internal void ShiftToNeutral() 
    { 
        gearIndex = 0;
        UpdateGearDataChannel();
    }

    internal void ShiftUP() 
    {
        gearIndex = Mathf.Min(gearIndex + 1, gearRatios.Length - 1);
        UpdateGearDataChannel();
    }

    internal void ShiftDown()
    {
        gearIndex = Mathf.Max(gearIndex - 1, 0);
        UpdateGearDataChannel();
    }

    private void UpdateGearDataChannel() => ChannelGearIndex.SetVal(gearIndex);
}
