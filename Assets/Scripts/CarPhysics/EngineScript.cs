using UnityEngine;

public class EngineScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] internal IgnitionScript ignition;
    [SerializeField] internal GearSystemScript GearBox;
    [SerializeField] private PedalScript GasPedal;
    [SerializeField] internal PedalScript ClutchPedal;
    [SerializeField] internal WheelScript[] ForceWheels;

    [Header("Engine Settings")]
    [SerializeField] internal AnimationCurve RPM_TO_MaxTorqueGraph;
    [SerializeField] private float EngineHP;
    [SerializeField] private int MaxKMH;
    [SerializeField] private int MinRPM;
    [SerializeField] private int MaxRPM;
    [SerializeField] private float RPMResponseSpeed = 1f;
    [SerializeField] private float RPMDecaySpeed = 1f;

    // Attributes
    public float KMH { get; private set;  }
    public float MaxSpeedKMH { get; private set; }
    public float EngineRPM { get; private set; }
    public float MaxTorque { get; private set; }

    public float minRPM => MinRPM;
    public float maxRPM => MaxRPM;

    private void Start()
    {
        MaxSpeedKMH = (float)((maxRPM * 2 * Mathf.PI * ForceWheels[0].wheelCollider.radius) / 
            (GearBox.GetGearRatio(GearBox.GearCount - 1) * GearBox.finalDrive * 60) * 3.6);

        MaxTorque = (EngineHP * 7127) / MaxRPM;

        if (ignition.isIgnitionOn) EngineRPM = MinRPM;
    }

    private void FixedUpdate()
    {
        // ****GlobalPhysics****
        KMH = CalculateKMH();

        if (!ignition.isIgnitionOn) return;

        // ****DrivePhysics****
        float EngineTorque_t = 0;

        if (GearBox.InFirst || (ClutchPedal.PedalPressure == 0 && !GearBox.InNeutral))
        {
            float LoadRPM = GetWheelLoadRPM();
            EngineRPM = ConvertLoadToEngineRPM(LoadRPM);
            float MaxTorque_t = ReadMaxTorqueForRPM(EngineRPM);
            EngineTorque_t = GetEngineTorqueFromThrottle(MaxTorque_t);
        }
        else // Disconnect the Motor from wheels and try to reduce RPM
        {
            EngineRPM = Mathf.Lerp(
                    EngineRPM,
                    MinRPM * (1 + GasPedal.PedalPressure),
                    Time.fixedDeltaTime * RPMDecaySpeed
                );
        }
        TransferTorqueToWheels(EngineTorque_t);
    }
    private float CalculateKMH()
    {
        return rb.linearVelocity.magnitude * 3.6f;
    }

    private float GetWheelLoadRPM()
    {
        float totalRPM = 0;
        float totalWheelsOnGround = ForceWheels.Length;

        foreach (WheelScript wheel in ForceWheels)
        {
            if (wheel.wheelCollider.GetGroundHit(out WheelHit _))
            {
                totalRPM += wheel.currentRPM;
            }
            else
            {
                totalWheelsOnGround--;
            }
        }
        if (totalRPM == 0 && totalWheelsOnGround == 0) return 0;
        return totalRPM / totalWheelsOnGround;
    }
    
    private float ConvertLoadToEngineRPM(float LoadRPM)
    {
        float TargetEngineRPM_t = LoadRPM * GearBox.currentGearRatio * GearBox.finalDrive;

        TargetEngineRPM_t = Mathf.Clamp(TargetEngineRPM_t, MinRPM, MaxRPM);

        return Mathf.Lerp(
            EngineRPM,
            TargetEngineRPM_t,
            Time.fixedDeltaTime * RPMResponseSpeed
        );
    }

    private float ReadMaxTorqueForRPM(float RPM_Level)
    {
        return MaxTorque * RPM_TO_MaxTorqueGraph.Evaluate(RPM_Level / MaxRPM);
    }

    private float GetEngineTorqueFromThrottle(float maxTorque_t)
    {
        return maxTorque_t * GasPedal.PedalPressure;
    }

    private void TransferTorqueToWheels(float engineTorque_t)
    {
        float wheelTorque = (engineTorque_t * GearBox.currentGearRatio * GearBox.finalDrive)
                            / ForceWheels.Length;

        foreach (WheelScript wheel in ForceWheels)
        {
            wheel.wheelCollider.motorTorque = wheelTorque;
        }
    }
}
