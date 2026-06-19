using Assets.Scripts.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GaugeScript : MonoBehaviour
{
    [SerializeField] private float needleSpeed;
    [SerializeField] private GameObject gaugeNeedle;
    [SerializeField] SerializedDict<int, float> GaugePointToRotation;

    private Dictionary<int, float> GaugeToRotation;
    private float currentAngle;

    private void Awake()
    {
        GaugeToRotation = GaugePointToRotation.toDictionary();
        currentAngle = gaugeNeedle.transform.localEulerAngles.z;
    }

    protected void MoveNeedle(float value)
    {
        int minClamp = GaugeToRotation.Keys.First();
        int maxClamp = GaugeToRotation.Keys.Last();

        foreach (int gaugePoint in GaugeToRotation.Keys)
        {
            if (value >= gaugePoint) minClamp = gaugePoint;
            else
            {
                maxClamp = gaugePoint;
                break;
            }
        }

        float angleMulti = Mathf.InverseLerp(minClamp, maxClamp, value);

        float angle = Mathf.Lerp(
            GaugeToRotation[minClamp],
            GaugeToRotation[maxClamp],
            angleMulti
        );

        UpdateNeedle(angle);
    }

    protected void UpdateNeedle(float targetAngle)
    {
        currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * needleSpeed);

        gaugeNeedle.transform.localEulerAngles = new Vector3(0, 0, currentAngle);
    }
}