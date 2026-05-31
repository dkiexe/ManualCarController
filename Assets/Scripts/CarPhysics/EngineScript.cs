using System;
using UnityEngine;

public class EngineScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] internal GearSystemScript GearBox;
    [SerializeField] private PedalScript GasPedal;
    [SerializeField] internal PedalScript ClutchPedal;
    [SerializeField] internal WheelScript[] ForceWheels;

    [Header("Engine Settings")]
    [SerializeField] private AnimationCurve RPM_TO_MaxTorqueGraph;
    [SerializeField] private float EngineHP;
    [SerializeField] private int MaxKMH;
    [SerializeField] private int MinRPM;
    [SerializeField] private int MaxRPM;
    [SerializeField] private float RPMResponseSpeed = 1f;
    [SerializeField] private float RPMDecaySpeed = 1f;

    [Header("Engine Tracking Data Channels")]
    [SerializeField] private FloatChannel ChannelEngineRPM;
    [SerializeField] private FloatChannel ChannelKMH;

    // Private Fields.
    private float MaxTorque;
    
    public float EngineRPM { get; private set; }

    private void Start()
    {
        ChannelKMH.InitializeChannel(0, MaxKMH, 0); // Note! this limit is visual only!
        ChannelEngineRPM.InitializeChannel(MinRPM, MaxRPM, MinRPM);
        MaxTorque = (EngineHP * 7127) / MaxRPM;
        EngineRPM = MinRPM;
    }

    private void FixedUpdate()
    {
        // ****GlobalPhysics****
        ChannelKMH.SetVal(CalculateKMH());

        // ****DrivePhysics****
        float EngineTorque_t = 0;

        if (ClutchPedal.PedalPressure != 1)
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
        ChannelEngineRPM.SetVal(EngineRPM);
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

        TargetEngineRPM_t = TargetEngineRPM_t * (1 - ClutchPedal.PedalPressure);

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
