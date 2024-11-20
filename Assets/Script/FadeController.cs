using UnityEngine;
using UnityEngine.Playables;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public PlayableDirector timelineDirector;
    public float delayBeforeFadeIn = 2f;  // Delay sebelum fade in dimulai

    private void Start()
    {
        StartCoroutine(PlayFadeAfterText());
    }

    private IEnumerator PlayFadeAfterText()
    {
        // Menunggu beberapa detik sampai teks selesai
        yield return new WaitForSeconds(delayBeforeFadeIn);

        // Mulai animasi fade
        timelineDirector.Play();
    }
}
