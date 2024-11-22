using UnityEngine;
using TMPro; // Hanya jika menggunakan TextMeshPro
using System.Collections;

public class TutorialTrigger2D : MonoBehaviour
{
    [SerializeField] private string message = "Press A and D to move"; // Pesan yang akan ditampilkan
    [SerializeField] private TextMeshProUGUI tutorialText; // Text untuk menampilkan pesan
    [SerializeField] private float fadeDuration = 1f; // Durasi fade in/out

    private CanvasGroup canvasGroup;

    private void Start()
    {
        // Mendapatkan CanvasGroup dari tutorialText
        canvasGroup = tutorialText.GetComponent<CanvasGroup>();
        
        // Pastikan teks mulai dalam keadaan transparan
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            tutorialText.text = message; // Set pesan
            tutorialText.gameObject.SetActive(true); // Tampilkan teks
            StartCoroutine(FadeIn()); // Mulai fade in
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(FadeOut()); // Mulai fade out
        }
    }

    // Coroutine untuk fade in
    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f; // Pastikan alpha selesai di 1
    }

    // Coroutine untuk fade out
    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f; // Pastikan alpha selesai di 0
        tutorialText.gameObject.SetActive(false); // Sembunyikan teks setelah fade out
    }
}
