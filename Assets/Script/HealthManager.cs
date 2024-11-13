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

    private void Start()
    {
        animator = GetComponent<Animator>(); // Assuming the player has an Animator component

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;  // Jika sudah mati, jangan proses damage lagi

        health -= damage;
        health = Mathf.Max(0, health); // Ensure health doesn't go below 0

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth);
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
        health = Mathf.Min(health, maxHealth); // Ensure health doesn't exceed maxHealth

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth);
        }

        Debug.Log($"Player healed by {healAmount} points. Current health: {health}");

        // Play heal sound effect
        AudioManager.instance.PlayHealSound();
    }

    private void Die()
    {
        // Trigger death animation
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        isDead = true;  // Tandai bahwa pemain sudah mati

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
