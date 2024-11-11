using System;
using UnityEngine;
using UnityEngine.Audio;

public class LoadVolumeOnStart : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;

    private void Start()
    {
        // Cek apakah ada nilai volume yang tersimpan di PlayerPrefs
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            float volume = PlayerPrefs.GetFloat("musicVolume");
            myMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        }
        else
        {
            // Jika belum ada, atur nilai default volume (misalnya 1 atau sesuai kebutuhan)
            myMixer.SetFloat("music", Mathf.Log10(1f) * 20);
        }
    }
}
