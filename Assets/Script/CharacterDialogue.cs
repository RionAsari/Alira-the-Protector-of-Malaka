using UnityEngine;
using TMPro;
using System.Collections;

public class DialogManager : MonoBehaviour
{
    public TMP_Text dialogText; // Tempat untuk menampilkan teks dialog
    public GameObject dialogBackground; // Background dialog
    public float typingSpeed = 0.05f; // Kecepatan pengetikan
    private bool isTyping = false; // Untuk memeriksa apakah dialog sedang mengetik
    public SceneTransitionManager transitionManager; // Komponen SceneTransitionManager untuk fade-in/fade-out
    public GameObject player; // Player untuk dihancurkan setelah dialog selesai
    public GameObject newPlayerPrefab; // Prefab player baru yang akan menggantikan yang lama

    public Vector3 newPlayerPosition = new Vector3(0, 0, 0);  // Posisi prefab baru
    public Vector3 newPlayerScale = new Vector3(1, 1, 1);     // Ukuran prefab baru
    public Vector3 newPlayerRotation = new Vector3(0, 0, 0);  // Rotasi prefab baru

    private int dialogIndex = 0; // Menyimpan urutan dialog

    private void Start()
    {
        ShowDialog("Slowly waking up...");
    }

    public void ShowDialog(string message)
    {
        // Tampilkan background
        if (dialogBackground != null)
            dialogBackground.SetActive(true);

        StartCoroutine(TypeDialog(message)); // Mengetik dialog
    }

    private IEnumerator TypeDialog(string message)
    {
        dialogText.text = "";  // Menghapus teks sebelumnya
        isTyping = true; // Menandakan dialog sedang mengetik

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
        dialogIndex++; // Menambah urutan dialog

        if (dialogIndex == 1)
        {
            ShowDialog("Agh.. I'm hurt, I need something to patch myself up but I have to find a weapon first before they return.");
        }
        else if (dialogIndex == 2)
        {
            EndDialogAndTransition();
        }
    }

    private void EndDialogAndTransition()
    {
        dialogText.text = "";

        // Sembunyikan background
        if (dialogBackground != null)
            dialogBackground.SetActive(false);

        ChangePlayerPrefab();

        if (transitionManager != null)
        {
            transitionManager.StartFadeOut();
        }

        StartCoroutine(DestroyDialogAfterFadeOut());
    }

    private void ChangePlayerPrefab()
    {
        if (player != null && newPlayerPrefab != null)
        {
            Destroy(player);

            Quaternion playerRotation = Quaternion.Euler(newPlayerRotation);
            player = Instantiate(newPlayerPrefab, newPlayerPosition, playerRotation);
            player.transform.localScale = newPlayerScale;
            player.tag = "Player";
        }
    }

    private IEnumerator DestroyDialogAfterFadeOut()
    {
        yield return new WaitForSeconds(transitionManager.fadeDuration);
        Destroy(gameObject);
    }
}
