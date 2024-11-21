using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class BowDialogManagerWithSceneTransition : MonoBehaviour
{
    public TMP_Text dialogText; // Tempat untuk menampilkan teks dialog
    public float typingSpeed = 0.05f; // Kecepatan pengetikan
    private bool isTyping = false; // Untuk memeriksa apakah dialog sedang mengetik

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

        foreach (char letter in message.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false; // Dialog selesai mengetik
    }

    private void Update()
    {
        if (!isTyping && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            ContinueDialog();
        }
    }

    private void ContinueDialog()
    {
        dialogIndex++;

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
            yield return new WaitForSeconds(transitionManager.fadeDuration); // Tunggu fade selesai
        }

        SceneManager.LoadScene("TutorialLevel");
    }
}
