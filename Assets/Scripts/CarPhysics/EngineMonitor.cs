using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EngineMonitor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI RPM_TEXT;
    [SerializeField] private Image RPMGaugeImage;

    [Header("Shift Settings")]
    [SerializeField] private float GlobalShutOffDelay;
    [SerializeField] private float ShutOffRedlineDelay;
    [SerializeField] private float ShutOffBluelineDelay;
    [SerializeField] private Color shiftUpColor = Color.red;
    [SerializeField] private Color shiftDownColor = Color.blue;
    [SerializeField] private string shiftUpText = "Shift UP !";
    [SerializeField] private string shiftDownText = "Shift down...";
    [SerializeField] private float shiftDownTextDelay = 2f;
    [SerializeField] private float RPM_MinLimit;
    [SerializeField] private int RPMSearchLimit;
    [SerializeField] private int RPMSearchJump;

    private EngineScript engine;
    private PedalScript clutchPedal;

    private float bestShiftRPM;
    private Coroutine shutOffCorutine = null;
    private Coroutine shiftDownCorutine = null;

    private void Awake()
    {
        engine = GetComponent<EngineScript>();
        clutchPedal = engine.ClutchPedal;
    }
    private void OnEnable()
    {
        clutchPedal.playerPress.performed += HandleGearShift;
        clutchPedal.OnImitatedPress += HandleGearShift;
    }

    private void OnDisable()
    {
        clutchPedal.playerPress.performed -= HandleGearShift;
        clutchPedal.OnImitatedPress -= HandleGearShift;
    }

    private void Start()
    {
        bestShiftRPM = engine.maxRPM;
        if (RPMSearchLimit == default) RPMSearchLimit = (int)engine.maxRPM;
    }

    private void Update()
    {
        if (!engine.ignition.isIgnitionOn)
        {
            ClearMonitorText();
            return;
        }

        if (engine.GearBox.InReverse)
        {
            ClearMonitorText();
            return;
        }

        if (CheckForShiftUP())
        {
            UpdateMonitorText(shiftUpColor, shiftUpText);
            if (shutOffCorutine == null)
            {
                shutOffCorutine = StartCoroutine
                (
                    AttemptShutOff
                        (
                            CheckForShiftUP,
                            ShutOffRedlineDelay,
                            shiftUpColor
                        )
                );
            }
        }
        else if (CheckForShiftDown())
        {
            if (shiftDownCorutine == null)
            {
                shiftDownCorutine = StartCoroutine(DelayShiftDown());
            }
        }
        else
        {
            ClearMonitorText();
        }
    }

    private void HandleGearShift(InputAction.CallbackContext context)
    {
        StartCoroutine(EvaluateNewBoostZone());
    }

    private IEnumerator DelayShiftDown()
    {
        yield return new WaitForSeconds(shiftDownTextDelay);
        
        if (CheckForShiftDown() && shutOffCorutine == null)
        {
            shutOffCorutine = StartCoroutine
            (
                AttemptShutOff
                    (
                        CheckForShiftDown,
                        ShutOffBluelineDelay,
                        shiftDownColor
                    )
            );
            UpdateMonitorText(shiftDownColor, shiftDownText);
        }
        shiftDownCorutine = null;
    }

    private IEnumerator AttemptShutOff(
        Func<bool> ShutOffCondition, 
        float ShutOffDelay,
        Color WarningColor
        )
    {
        yield return new WaitForSeconds(GlobalShutOffDelay);

        float elapsed = 0;
        while (ShutOffCondition())
        {
            if (elapsed >= ShutOffDelay)
            {
                engine.ignition.StopIgniton();
                break;
            }

            elapsed += Time.deltaTime;
            float t = Mathf.Sin(Time.time * 20) * 0.5f + 0.5f;
            RPMGaugeImage.color = new Color(WarningColor.r, WarningColor.g, WarningColor.b, t);
            // blink visual
            yield return null;
        }
        // cleanup
        RPMGaugeImage.color = Color.white;
        shutOffCorutine = null;

    }

    private IEnumerator EvaluateNewBoostZone()
    {
        while (clutchPedal.PedalPressure > 0)
        {
            yield return null;
        }

        if ( // ignore neutral shifts or final gear.
            engine.GearBox.InNeutral ||
            engine.GearBox.InLast
            ) 
        {
            yield break;
        }

        int currentGearIndex = engine.GearBox.currentGearIndex;
        float engineRPM_t = engine.EngineRPM;
        float engineRPM_rolling = engineRPM_t;
        float maxEngineRPM = engine.maxRPM;
        float maxEngineTorque = engine.MaxTorque;

        float currentGearRatio = engine.GearBox.currentGearRatio;
        float nextGearRatio = engine.GearBox.GetGearRatio(currentGearIndex + 1);
        
        float bestShiftRPM = RPMSearchLimit;

        for (float rpm = engine.minRPM;
             rpm <= RPMSearchLimit;
             rpm += RPMSearchJump)
        {
            float accelCurrent =
                EvaluateAcceleration(rpm, currentGearRatio);

            float rpmAfterShift =
                rpm * nextGearRatio / currentGearRatio;

            float accelNext =
                EvaluateAcceleration(rpmAfterShift, nextGearRatio);

            if (accelNext > accelCurrent)
            {
                bestShiftRPM = rpm;
                break;
            }
        }
        this.bestShiftRPM = bestShiftRPM;
    }

    private float EvaluateAcceleration(float rpm, float gearRatio)
    {
        // Engine torque at this RPM
        float engineTorque =
            engine.MaxTorque *
            engine.RPM_TO_MaxTorqueGraph.Evaluate(rpm / engine.maxRPM);

        // Torque at the wheels
        float wheelTorque =
            engineTorque *
            gearRatio *
            engine.GearBox.finalDrive;

        // Force at the contact patch
        float wheelForce =
            wheelTorque / engine.ForceWheels[0].wheelCollider.radius;

        // F = ma
        return wheelForce / engine.rb.mass;
    }

    private void UpdateMonitorText(Color color, string text)
    {
        RPM_TEXT.color = color;
        RPM_TEXT.text = text;
        RPM_TEXT.alpha = 1;
    }

    private void ClearMonitorText()
    {
        RPM_TEXT.alpha = 0;
        
        if (shiftDownCorutine != null)
        {
            StopCoroutine(shiftDownCorutine);
            shiftDownCorutine = null;
        }
    }

    private bool CheckForShiftDown()
    {
        return (engine.EngineRPM <= RPM_MinLimit) &&
            !engine.GearBox.InFirst &&
            !engine.GearBox.InNeutral;
    }

    private bool CheckForShiftUP()
    {
        return engine.EngineRPM >= bestShiftRPM;
    }
}
