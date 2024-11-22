using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public Image fadeImage; // Gambar untuk efek fade
    public float fadeDuration = 2f; // Durasi fade
    private bool isFading = false;

    private void Awake()
    {
        StartFadeOut(); // Mulai fade-out begitu prefab aktif
    }

    public void StartFadeIn()
    {
        if (!isFading)
        {
            StartCoroutine(Fade(0, 1)); // Fade-in (dari transparan ke hitam)
        }
    }

    public void StartFadeOut()
    {
        if (!isFading)
        {
            StartCoroutine(Fade(1, 0)); // Fade-out (dari hitam ke transparan)
        }
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        isFading = true;
        Color fadeColor = fadeImage.color;
        fadeColor.a = startAlpha;

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeImage.color = fadeColor;
            yield return null;
        }

        fadeColor.a = endAlpha;
        fadeImage.color = fadeColor;

        isFading = false;
    }
}
