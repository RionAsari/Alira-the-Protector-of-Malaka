using UnityEngine;
using System.Collections;
using Cinemachine;  // Pastikan untuk menambahkan ini untuk menggunakan Cinemachine

public class Health : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public HealthbarBehaviour healthbar;
    private Animator animator;

    public GameOverManager gameOverManager; // Referensi ke GameOverManager
    public CinemachineVirtualCamera virtualCamera; // Referensi ke Cinemachine Virtual Camera

    public bool isDead = false;  // Menandakan apakah pemain sudah mati atau belum

    // Tambahan untuk AudioSource dan AudioClip healing
    public AudioClip healSound;  // Suara saat pemulihan
    private AudioSource audioSource;  // Referensi ke AudioSource untuk memutar suara healing

    private void Start()
    {
        animator = GetComponent<Animator>(); // Mengambil referensi ke komponen Animator

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth);  // Set health bar pada nilai awal
        }

        audioSource = GetComponent<AudioSource>();  // Mengambil referensi ke komponen AudioSource
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;  // Jika sudah mati, jangan proses damage lagi

        health -= damage;
        health = Mathf.Max(0, health); // Pastikan kesehatan tidak kurang dari 0

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth);  // Update health bar
        }

        if (health <= 0)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        if (isDead) return;  // Jika sudah mati, jangan proses heal lagi

        health += healAmount;
        health = Mathf.Min(health, maxHealth); // Pastikan kesehatan tidak melebihi maxHealth

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth);  // Update health bar
        }

        Debug.Log($"Player healed by {healAmount} points. Current health: {health}");

        // Mainkan suara healing jika ada
        if (healSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(healSound);  // Memutar suara healing
        }
    }

    private void Die()
    {
        // Trigger animasi mati
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        isDead = true;  // Tandai pemain sebagai mati

        // Mengubah tag menjadi PlayerDeath agar musuh tidak menyerang
        gameObject.tag = "PlayerDeath";

        // Call the GameOver method after animation is complete
        StartCoroutine(WaitForDeathAnimation());
    }

    private IEnumerator WaitForDeathAnimation()
    {
        // Tunggu sampai animasi mati selesai
        yield return new WaitForSeconds(1f);  // Sesuaikan waktu dengan durasi animasi mati

        // Pastikan kamera tetap di posisi pemain terakhir
        if (virtualCamera != null)
        {
            // Menetapkan posisi kamera tetap pada posisi pemain terakhir
            virtualCamera.Follow.position = transform.position;
            virtualCamera.LookAt.position = transform.position; // Jika juga ingin kamera mengarah ke pemain
        }

        // Setelah animasi selesai, munculkan Game Over
        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOverMenu();
        }
    }
}
