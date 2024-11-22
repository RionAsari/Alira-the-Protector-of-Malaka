using UnityEngine;

public class CustomCursorObject : MonoBehaviour
{
    public GameObject cursorObject;  // GameObject untuk cursor custom
    public float cursorOffset = 10f;  // Menentukan posisi Z, tergantung pada kamera dan kebutuhanmu

    void Start()
    {
        // Menyembunyikan kursor default
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;  // Membatasi kursor agar tetap di dalam layar
    }

    void Update()
    {
        // Ambil posisi mouse di layar
        Vector3 mousePosition = Input.mousePosition;

        // Mengubah posisi objek cursor agar mengikuti posisi mouse
        // Di sini, kita menggunakan layar koordinat untuk memposisikan kursor di dunia 2D
        Vector3 screenToWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, cursorOffset));

        // Pastikan posisi Z-nya sesuai dengan kamera atau layer yang kamu gunakan
        screenToWorldPosition.z = 0;  // Pastikan Z-nya 0 untuk objek 2D, atau sesuai dengan layer kamera

        // Menyusun posisi objek cursor agar sesuai dengan mouse
        cursorObject.transform.position = screenToWorldPosition;
    }
}
