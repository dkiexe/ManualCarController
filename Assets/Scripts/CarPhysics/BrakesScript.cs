using UnityEngine;

public class BrakesScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PedalScript BrakePedal;
    [SerializeField] internal WheelScript[] BrakeWheels;
    [SerializeField] private float BreakForceScalar = 1f;
    
    
    private float targetDeceleration = 9.81f; // ~1g

    private void FixedUpdate()
    {
        float NeededBrakeForce_t = CalculateBrakeForce();
        float BrakeForce_t = GetBrakeForceFromBrakePedal(NeededBrakeForce_t);
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
        foreach (WheelScript wheel in BrakeWheels)
        {
            wheel.wheelCollider.brakeTorque = BrakeForce_t;
        }
    }
}
