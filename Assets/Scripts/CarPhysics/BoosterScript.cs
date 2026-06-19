using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(EngineScript))]
public class BoosterScript : MonoBehaviour
{
    [SerializeField] private Image BoostZoneIndicator;
    [SerializeField] private float BoostZoneIndicatorMinVal;
    [SerializeField] private float BoostZoneIndicatorMaxVal;
    [SerializeField] private int RPMSearchLimit;
    [SerializeField] private int RPMSearchJump;

    private EngineScript engine;
    private PedalScript clutchPedal;

    private float bestShiftRPM = -1;

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
        if (RPMSearchLimit == default) RPMSearchLimit = (int)engine.maxRPM;
    }

    private void HandleGearShift(InputAction.CallbackContext context)
    {
        DisableBoostZone();
        
        if (engine.GearBox.InNeutral) // ignore neutral shifts
        {
            return;
        } 


        StartCoroutine(EvaluateNewBoostZone());
        DisplayNewBoostZone();
    }

    private void DisableBoostZone()
    {
        bestShiftRPM = 0;
        BoostZoneIndicator.transform.rotation = Quaternion.identity;
        BoostZoneIndicator.gameObject.SetActive(false);
    }

    private void DisplayNewBoostZone()
    {
        float t = Mathf.InverseLerp(engine.minRPM, engine.maxRPM, bestShiftRPM);
        float rotatioZ = Mathf.Lerp(BoostZoneIndicatorMaxVal, BoostZoneIndicatorMinVal, t);
        BoostZoneIndicator.transform.rotation = Quaternion.Euler(Vector3.back * rotatioZ);
        BoostZoneIndicator.gameObject.SetActive(true);
    }

    private IEnumerator EvaluateNewBoostZone()
    {
        // awaiting clutch pedal pressure loss.
        while (clutchPedal.PedalPressure > 0)
        {
            yield return null;
        }

        int currentGearIndex = engine.GearBox.currentGearIndex;
        float engineRPM_t = engine.EngineRPM;
        float engineRPM_rolling = engineRPM_t;
        float maxEngineRPM = engine.maxRPM;
        float maxEngineTorque = engine.MaxTorque;

        float currentGearRatio = engine.GearBox.currentGearRatio;
        float nextGearRatio = engine.GearBox.GetGearRatio(currentGearIndex + 1);

        float bestTorqueGeneration = 0;
        float bestShiftRPM = 0;

        while (engineRPM_rolling < RPMSearchLimit)
        {
            float wheelTorque_beforeShift = EvaluateTorqueWheels(engineRPM_rolling, maxEngineRPM, maxEngineTorque, currentGearRatio);

            float engineRPM_afterShift = engineRPM_rolling * (nextGearRatio / currentGearRatio);

            float wheelTorque_afterShift = EvaluateTorqueWheels(engineRPM_afterShift, maxEngineRPM, maxEngineTorque, nextGearRatio);

            if (wheelTorque_afterShift >= bestTorqueGeneration)
            {
                bestTorqueGeneration = wheelTorque_afterShift;
                bestShiftRPM = engineRPM_rolling;
            }
            engineRPM_rolling += RPMSearchJump;
        }
        
        Debug.Log(bestShiftRPM);
        this.bestShiftRPM = bestShiftRPM;
    }

    private float EvaluateTorqueWheels
        (
            float rpm, 
            float maxRpm,
            float maxEngineTorque,
            float GearRatio
        )
    {
        return maxEngineTorque * engine.RPM_TO_MaxTorqueGraph.Evaluate
        (
            rpm / maxRpm
        ) * GearRatio * engine.GearBox.finalDrive;
    }
}