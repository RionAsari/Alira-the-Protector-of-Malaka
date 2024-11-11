using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;

    private void Start()
    {
        // Cek apakah ada volume yang disimpan di PlayerPrefs
        if (PlayerPrefs.HasKey("musicVolume") || PlayerPrefs.HasKey("SFXVolume"))
        {
            LoadVolume();  // Memuat volume yang disimpan di PlayerPrefs
        }
        else
        {
            // Jika belum ada volume yang disimpan, atur dengan volume default
            SetMusicVolume();
            SetSFXVolume();
        }
    }

    public void SetMusicVolume()
    {
        // Set volume musik berdasarkan nilai dari slider
        float volume = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume); // Menyimpan nilai volume ke PlayerPrefs
    }

    public void SetSFXVolume()
    {
        // Set volume SFX berdasarkan nilai dari slider
        float volume = SFXSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume); // Menyimpan nilai volume ke PlayerPrefs
    }

    private void LoadVolume()
    {
        // Memuat nilai volume musik dari PlayerPrefs dan menetapkan nilai pada slider
        float musicVolume = PlayerPrefs.GetFloat("musicVolume");
        musicSlider.value = musicVolume;
        myMixer.SetFloat("music", Mathf.Log10(musicVolume) * 20); // Terapkan volume musik ke mixer

        // Memuat nilai volume SFX dari PlayerPrefs dan menetapkan nilai pada slider
        float SFXVolume = PlayerPrefs.GetFloat("SFXVolume");
        SFXSlider.value = SFXVolume;
        myMixer.SetFloat("SFX", Mathf.Log10(SFXVolume) * 20); // Terapkan volume SFX ke mixer
    }
}
