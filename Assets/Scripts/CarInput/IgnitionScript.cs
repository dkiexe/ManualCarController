using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class IgnitionScript : MonoBehaviour
{
    [field: SerializeField] public IgnitionState ignitionState { get; private set; } = IgnitionState.IgnitionOn;

    [Header("UI Settings && Refrences")]
    [SerializeField] private Image[] UIGauges;
    [SerializeField] private TextMeshProUGUI InstructionText;

    [Header("Audio")]
    [SerializeField] private AudioSource idleAudioSource;
    [SerializeField] private AudioClip startUpAudioClip;
    [SerializeField] private AudioClip engineShutoffClip;

    [Header("Player Input")]
    [SerializeField] internal InputAction keyInput;

    public bool isIgnitionOn => ignitionState == IgnitionState.IgnitionOn;

    private void OnEnable()
    {
        keyInput.Enable();
        keyInput.performed += HandleIgnitionInput;
    }

    private void Start()
    {
        ManageCarIU();
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

            ManageCarIU();
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
    private void ManageCarIU()
    {
        if (ignitionState == IgnitionState.IgnitionOn)
        {
            InstructionText.gameObject.SetActive(false);
            SetAlphaUI(1);
        }
        else
        {
            InstructionText.gameObject.SetActive(true);
            SetAlphaUI(0.5f);
        }
    }

    private void SetAlphaUI(float alpha)
    {
        foreach(Image UIGuage in UIGauges)
        {
            Color c = UIGuage.color;

            c.a = alpha;

            UIGuage.color = c;
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
