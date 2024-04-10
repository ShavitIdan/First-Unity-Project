using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_SoundManager : MonoBehaviour
{
    #region Singleton
    public static SC_SoundManager Instance { get; private set; }

    #endregion

    #region Variables
    public AudioSource BackgroundMusic;
    public AudioSource ClickSound;
    public Slider BackgroundMusicSlider;
    public Slider SfxSlider;
    #endregion

    #region MonoBehavior

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        SetBackgroundMusicVolume(BackgroundMusicSlider.value);
        SetSfxVolume(SfxSlider.value);
    }

    void Start()
    {
        BackgroundMusic.Play();

    }

    #endregion

    #region logic
    public void PlayClickSound()
    {
        ClickSound.Play();
    }

    public void SetBackgroundMusicVolume(float volume)
    {
        BackgroundMusic.volume = volume;
    }

    public void SetSfxVolume(float volume)
    {
        ClickSound.volume = volume;
    }
    #endregion

}
