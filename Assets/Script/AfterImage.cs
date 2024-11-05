using UnityEngine;
using System.Collections;

public class AfterImageFade : MonoBehaviour
{
    public float fadeDuration = 0.5f; // Durasi fade out

    private SpriteRenderer[] spriteRenderers; // Array untuk menyimpan semua SpriteRenderer

    private void Awake()
    {
        // Mengambil semua SpriteRenderer dalam objek ini
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        // Mengambil warna asli dari semua SpriteRenderer
        Color[] originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalColors[i] = spriteRenderers[i].color;
        }

        // Fade out untuk setiap SpriteRenderer
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                Color newColor = originalColors[i];
                newColor.a = Mathf.Lerp(originalColors[i].a, 0, normalizedTime); // Lerp untuk mengubah alpha
                spriteRenderers[i].color = newColor;
            }
            yield return null;
        }

        // Menghancurkan objek afterimage setelah selesai fade out
        Destroy(gameObject);
    }
}
