using UnityEngine;

[RequireComponent(typeof(WheelCollider))]
public class WheelScript : MonoBehaviour
{
    [Header("Vehicle Reference")]
    public MeshFilter WheelVisual;

    [Header("Debug")]
    public float currentWheelTorque;
    public float currentRPM;
    public bool isSpinning;

    internal WheelCollider wheelCollider;

    private void Awake()
    {
        wheelCollider = GetComponent<WheelCollider>();
    }

    private void FixedUpdate()
    {
        currentWheelTorque = Mathf.Abs(wheelCollider.motorTorque);
        currentRPM = wheelCollider.rpm;
        isSpinning = Mathf.Abs(currentRPM) > 0;
    }

    private void Update()
    {
        if (isSpinning) WheelSpin();
    }

    private void WheelSpin()
    {
        float rotationThisFrame = (int)(currentRPM * 360 ) / 60 * Time.deltaTime;
        WheelVisual.transform.Rotate(Vector3.right * rotationThisFrame);
    }
}