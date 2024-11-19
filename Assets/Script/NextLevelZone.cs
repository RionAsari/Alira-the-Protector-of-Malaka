using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class NextLevelManager : MonoBehaviour
{
    [SerializeField] private GameObject fadeImage;  // GameObject untuk fade image
    [SerializeField] private KeyCode nextLevelKey = KeyCode.F;  // Tombol untuk lanjut ke level selanjutnya

    private bool isPlayerInZone = false;

    private void Update()
    {
        // Pastikan player di dalam zona dan semua musuh (LightGrunt, MiddleBot) sudah kalah
        if (isPlayerInZone && LightGrunt.activeEnemies <= 0 && MiddleBot.activeEnemies <= 0)
        {
            Debug.Log("Player is in zone and enemies are defeated.");

            // Pengecekan tombol F untuk melanjutkan ke level selanjutnya
            if (Input.GetKeyDown(nextLevelKey))
            {
                Debug.Log("F key pressed. Loading next level...");
                StartCoroutine(LoadNextLevel());
            }
        }
    }

    private IEnumerator LoadNextLevel()
    {
        // Mulai animasi fade in
        fadeImage.SetActive(true);
        Image fade = fadeImage.GetComponent<Image>();

        // Fade in ke hitam
        for (float t = 0; t <= 1; t += Time.deltaTime)
        {
            fade.color = new Color(0, 0, 0, t);  // Ubah alpha untuk fade in
            yield return null;
        }

        // Muat scene berikutnya secara dinamis berdasarkan urutan scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;  // Scene berikutnya

        // Pastikan scene berikutnya ada dalam build settings
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogError("No next scene available. Please add more scenes to the build settings.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            Debug.Log("Player entered the zone.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            Debug.Log("Player left the zone.");
        }
    }
}
