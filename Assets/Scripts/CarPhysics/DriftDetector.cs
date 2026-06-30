using UnityEngine;

[RequireComponent(typeof(HandBrakeScript))]
public class DriftDetector : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private TrailRenderer[] tireParticleSystems;
    [SerializeField] private AudioSource driftAudioSorce;
    [SerializeField] private AudioClip[] driftAudioClips;
    [SerializeField] private float driftThreshold = 0.4f;

    private bool isDrifting;
    private bool wasDrifting;
    private float maxLateralSlip;
    private HandBrakeScript hBrakeScript;
    private WheelCollider[] wheels;

    private void Start()
    {
        hBrakeScript = GetComponent<HandBrakeScript>();
        wheels = hBrakeScript.HandBrakeWheels;
    }

    private void Update()
    {
        float yawRateDeg = rb.angularVelocity.y * Mathf.Rad2Deg;

        foreach (TrailRenderer tireTrail in tireParticleSystems)
        {
            tireTrail.emitting = isDrifting;
        }
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

        if (isDrifting && !wasDrifting)
        {
            AudioClip randAudioClip = driftAudioClips[
                    (int)Random.Range(minInclusive : 0, maxInclusive: driftAudioClips.Length)
                ];
            driftAudioSorce.PlayOneShot(randAudioClip);
        }
        wasDrifting = isDrifting;
    }
}
