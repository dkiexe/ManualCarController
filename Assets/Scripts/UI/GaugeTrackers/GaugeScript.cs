using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utils;

public class GaugeScript : MonoBehaviour
{
    [SerializeField] private GameObject gaugeNeedle;
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;
    [SerializeField] SerializedDict<int, float> GaugePointToRotation;

    protected void UpdateNeedle(float minVal, float maxVal, float currVal)
    {
        float t = Mathf.InverseLerp(minVal, maxVal, currVal);
        float angle = Mathf.Lerp(minAngle, maxAngle, t);
        gaugeNeedle.transform.localEulerAngles = new Vector3(0, 0, angle);
    }
}