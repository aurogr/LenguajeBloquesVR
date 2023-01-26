using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private Slider _generalSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    public const string MIXER_GENERAL = "GeneralVolume";
    public const string MIXER_MUSIC = "MusicVolume";
    public const string MIXER_SFX = "SfxVolume";

    private void Awake()
    {
        // not doing it on OnEnable and OnDisable because it won't be called if it's not enabled anyways
        _generalSlider.onValueChanged.AddListener(SetGeneralVolume);
        _musicSlider.onValueChanged.AddListener(SetMusicVolume);
        _sfxSlider.onValueChanged.AddListener(SetSfxVolume);
    }

    private void Start()
    {
        GetInitialValues();
    }

    void GetInitialValues()
    {
        _generalSlider.value = VolumeManager.Instance.GetGeneralVolume();
        _musicSlider.value = VolumeManager.Instance.GetMusicVolume();
        _sfxSlider.value = VolumeManager.Instance.GetSfxVolume();
    }

    void SetGeneralVolume (float value)
    {
        _mixer.SetFloat(MIXER_GENERAL, Mathf.Log10(value)*20);
        VolumeManager.Instance.SetGeneralVolume(value);
    }
    
    void SetMusicVolume (float value)
    {
        _mixer.SetFloat(MIXER_MUSIC, Mathf.Log10(value)*20);
        VolumeManager.Instance.SetMusicVolume(value);
    }
    
    void SetSfxVolume (float value)
    {
        _mixer.SetFloat(MIXER_SFX, Mathf.Log10(value)*20);
        VolumeManager.Instance.SetSfxVolume(value);
    }

}
