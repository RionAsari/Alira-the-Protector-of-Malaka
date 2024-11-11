using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class VolumeSetting : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("music", MathF.Log10(volume)*20);
    }
}

