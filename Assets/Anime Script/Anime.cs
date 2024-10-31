using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anime : MonoBehaviour
{
    public float moveSpeed = 5f; // Kecepatan gerak
    public Transform invisibleMouseTarget; // Referensi ke InvisibleMouseTarget

    private void Update()
    {
        // Gerakan kiri dan kanan dengan A dan D
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        }

        // Aiming dan flip karakter sesuai posisi mouse
        AimAndFlip();
    }

    private void AimAndFlip()
    {
        // Dapatkan arah ke InvisibleMouseTarget
        Vector3 direction = invisibleMouseTarget.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotasi karakter agar menghadap InvisibleMouseTarget
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Balik karakter sesuai posisi mouse di kanan atau kiri
        if (invisibleMouseTarget.position.x < transform.position.x)
        {
            // Mouse di kiri -> flip karakter ke kiri
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            // Mouse di kanan -> flip karakter ke kanan
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
