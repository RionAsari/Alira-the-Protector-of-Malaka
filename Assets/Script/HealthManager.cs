using UnityEngine;
using System.Collections;
using Cinemachine; // Untuk Cinemachine

public class Health : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public HealthbarBehaviour healthbar;
    private Animator animator;

    public GameOverManager gameOverManager; // Referensi ke GameOverManager
    public CinemachineVirtualCamera virtualCamera; // Referensi ke Cinemachine Virtual Camera

    public bool isDead = false; // Menandakan apakah pemain sudah mati atau belum

    public Transform bowTransform; // Referensi ke BowTransform

    // Tambahan untuk AudioSource dan AudioClip healing
    public AudioClip healSound; // Suara saat pemulihan
    public AudioClip dieSound;
    public AudioClip damageSound;
    private AudioSource audioSource; // Referensi ke AudioSource untuk memutar suara healing

    private void Start()
    {
        animator = GetComponent<Animator>(); // Mengambil referensi ke komponen Animator

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth); // Set health bar pada nilai awal
        }

        audioSource = GetComponent<AudioSource>(); // Mengambil referensi ke komponen AudioSource
    }

    public void TakeDamage(float damage)
{
    if (isDead) return; // Jika sudah mati, jangan proses damage lagi

    health -= damage;
    health = Mathf.Max(0, health); // Pastikan kesehatan tidak kurang dari 0

    // Mainkan suara saat terkena damage
    if (damageSound != null && audioSource != null)
    {
        audioSource.PlayOneShot(damageSound); // Memutar suara damage
    }

    if (healthbar != null)
    {
        healthbar.SetHealth(health, maxHealth); // Update health bar
    }

    if (health <= 0)
    {
        Die();
    }
}


    public void Heal(float healAmount)
    {
        if (isDead) return; // Jika sudah mati, jangan proses heal lagi

        health += healAmount;
        health = Mathf.Min(health, maxHealth); // Pastikan kesehatan tidak melebihi maxHealth

        if (healthbar != null)
        {
            healthbar.SetHealth(health, maxHealth); // Update health bar
        }

        Debug.Log($"Player healed by {healAmount} points. Current health: {health}");

        // Mainkan suara healing jika ada
        if (healSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(healSound); // Memutar suara healing
        }
    }

private void Die()
{
    if (dieSound != null && audioSource != null)
    {
        audioSource.PlayOneShot(dieSound);
    }

    // Trigger animasi mati
    if (animator != null)
    {
        animator.SetTrigger("Die");
    }

    isDead = true; // Tandai pemain sebagai mati

    // Mengubah tag menjadi PlayerDeath agar musuh tidak menyerang
    gameObject.tag = "PlayerDeath";

    // Aktifkan physics agar pemain mulai jatuh sambil memainkan animasi
    Rigidbody2D rb = GetComponent<Rigidbody2D>();
    if (rb != null)
    {
        rb.isKinematic = false; // Aktifkan physics
        rb.velocity = Vector2.zero; // Pastikan kecepatan awal direset
        rb.gravityScale = 1; // Pastikan gravitasi diaktifkan
    }

    // Hentikan kamera mengikuti pemain
    if (virtualCamera != null)
    {
        virtualCamera.Follow = null;
    }

    // Nonaktifkan BowTransform
    if (bowTransform != null)
    {
        bowTransform.gameObject.SetActive(false);
    }

    // Munculkan menu Game Over setelah animasi selesai
    StartCoroutine(WaitForDeathAnimation());
}

private IEnumerator WaitForDeathAnimation()
{
    // Tunggu durasi animasi mati selesai
    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    float deathAnimationLength = stateInfo.length;
    yield return new WaitForSeconds(deathAnimationLength);

    // Setelah animasi selesai, munculkan Game Over
    if (gameOverManager != null)
    {
        gameOverManager.ShowGameOverMenu();
    }
}

}
