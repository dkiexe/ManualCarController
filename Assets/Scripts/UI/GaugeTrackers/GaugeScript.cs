using UnityEngine;

public class GaugeScript : MonoBehaviour
{
    [SerializeField] private FloatChannel channel;
    [SerializeField] private GameObject gaugeNeedle;
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;

    private void FixedUpdate()
    {
        float t = Mathf.InverseLerp(channel.minVal, channel.maxVal, channel.Value);
        float angle = Mathf.Lerp(minAngle, maxAngle, t);
        gaugeNeedle.transform.localEulerAngles = new Vector3(0, 0, angle);
    }
}