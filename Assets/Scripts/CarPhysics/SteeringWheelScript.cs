using UnityEngine;
using UnityEngine.InputSystem;

public class SteeringWheelScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private WheelScript[] RotationWheels;

    [Header("Settings")]
    [SerializeField] private float physicsRotationScalar;
    [SerializeField] private float visualRotationScalar;
    [SerializeField] private float rotationDampMulti;
    [SerializeField] private float maxRotationAngle;

    [Header("Steering Assist")]
    [SerializeField] private float yawAssistStrength = 500f;

    [Header("Player Input")]
    [SerializeField] private InputAction RotationDirInput;

    // Private Fields

    private void OnEnable()
    {
        RotationDirInput.Enable();
    }

    private void FixedUpdate()
    {
        if (RotationDirInput.IsPressed())
        {
            int RotDir = (int)RotationDirInput.ReadValue<float>();
            ApplyRotationPhysics(RotDir);
        }
        else
        {
            DampenRotationPhysics();
        }

        ApplyYawAssist();
    }

    private void ApplyYawAssist()
    {
        float steerNormalized = RotationWheels[0].wheelCollider.steerAngle / maxRotationAngle;
        float speed = rb.linearVelocity.magnitude;

        float yawTorque = steerNormalized * speed * yawAssistStrength * Time.fixedDeltaTime;
        rb.AddTorque(transform.up * yawTorque, ForceMode.Force);
    } 

    private void Update()
    {
        if (RotationDirInput.IsPressed())
        {
            int RotDir = (int)RotationDirInput.ReadValue<float>();
            ApplyRotationVisuals(RotDir);
        }
        else
        {
            DampenRotationVisuals();
        }
    }

    private void OnDisable()
    {
        RotationDirInput.Disable();
    }

    private void OnDestroy()
    {
        RotationDirInput.Dispose();
    }

    private void ApplyRotationPhysics(int RotDir)
    {
        for (int i = 0; i < RotationWheels.Length; i++)
        {
            WheelCollider wc = RotationWheels[i].wheelCollider;

            float currentAngle = wc.steerAngle;
            float rotationThisFrame = physicsRotationScalar * Time.fixedDeltaTime * RotDir;
            float nextAngle = currentAngle + rotationThisFrame;

            wc.steerAngle = Mathf.Clamp(nextAngle, -maxRotationAngle, maxRotationAngle);
        }
    }
    private void DampenRotationPhysics()
    {
        for (int i = 0; i < RotationWheels.Length; i++)
        {
            WheelCollider wc = RotationWheels[i].wheelCollider;

            if (Mathf.Abs(wc.steerAngle) < 0.01f)
            {
                wc.steerAngle = 0;
                continue;
            }

            float OppositeDir = Mathf.Sign(wc.steerAngle) * -1;
            float rotationThisFrame = physicsRotationScalar * rotationDampMulti * Time.fixedDeltaTime * OppositeDir;
            float nextAngle = wc.steerAngle + rotationThisFrame;


            if (Mathf.Sign(nextAngle) != Mathf.Sign(wc.steerAngle))
                wc.steerAngle = 0;
            else
                wc.steerAngle = nextAngle;
        }
    }

    private void ApplyRotationVisuals(int RotDir)
    {
        for (int i = 0; i < RotationWheels.Length; i++)
        {
            WheelScript Ws = RotationWheels[i];

            float currentAngle = Ws.transform.localEulerAngles.y;
            if (currentAngle > 180f)
                currentAngle -= 360f;

            float rotationThisFrame = visualRotationScalar * Time.deltaTime * RotDir;
            float nextAngle = currentAngle + rotationThisFrame;

            if (Mathf.Abs(nextAngle) <= maxRotationAngle)
            {
                Ws.transform.Rotate(Vector3.up * rotationThisFrame);
            }
            else
            {
                float remaining = (maxRotationAngle * RotDir) - currentAngle;
                Ws.transform.Rotate(Vector3.up * remaining);
            }
        }
    }

    private void DampenRotationVisuals()
    {
        for (int i = 0; i < RotationWheels.Length; i++)
        {
            WheelScript Ws = RotationWheels[i];

            float currentAngle = Ws.transform.localEulerAngles.y;

            if (currentAngle > 180f)
                currentAngle -= 360f;

            if (currentAngle == 0) continue;

            float rotationThisFrame = visualRotationScalar * Time.deltaTime;

            float OppositeDir = Mathf.Sign(currentAngle) * -1;

            float nextAngle = currentAngle + (rotationThisFrame * OppositeDir);

            if (Mathf.Sign(nextAngle) != Mathf.Sign(currentAngle))
            {
                Ws.transform.Rotate(Vector3.up * (0 - currentAngle));
            }
            else
            {
                Ws.transform.Rotate(Vector3.up * rotationThisFrame * OppositeDir);
            }
        }
    }
}
