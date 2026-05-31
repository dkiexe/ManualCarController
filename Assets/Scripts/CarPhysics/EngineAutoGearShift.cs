using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EngineScript))]
public class EngineAutoGearShift : MonoBehaviour
{
    [Header("Auto shiftdown Settings")]
    [SerializeField] private int minBluelineRPM;
    [SerializeField] private int maxBluelineRPM;
    [SerializeField] private float AutoShiftUpDelay = 2f;

    [Header("Auto shiftup Settings")]
    [SerializeField] private int minRedlineRPM;
    [SerializeField] private int maxRedlineRPM;
    [SerializeField] private float AutoShiftDownDelay = 0f;

    private EngineScript _engine;
    private GearSystemScript _gearBox;
    private PedalScript _clutchPedal;
    private Coroutine ShiftCorutine;

    private bool IsInNeutral => _gearBox.currentGearIndex == 0;
    private bool IsInFirst => _gearBox.currentGearIndex == 1;
    private bool IsRPMInRangeUp => _engine.EngineRPM >= minRedlineRPM && _engine.EngineRPM <= maxRedlineRPM;
    private bool IsRPMInRangeDown => _engine.EngineRPM >= minBluelineRPM && _engine.EngineRPM <= maxBluelineRPM;

    private void Start()
    {
        _engine = GetComponent<EngineScript>();
        _gearBox = _engine.GearBox;
        _clutchPedal = _engine.ClutchPedal;
    }

    private void Update()
    {
        if (_clutchPedal.PedalPressure > 0) return;

        if (!IsInNeutral && IsRPMInRangeUp)
        {
            if (ShiftCorutine == null) ShiftCorutine = StartCoroutine(AutoShiftUp());
        }
        else if (!IsInNeutral && !IsInFirst && IsRPMInRangeDown)
        {
            if (ShiftCorutine == null) ShiftCorutine = StartCoroutine(AutoShiftDown());
        }
    }

    private IEnumerator AutoShiftUp()
    {
        float elapsed = 0;
        
        _clutchPedal.AssignPedalCooldown(AutoShiftUpDelay);

        while (elapsed < AutoShiftUpDelay)
        {
            yield return null;
            elapsed += Time.deltaTime;
        }
        
        _clutchPedal.ImitatePress();
        _gearBox.ShiftUP();
        ShiftCorutine = null;
    }

    private IEnumerator AutoShiftDown()
    {
        float elapsed = 0;

        _clutchPedal.AssignPedalCooldown(AutoShiftDownDelay);

        while (elapsed < AutoShiftDownDelay)
        {
            yield return null;
            elapsed += Time.deltaTime;
        }

        _clutchPedal.ImitatePress();
        _gearBox.ShiftDown();
        ShiftCorutine = null;
    }
}
