using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // Untuk mengelola perpindahan scene

public class BowDialogManagerWithSceneTransition : MonoBehaviour
{
    public TMP_Text dialogText; // Tempat untuk menampilkan teks dialog
    public float typingSpeed = 0.05f; // Kecepatan pengetikan
    private bool isTyping = false; // Untuk memeriksa apakah dialog sedang mengetik

    public SceneTransitionManager transitionManager; // Referensi ke SceneTransitionManager

    private int dialogIndex = 0; // Menyimpan urutan dialog

    private void Start()
    {
        // Mulai dialog
        ShowDialog("A bow? I don’t know if these can damage them… Wait but the arrows… looks like my people have always prepared for this. I can use this.");
    }

    // Fungsi untuk memulai dialog
    public void ShowDialog(string message)
    {
        StartCoroutine(TypeDialog(message)); // Mengetik dialog
    }

    // Fungsi untuk mengetikkan dialog secara perlahan
    private IEnumerator TypeDialog(string message)
    {
        dialogText.text = ""; // Menghapus teks sebelumnya
        isTyping = true; // Menandakan dialog sedang mengetik

        // Mengetikkan karakter satu per satu
        foreach (char letter in message.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false; // Dialog selesai mengetik
    }

    private void Update()
    {
        // Jika dialog selesai mengetik dan player menekan tombol Spasi atau klik kiri
        if (!isTyping && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            ContinueDialog();
        }
    }

    // Fungsi untuk melanjutkan dialog
    private void ContinueDialog()
    {
        dialogIndex++; // Menambah urutan dialog

        if (dialogIndex == 1)
        {
            StartCoroutine(TransitionToScene());
        }
    }

    // Fungsi untuk berpindah ke scene TutorialLevel dengan transisi fade-in
    private IEnumerator TransitionToScene()
    {
        if (transitionManager != null)
        {
            // Mulai efek fade-in
            transitionManager.StartFadeIn();
            yield return new WaitForSeconds(transitionManager.fadeDuration); // Tunggu hingga fade selesai
        }

        // Pindah ke scene "TutorialLevel"
        SceneManager.LoadScene("TutorialLevel");
    }
}
