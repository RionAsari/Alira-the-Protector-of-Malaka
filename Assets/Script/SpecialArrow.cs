using System.Collections;
using UnityEngine;

public class SpecialArrow : MonoBehaviour
{
    public float disableDuration = 5f; // Durasi musuh dinonaktifkan

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Jika panah mengenai musuh dengan tag "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (enemy != null && !enemy.isAlly) // Cek apakah musuh bukan sekutu
            {
                StartCoroutine(DisableEnemy(enemy)); // Disable musuh
            }
        }

        // Hancurkan panah setelah mengenai objek
        Destroy(gameObject);
    }

    // Coroutine untuk menonaktifkan musuh
    private IEnumerator DisableEnemy(Enemy enemy)
    {
        enemy.isHackable = true; // Set musuh menjadi hackable
        yield return enemy.DisableEnemy(); // Memanggil fungsi DisableEnemy di script Enemy

        yield return new WaitForSeconds(disableDuration); // Tunggu sampai disable duration selesai

        if (!enemy.isAlly) // Jika musuh belum di-hack, musuh dihancurkan
        {
            enemy.isHackable = false; // Musuh tidak bisa di-hack lagi
            Destroy(enemy.gameObject); // Hancurkan musuh
        }
    }
}
