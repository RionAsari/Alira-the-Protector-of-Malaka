using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifetime = 5f; // Waktu sebelum panah dihancurkan
    public float colliderDelay = 0.2f; // Waktu sebelum collider diaktifkan

    private Collider2D arrowCollider; // Collider pada panah

    private void Start()
    {
        // Ambil komponen collider
        arrowCollider = GetComponent<Collider2D>();

        // Matikan collider di awal
        arrowCollider.enabled = false;

        // Aktifkan kembali collider setelah beberapa waktu
        StartCoroutine(EnableColliderAfterDelay());

        // Hancurkan panah setelah waktu tertentu
        Destroy(gameObject, lifetime);
    }

    // Coroutine untuk mengaktifkan kembali collider setelah delay
    private IEnumerator EnableColliderAfterDelay()
    {
        yield return new WaitForSeconds(colliderDelay);
        arrowCollider.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Hancurkan panah saat menabrak objek
        Destroy(gameObject);
    }
}