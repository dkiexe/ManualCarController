using UnityEngine;

[RequireComponent(typeof(HandBrakeScript))]
public class DriftDetector : MonoBehaviour
{
    [SerializeField] private float driftThreshold = 0.4f;

    private bool isDrifting;
    private float maxLateralSlip;
    private HandBrakeScript hBrakeScript;
    private WheelCollider[] wheels;

    private void Start()
    {
        hBrakeScript = GetComponent<HandBrakeScript>();
        wheels = hBrakeScript.HandBrakeWheels;
    }

    private void FixedUpdate()
    {
        maxLateralSlip = 0f;

        foreach (var wheel in wheels)
        {
            WheelHit hit;
            if (!wheel.GetGroundHit(out hit)) continue;

            float absLateral = Mathf.Abs(hit.sidewaysSlip);
            if (absLateral > maxLateralSlip)
                maxLateralSlip = absLateral;
        }

        isDrifting = maxLateralSlip > driftThreshold;

        //Debug.Log(isDrifting);
    }
}
