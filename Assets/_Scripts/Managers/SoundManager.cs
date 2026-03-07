using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    void Start()
    {
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 1);
            load();
        }
        else        {
            load();
        }
    }

    public void ChangeVolume()
    {
        AudioListener.volume = musicVolumeSlider.value;
        save();
    }

    private void save()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
    }
    private void load()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
    }
}
