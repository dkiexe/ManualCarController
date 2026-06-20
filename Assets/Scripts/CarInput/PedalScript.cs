using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PedalScript : MonoBehaviour
{
    [Header("Pedal Settings")]
    [SerializeField] private IgnitionScript ignition;
    [SerializeField] private bool ignoreIgnition = false;
    [SerializeField] private float ConstMinPressure = 0f;
    [SerializeField] private float InputPressureRate = 0.1f;
    [SerializeField] private float MaxInputPedalPressure = 1f; // Note. maximum pressure a PLAYER can inflict on the pedal using input
    
    [SerializeField] private float PressureDampeningRate = 0.1f;
    [SerializeField] private float PressureDampeningInterval = 0.5f; // Note. In seconds


    [Header("Player Input")]
    [SerializeField] internal InputAction playerPress;

    // Class Private Fields
    private float DampenTimeElapsed = 0f;

    // Class Properties
    public float MaxPedalPressure { get; private set; }
    public float InputPedalPressure { get; private set; }
    public float PedalPressure => MaxPedalPressure * (InputPedalPressure + ConstMinPressure);

    internal event Action<InputAction.CallbackContext> OnImitatedPress;

    private void OnEnable()
    {
        playerPress.Enable();
    }

    protected virtual void Start()
    {
        MaxPedalPressure = Mathf.Clamp01(ConstMinPressure + MaxInputPedalPressure);
        if (MaxPedalPressure < 1f)
        {
            Debug.LogWarning("Incorrect Pressure Assignment! ( MaxPedalPressure = ConstMinPressure + MaxPlayerPedalPressure != 1 ), this can cause unexpected behaviour");
        }
    }

    protected virtual void Update()
    {
        bool ignitionModifier = ignoreIgnition ? true : ignition.isIgnitionOn;
        
        if (playerPress.IsPressed() && ignitionModifier)
        {
            ApplyPressure();
            if (DampenTimeElapsed != 0) DampenTimeElapsed = 0;
        }
        else
        {
            AttemptPressureDampening();
        }
    }

    private void OnDisable()
    {
        playerPress.Disable();
    }

    private void OnDestroy()
    {
        playerPress.Dispose();
    }

    private void AttemptPressureDampening()
    {
        if (DampenTimeElapsed >= PressureDampeningInterval)
        {
            DampenPressure();
            DampenTimeElapsed = 0;
        }
        else
        {
            DampenTimeElapsed += Time.deltaTime;
        }
    }

    public void ImitatePress()
    {
        ApplyPressure();
        if (DampenTimeElapsed != 0) DampenTimeElapsed = 0;
        OnImitatedPress?.Invoke(default);
    }

    private void ApplyPressure() => InputPedalPressure = Mathf.Min(
            InputPedalPressure + InputPressureRate * Time.deltaTime, 
            MaxInputPedalPressure
        );

    private void DampenPressure() => InputPedalPressure = Mathf.Max(
            InputPedalPressure - PressureDampeningRate, 
            0
        );
}
