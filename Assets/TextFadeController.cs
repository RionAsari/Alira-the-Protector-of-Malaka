using UnityEngine;
using UnityEngine.Playables;

public class TextFadeController : MonoBehaviour
{
    public PlayableDirector timelineDirector;
    public float fadeInDuration = 2f;  // Durasi fade in
    public float fadeOutDuration = 2f; // Durasi fade out

    private void Start()
    {
        // Kamu bisa memulai Timeline di sini, atau trigger melalui event lain
        timelineDirector.Play();
    }
}
