using UnityEngine;
using UnityEngine.InputSystem;

public class SteeringWheelScript : MonoBehaviour
{
    [Header("Player Input")]
    [SerializeField] private InputAction leftTurnInput;
    [SerializeField] private InputAction RightTurnInput;

    private void OnEnable()
    {
        leftTurnInput.Enable();
        RightTurnInput.Enable();
    }
    private void OnDisable()
    {
        leftTurnInput.Disable();
        RightTurnInput.Disable();
    }

    private void OnDestroy()
    {
        leftTurnInput.Dispose();
        RightTurnInput.Dispose();
    }
}
