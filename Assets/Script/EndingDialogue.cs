using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndingDialogue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI characterNameText; // TextMeshPro untuk nama karakter
    [SerializeField] private TextMeshProUGUI dialogText; // TextMeshPro untuk teks dialog
    [SerializeField] private float typingSpeed = 0.05f; // Kecepatan mengetik teks
    [SerializeField] private EndingSceneTransitionManager transitionManager; // Referensi ke transition manager

    private (string characterName, string dialog)[] dialogLines = new (string, string)[]
    {
        ("Alira", "Mom! Are you okay?"),
        ("Alira's Mother", "Yes I’m okay, thank you so much Alira. I’m sorry you had to go through this."),
        ("Alira", "It’s okay mom, I used what I learned from you and father. Are our people with you?"),
        ("Alira's Mother", "No we got separated when the Iron Beasts attacked, they took our people elsewhere."),
        ("Alira", "Then we have to save them!"),
        ("Alira's Mother", "We will Alira, know that we’re proud of you. I’m sure your father does too."),
        ("Alira", "Thank you mom, I'm glad that I can get to you before it was too late, then let’s go!")
    };

    private int currentLineIndex = 0; // Indeks kalimat saat ini
    private bool isTyping = false; // Apakah teks sedang diketik
    private bool skipTyping = false; // Apakah teks harus di-skip langsung

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) // Deteksi input spasi atau klik kiri
        {
            if (isTyping)
            {
                skipTyping = true; // Hentikan proses mengetik dan tampilkan seluruh kalimat
            }
            else
            {
                NextLine(); // Tampilkan kalimat berikutnya
            }
        }
    }

    private void Start()
    {
        StartCoroutine(TypeLine()); // Mulai dialog
    }

    private IEnumerator TypeLine()
    {
        isTyping = true; // Mulai mengetik
        skipTyping = false; // Reset skipTyping

        // Set nama karakter
        characterNameText.text = dialogLines[currentLineIndex].characterName;

        // Tampilkan dialog secara bertahap
        dialogText.text = ""; // Kosongkan teks dialog sebelumnya
        foreach (char letter in dialogLines[currentLineIndex].dialog.ToCharArray())
        {
            if (skipTyping)
            {
                dialogText.text = dialogLines[currentLineIndex].dialog; // Tampilkan seluruh dialog
                break;
            }

            dialogText.text += letter; // Tambahkan huruf satu per satu
            yield return new WaitForSeconds(typingSpeed); // Tunggu sesuai kecepatan mengetik
        }

        isTyping = false; // Pengetikan selesai
    }

    private void NextLine()
    {
        if (currentLineIndex < dialogLines.Length - 1)
        {
            currentLineIndex++; // Pindah ke dialog berikutnya
            StartCoroutine(TypeLine()); // Ketik dialog baru
        }
        else
        {
            Debug.Log("Dialog selesai!"); // Dialog selesai
            StartCoroutine(TransitionToCreditScene()); // Pindah ke scene Credit
        }
    }

    private IEnumerator TransitionToCreditScene()
    {
        if (transitionManager != null)
        {
            transitionManager.LoadSceneWithFade("Credit"); // Memanggil transisi fade dan load scene Credit
        }
        yield return null;
    }
}
