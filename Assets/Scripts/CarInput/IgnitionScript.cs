using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class IgnitionScript : MonoBehaviour
{
    [field: SerializeField] public IgnitionState ignitionState { get; private set; } = IgnitionState.IgnitionOn;

    [Header("Audio")]
    [SerializeField] private AudioSource idleAudioSource;
    [SerializeField] private AudioClip startUpAudioClip;
    [SerializeField] private AudioClip engineShutoffClip;

    [Header("Player Input")]
    [SerializeField] internal InputAction keyInput;

    internal event Action<IgnitionState> OnIgnitionStateChange;

    public bool isIgnitionOn => ignitionState == IgnitionState.IgnitionOn;

    private void OnEnable()
    {
        keyInput.Enable();
        keyInput.performed += HandleIgnitionInput;
    }

    private void OnDisable()
    {
        keyInput.performed -= HandleIgnitionInput;
    }

    private void HandleIgnitionInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            int StateInex = (int)ignitionState;

            ignitionState = (IgnitionState)((StateInex + 1) % 2);

            ManageCarIdleSounds();
        }
    }

    private void ManageCarIdleSounds()
    {
        if (ignitionState == IgnitionState.IgnitionOn)
        {
            playStartUpSounds();
        }
        else
        {
            stopIdleSounds();
            idleAudioSource.PlayOneShot(engineShutoffClip, 3f);
        }
    }

    internal void StopIgniton()
    {
        ignitionState = IgnitionState.IgnitionOff;
        ManageCarIdleSounds();
    }

    private void playStartUpSounds()
    {
        idleAudioSource.PlayOneShot(startUpAudioClip, 3f);
        StartCoroutine(ScheduleIdleSounds(startUpAudioClip.length));
    }

    private void stopIdleSounds()
    {
        if (idleAudioSource.isPlaying) idleAudioSource.Stop();
    }

    private IEnumerator ScheduleIdleSounds(float DelayT)
    {
        yield return new WaitForSeconds(DelayT);

        idleAudioSource.Play();
    }
}

[Serializable]
public enum IgnitionState
{
    IgnitionOn,
    IgnitionOff,
}
