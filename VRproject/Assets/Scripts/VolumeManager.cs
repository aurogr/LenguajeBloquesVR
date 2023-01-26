using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance; // singleton

    [SerializeField] private AudioMixer _mixer;

    const string GENERAL_KEY = "generalVolume";
    const string MUSIC_KEY = "musicVolume";
    const string SFX_KEY = "sfxVolume";

    private float _generalVolume;
    private float _musicVolume;
    private float _sfxVolume;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadVolume();
    }

    void LoadVolume()
    {
        _generalVolume = PlayerPrefs.GetFloat(GENERAL_KEY, 1f);
        _musicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        _sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);

        _mixer.SetFloat(VolumeSettings.MIXER_GENERAL, Mathf.Log10(_generalVolume) * 20);
        _mixer.SetFloat(VolumeSettings.MIXER_MUSIC, Mathf.Log10(_musicVolume) * 20);
        _mixer.SetFloat(VolumeSettings.MIXER_SFX, Mathf.Log10(_sfxVolume) * 20);
    }

    public void SaveVolume() // call this when exiting the game (it could also be called when disabling volume settings so it saves every time)
    {
        PlayerPrefs.SetFloat(GENERAL_KEY, _generalVolume);
        PlayerPrefs.SetFloat(MUSIC_KEY, _musicVolume);
        PlayerPrefs.SetFloat(SFX_KEY, _sfxVolume);
    }

    #region Getters/Setters
    public void SetGeneralVolume(float value)
    {
        _generalVolume = value;
    }

    public void SetMusicVolume(float value)
    {
        _musicVolume = value;
    }

    public void SetSfxVolume(float value)
    {
        _sfxVolume = value;
    }

    public float GetGeneralVolume()
    {
        return _generalVolume;
    }
    public float GetMusicVolume()
    {
        return _musicVolume;
    }
    public float GetSfxVolume()
    {
        return _sfxVolume;
    }


    #endregion

}
