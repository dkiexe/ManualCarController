// intChannel.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Gauges/Int Channel")]
public class IntChannel : ScriptableObject
{
    public int Value { get; private set; }
    public int maxVal { get; private set; }
    public int minVal { get; private set; }
    public void InitializeChannel(int Value, int maxVal, int minVal)
    {
        this.Value = Value;
        this.maxVal = maxVal;
        this.minVal = minVal;
    }

    public void SetVal(int newVal) => Value = newVal;
}