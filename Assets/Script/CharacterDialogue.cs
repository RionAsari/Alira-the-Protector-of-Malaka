using UnityEngine;
using TMPro;
using System.Collections;

public class DialogManager : MonoBehaviour
{
    public TMP_Text dialogText; // Tempat untuk menampilkan teks dialog
    public float typingSpeed = 0.05f; // Kecepatan pengetikan
    private bool isTyping = false; // Untuk memeriksa apakah dialog sedang mengetik
    public SceneTransitionManager transitionManager; // Komponen SceneTransitionManager untuk fade-in/fade-out
    public GameObject player; // Player untuk dihancurkan setelah dialog selesai
    public GameObject newPlayerPrefab; // Prefab player baru yang akan menggantikan yang lama

    // Variabel untuk menentukan posisi, ukuran, dan rotasi
    public Vector3 newPlayerPosition = new Vector3(0, 0, 0);  // Posisi prefab baru
    public Vector3 newPlayerScale = new Vector3(1, 1, 1);     // Ukuran prefab baru
    public Vector3 newPlayerRotation = new Vector3(0, 0, 0);  // Rotasi prefab baru

    private int dialogIndex = 0; // Menyimpan urutan dialog

    private void Start()
    {
        // Mulai dengan dialog pertama
        ShowDialog("Slowly waking up...");
    }

    // Fungsi untuk memulai dialog
    public void ShowDialog(string message)
    {
        StartCoroutine(TypeDialog(message)); // Mengetik dialog
    }

    // Fungsi untuk mengetikkan dialog secara perlahan
    private IEnumerator TypeDialog(string message)
    {
        dialogText.text = "";  // Menghapus teks sebelumnya
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
            // Melanjutkan ke dialog berikutnya
            ContinueDialog();
        }
    }

    // Fungsi untuk melanjutkan ke dialog berikutnya
    private void ContinueDialog()
    {
        // Ganti teks dialog sesuai dengan urutan
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

    // Fungsi untuk mengganti prefab player
    private void ChangePlayerPrefab()
    {
        if (player != null && newPlayerPrefab != null)
        {
            // Pastikan player yang lama dihancurkan
            Destroy(player);

            // Mengatur rotasi sesuai dengan nilai yang diberikan
            Quaternion playerRotation = Quaternion.Euler(newPlayerRotation);

            // Instansiasi prefab player baru dengan posisi, rotasi, dan skala yang ditentukan
            player = Instantiate(newPlayerPrefab, newPlayerPosition, playerRotation);

            // Mengatur skala player baru
            player.transform.localScale = newPlayerScale;

            // Opsional: ubah tag atau komponen lainnya jika perlu
            player.tag = "Player"; // Ganti dengan tag yang sesuai jika perlu
        }
    }

    // Fungsi untuk mengakhiri dialog dan memulai transisi
    private void EndDialogAndTransition()
    {
        // Menghancurkan teks dialog terlebih dahulu
        dialogText.text = "";

        // Panggil untuk mengganti prefab player setelah teks dihapus
        ChangePlayerPrefab();

        // Mulai transisi fade-out setelah teks dihapus
        if (transitionManager != null)
        {
            transitionManager.StartFadeOut();
        }

        // Hancurkan objek dialog setelah transisi selesai
        StartCoroutine(DestroyDialogAfterFadeOut());
    }

    // Menghancurkan dialog setelah fade-out selesai
    private IEnumerator DestroyDialogAfterFadeOut()
    {
        // Tunggu durasi fade-out
        yield return new WaitForSeconds(transitionManager.fadeDuration);

        // Hancurkan objek dialog setelah transisi selesai
        Debug.Log($"Menghancurkan GameObject dialog: {gameObject.name}");
        Destroy(gameObject);

        // Jika kamu ingin melanjutkan ke scene baru atau memperkenalkan objek lain
        // kamu bisa memulai transisi scene atau objek lain di sini
    }
}
