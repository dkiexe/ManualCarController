// FloatChannel.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Gauges/Float Channel")]
public class FloatChannel : ScriptableObject
{
    public float Value { get; private set; }
    public float maxVal { get; private set; }
    public float minVal { get; private set; }
    public void InitializeChannel(float Value, float maxVal, float minVal)
    {
        this.Value = Value;
        this.maxVal = maxVal;
        this.minVal = minVal;
    }

    public void SetVal(float newVal) => Value = newVal;
}