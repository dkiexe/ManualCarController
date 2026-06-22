using System.Collections.Generic;
using UnityEngine;

public class BrakesScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] internal PedalScript BrakePedal;
    [SerializeField] internal WheelScript[] BrakeWheels;
    [SerializeField] private float BreakForceScalar = 1f;

    private bool SupressBrakes;
    private float targetDeceleration = 9.81f; // ~1g

    public float BrakeForce_t { get; private set; }

    private static List<BrakesScript> instances = new List<BrakesScript>();

    private void OnEnable()
    {
        instances.Add(this);
    }

    private void FixedUpdate()
    {
        float NeededBrakeForce_t = CalculateBrakeForce();
        
        BrakeForce_t = GetBrakeForceFromBrakePedal(NeededBrakeForce_t); // need domain reload for it not to leek.
        
        TransferBrakeToWheels(BrakeForce_t);
    }
    private float CalculateBrakeForce()
    {
        return (rb.mass * targetDeceleration) / BrakeWheels.Length;
    }

    private float GetBrakeForceFromBrakePedal(float maxBrakeForce_t)
    {
        return maxBrakeForce_t * BrakePedal.PedalPressure * BreakForceScalar;
    }

    private void TransferBrakeToWheels(float BrakeForce_t)
    {
        foreach (var BrakeScript in instances)
        {
            if (BrakeScript.BrakeForce_t > BrakeForce_t) return;
        }

        foreach (WheelScript wheel in BrakeWheels)
        {
            if (SupressBrakes)
            {
                wheel.wheelCollider.brakeTorque = 0;
            }
            else
            {
                wheel.wheelCollider.brakeTorque = BrakeForce_t;
            }
        }
    }

    public void ToggleBrakeSuppression() => SupressBrakes = !SupressBrakes;
}
