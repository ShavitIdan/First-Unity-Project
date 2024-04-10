using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_AnimationSoundPlayer : MonoBehaviour
{
    #region Variables

    [SerializeField] private AudioClip ShootAudioClip;
    [SerializeField] private AudioClip StepsAudioClip;
    private AudioSource audioSource;

    #endregion

    #region MonoBehaviour
    private void Awake()
    {
       audioSource = GetComponent<AudioSource>(); 
    }

    #endregion

    #region Logic
    private void PlaySound(AudioClip clip)
    {
        if(audioSource && !audioSource.isPlaying)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    public void PlayShootSound()
    {
        PlaySound(ShootAudioClip);
    }

    public void PlayStepsSound()
    {
        int randomIndex = (int)Random.Range(0, StepsAudioClip.length);
        PlaySound(StepsAudioClip);
    }

    public void StopSound()
    {
        audioSource.Stop();
    }
    #endregion

}
