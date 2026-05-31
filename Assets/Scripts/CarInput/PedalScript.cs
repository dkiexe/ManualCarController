using UnityEngine;
using UnityEngine.InputSystem;

public class PedalScript : MonoBehaviour
{
    [Header("Player Input")]
    [SerializeField] private InputAction playerPress;

    private void OnEnable()
    {
        playerPress.Enable();
    }
    private void OnDisable()
    {
        playerPress.Disable();
    }

    private void OnDestroy()
    {
        playerPress.Dispose();
    }
}
