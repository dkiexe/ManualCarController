using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumeControl : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Volume volume;
    [SerializeField] private EngineScript engine;

    [Header("ChromaticAberration Settings")]
    [SerializeField] private float minCA = 0f;
    [SerializeField] private float maxCA = 1f;

    [Header("Vignette Settings")]
    [SerializeField] private float minVignette = 0.25f;
    [SerializeField] private float maxVignette = 0.55f;

    [Header("MotionBlur Settings")]
    [SerializeField] private float minMB = 0f;
    [SerializeField] private float maxMB = 1f;

    [Header("DepthOfField Settings")]
    [SerializeField] private float minDof = 115f;
    [SerializeField] private float maxDof = 0;

    [Header("Bloom Settings")]
    [SerializeField] private float minBloom = 0.8f;
    [SerializeField] private float maxBloom = 2.5f;

    void Update()
    {
        VolumeProfile vP = volume.profile;

        float speed_t = Mathf.Clamp01((float)(rb.linearVelocity.magnitude * 3.6 / engine.MaxSpeedKMH));

        if (vP.TryGet(out ChromaticAberration ca))
        {
            ca.intensity.value = Mathf.SmoothStep(minCA, maxCA, Mathf.InverseLerp(0.3f, 1f, speed_t));
        }

        if (vP.TryGet(out Vignette Vi))
        {
            Vi.intensity.value = Mathf.Lerp(minVignette, maxVignette, speed_t);
        }

        if (vP.TryGet(out MotionBlur Mb))
        {
            Mb.intensity.value = Mathf.Lerp(minMB, maxMB, Mathf.SmoothStep(0f, 1f, speed_t));
        }

        if (vP.TryGet(out DepthOfField dof))
        {
            dof.focalLength.value = Mathf.Lerp(minDof, maxDof, speed_t);
        }

        if (vP.TryGet(out Bloom b))
        {
            b.intensity.value = Mathf.Lerp(minBloom, maxBloom, speed_t);
        }

    }
}
