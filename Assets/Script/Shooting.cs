using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;
    private Transform playerTransform;
    public GameObject arrowPrefab; // Prefab untuk panah
    public Transform bowTransform; // Posisi bow tempat anak panah muncul
    public float maxArrowSpeed = 15f; // Kecepatan maksimum panah
    public float chargeSpeed = 5f; // Kecepatan charge
    private float currentCharge = 0f; // Kekuatan charge yang diisi
    private bool isCharging = false; // Apakah sedang mengisi daya
    private bool canFire = true;
    private float timer;
    public float timeBetweenFiring = 0.5f; // Waktu cooldown antar tembakan

    private void Start()
    {
        mainCam = Camera.main; // Ambil referensi kamera utama
        playerTransform = transform.parent; // Ambil referensi player dari parent
    }

    private void Update()
    {
        // Dapatkan posisi mouse dalam koordinat dunia
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mousePos - transform.position;

        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        // Jika player menghadap kiri, balikkan rotasi
        if (playerTransform.localScale.x < 0)
        {
            rotZ += 180;
        }

        // Atur rotasi bow
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        // Mulai charging ketika klik mouse ditekan
        if (Input.GetMouseButtonDown(0) && canFire)
        {
            isCharging = true;
            currentCharge = 0f;
        }

        // Proses charging selama tombol mouse ditahan
        if (Input.GetMouseButton(0) && isCharging)
        {
            currentCharge += chargeSpeed * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0f, maxArrowSpeed);
        }

        // Lepas panah saat mouse dilepas
        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            isCharging = false;
            FireArrow(currentCharge); // Tembakkan panah dengan charge yang terkumpul
            canFire = false;
            currentCharge = 0f; // Reset charge
        }

        // Cooldown antar tembakan
        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring)
            {
                canFire = true;
                timer = 0;
            }
        }
    }

    // Fungsi untuk menembakkan panah
    private void FireArrow(float charge)
    {
        // Instansiasi panah
        GameObject arrow = Instantiate(arrowPrefab, bowTransform.position, bowTransform.rotation);

        // Dapatkan komponen Rigidbody2D untuk memberi kecepatan
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();

        // Hitung arah panah sesuai dengan arah bow
        Vector2 shootDirection = (mousePos - bowTransform.position).normalized;

        // Berikan kecepatan pada panah berdasarkan arah dan charge
        rb.velocity = shootDirection * charge;

        // Flip sprite panah jika player menghadap ke kiri
        if (playerTransform.localScale.x < 0)
        {
            Vector3 arrowScale = arrow.transform.localScale;
            arrowScale.x *= -1; // Membalikkan skala di sumbu X (untuk sprite flip)
            arrow.transform.localScale = arrowScale;
        }

        // Reset charge setelah panah ditembakkan
        currentCharge = 0;
    }
}