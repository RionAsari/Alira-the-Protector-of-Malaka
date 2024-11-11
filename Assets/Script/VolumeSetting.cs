using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;

    private void Start()
    {
        // Cek apakah ada volume yang disimpan di PlayerPrefs
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();  // Memuat volume yang disimpan di PlayerPrefs
        }
        else
        {
            // Jika belum ada volume yang disimpan, atur dengan volume default
            SetMusicVolume();
        }
    }

    public void SetMusicVolume()
    {
        // Set volume musik berdasarkan nilai dari slider
        float volume = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume); // Menyimpan nilai volume ke PlayerPrefs
    }

    private void LoadVolume()
    {
        // Memuat nilai volume dari PlayerPrefs dan menetapkan nilai pada slider
        float volume = PlayerPrefs.GetFloat("musicVolume");
        musicSlider.value = volume;
        myMixer.SetFloat("music", Mathf.Log10(volume) * 20); // Terapkan volume ke mixer
    }
}
