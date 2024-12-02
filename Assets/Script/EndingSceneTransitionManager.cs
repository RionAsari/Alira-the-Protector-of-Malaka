using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingSceneTransitionManager : MonoBehaviour
{
    [SerializeField] private Image fadeImage; // Panel hitam untuk transisi
    [SerializeField] private float fadeDuration = 1f; // Durasi transisi (dalam detik)

    private void Start()
    {
        // Pastikan fadeImage aktif dan transparan
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            StartCoroutine(FadeIn());
        }
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName)); // Memulai fade out
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, timer / fadeDuration); // Kurangi alpha ke 0
            fadeImage.color = color;
            yield return null;
        }

        fadeImage.gameObject.SetActive(false); // Sembunyikan panel setelah selesai
    }

    private IEnumerator FadeOut(string sceneName)
    {
        float timer = 0f;
        Color color = fadeImage.color;

        fadeImage.gameObject.SetActive(true);

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, timer / fadeDuration); // Tambahkan alpha ke 1
            fadeImage.color = color;
            yield return null;
        }

        SceneManager.LoadScene(sceneName); // Muat scene baru
    }
}
