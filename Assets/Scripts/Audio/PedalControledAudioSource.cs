using System;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(PedalScript))]
public class PedalControlledAudioSource : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private PedalContolledAudioClip[] controlledAudioClips;

    private PedalScript pedal;

    void Start()
    {
        pedal = GetComponent<PedalScript>();
    }

    void Update()
    {
        float currPressure = pedal.PedalPressure;

        foreach (var PC_AudioClip in controlledAudioClips)
        {
            bool ActiveStatus = CheckActive(currPressure, PC_AudioClip);

            if (!ActiveStatus)
            {
                PC_AudioClip.playedInRangeCooldown = false;
                PC_AudioClip.stoppedByDamp = false;
                if (audioSource.clip == PC_AudioClip.audioClip && audioSource.isPlaying)
                {
                    audioSource.Stop();
                    audioSource.clip = null;
                }
                continue;
            }

            if (PC_AudioClip.dampeningDropOff)
            {
                float prevPressureCopy = PC_AudioClip.prevPressure;
                PC_AudioClip.prevPressure = currPressure;
                
                if (prevPressureCopy >= currPressure && currPressure != 1)
                {
                    if (audioSource.isPlaying) audioSource.Stop();
                    PC_AudioClip.stoppedByDamp = true;
                    continue;
                }
                else
                {
                    PC_AudioClip.stoppedByDamp = false;
                }
            }

            if (PC_AudioClip.stoppedByDamp) continue;

            if (PC_AudioClip.playedInRangeCooldown) continue;

            float ClipVolume = DetermineClipVolume(currPressure, PC_AudioClip);

            PlayAudioClip(ClipVolume, PC_AudioClip);
        }
    }

    private bool CheckActive(float currentPressure, PedalContolledAudioClip PC_AudioClip)
    {
        bool lowerBoundcheck;
        bool upperBoundcheck;

        if (PC_AudioClip.activeRange.minInclusive)
        {
            lowerBoundcheck = currentPressure >= PC_AudioClip.activeRange.minPressureActive;
        }
        else
        {
            lowerBoundcheck = currentPressure > PC_AudioClip.activeRange.minPressureActive;
        }

        if (PC_AudioClip.activeRange.maxInclusive)
        {
            upperBoundcheck = currentPressure <= PC_AudioClip.activeRange.maxPressureActive;
        }
        else
        {
            upperBoundcheck = currentPressure < PC_AudioClip.activeRange.maxPressureActive;
        }

        return lowerBoundcheck && upperBoundcheck;
    }

    private float DetermineClipVolume(float currentPressure, PedalContolledAudioClip PC_AudioClip)
    {
        if (PC_AudioClip.regulateVolumeByPressure)
        {
            return Mathf.Lerp(
                    PC_AudioClip.minClipVolume,
                    PC_AudioClip.maxClipVolume,
                    Mathf.SmoothStep(0, 1, currentPressure)
                );
        }
        else
        {
            return PC_AudioClip.maxClipVolume;
        }
    }
    private void PlayAudioClip(float ClipVolume, PedalContolledAudioClip PC_AudioClip)
    {
        if (PC_AudioClip.playOnceInRange)
        {
            audioSource.PlayOneShot(PC_AudioClip.audioClip, ClipVolume);
            PC_AudioClip.playedInRangeCooldown = true;
        }
        else
        {
            if (!audioSource.isPlaying)
            {
                audioSource.volume = ClipVolume;
                audioSource.clip = PC_AudioClip.audioClip;
                audioSource.loop = true;
                audioSource.Play();
            }

            audioSource.volume = ClipVolume; 
        }
    }
}

[Serializable]
public class PedalContolledAudioClip
{
    [SerializeField] public AudioClip audioClip;
    [SerializeField] public ActiveRange activeRange;
    [SerializeField, Range(0, 1)] public float minClipVolume;
    [SerializeField, Range(0, 1)] public float maxClipVolume;
    [SerializeField] public bool regulateVolumeByPressure = true;
    [SerializeField] public bool dampeningDropOff = true;
    [SerializeField] public bool playOnceInRange = false;

    internal bool playedInRangeCooldown = false; // playedCooldown resets once pressure leaves range. (works only with playOncePerRange)
    internal float prevPressure = 0; // the previous pedal pressure to insure prevPressure < current Pressure (works only with dampeningDropOff)
    internal bool stoppedByDamp = false;
}

[Serializable]
public class ActiveRange
{
    [SerializeField, Range(0, 1)] public float minPressureActive;
    [SerializeField, Range(0, 1)] public float maxPressureActive;
    public bool minInclusive = true;
    public bool maxInclusive = false;
}
