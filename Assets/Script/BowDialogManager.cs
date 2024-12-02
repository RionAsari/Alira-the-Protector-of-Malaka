using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class BowDialogManagerWithSceneTransition : MonoBehaviour
{
    public TMP_Text dialogText; // Tempat untuk menampilkan teks dialog
    public float typingSpeed = 0.05f; // Kecepatan pengetikan
    private bool isTyping = false; // Untuk memeriksa apakah dialog sedang mengetik
    private bool skipTyping = false; // Untuk melewati pengetikan teks

    public SceneTransitionManager transitionManager; // Referensi ke SceneTransitionManager

    private int dialogIndex = 0; // Menyimpan urutan dialog

    private void Start()
    {
        ShowDialog("A bow? I don’t know if these can damage them… Wait but the arrows… looks like my people have always prepared for this. I can use this.");
    }

    public void ShowDialog(string message)
    {
        StartCoroutine(TypeDialog(message)); // Mengetik dialog
    }

    private IEnumerator TypeDialog(string message)
    {
        dialogText.text = ""; // Menghapus teks sebelumnya
        isTyping = true;
        skipTyping = false; // Reset skipTyping

        foreach (char letter in message.ToCharArray())
        {
            if (skipTyping) // Jika pemain ingin melewati animasi mengetik
            {
                dialogText.text = message; // Tampilkan seluruh teks sekaligus
                break;
            }

            dialogText.text += letter; // Tambahkan huruf satu per satu
            yield return new WaitForSeconds(typingSpeed); // Tunggu sesuai kecepatan mengetik
        }

        isTyping = false; // Pengetikan selesai
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) // Deteksi input
        {
            if (isTyping)
            {
                skipTyping = true; // Langsung selesaikan teks
            }
            else
            {
                ContinueDialog(); // Lanjutkan ke dialog berikutnya
            }
        }
    }

    private void ContinueDialog()
    {
        dialogIndex++; // Pindah ke dialog berikutnya

        if (dialogIndex == 1)
        {
            StartCoroutine(TransitionToScene());
        }
    }

    private IEnumerator TransitionToScene()
    {
        if (transitionManager != null)
        {
            transitionManager.StartFadeIn(); // Mulai fade-in
            yield return new WaitForSeconds(transitionManager.fadeDuration); // Tunggu hingga fade selesai
        }

        SceneManager.LoadScene("TutorialLevel"); // Pindah ke scene berikutnya
    }
}
