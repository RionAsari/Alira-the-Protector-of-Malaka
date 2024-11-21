using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    Transform cam;
    Vector3 camStartPos;
    float distance;

    GameObject[] backgrounds;
    Material[] mat;
    float[] backspeed;

    [Range(0.01f, 0.05f)]
    public float parallaxSpeed; // Kecepatan umum parallax

    [Header("Kecepatan untuk setiap layer")]
    public float[] layerSpeeds;  // Kecepatan spesifik untuk setiap layer background

    float farthestBack; // Declare the farthestBack variable

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;
        camStartPos = cam.position;

        int backCount = transform.childCount;
        mat = new Material[backCount];
        backgrounds = new GameObject[backCount];
        backspeed = new float[backCount]; // Array untuk menyimpan kecepatan setiap layer

        for (int i = 0; i < backCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            mat[i] = backgrounds[i].GetComponent<Renderer>().material;
        }

        // Menentukan kecepatan layer secara otomatis jika belum diatur
        if (layerSpeeds.Length != backCount)
        {
            layerSpeeds = new float[backCount];
            for (int i = 0; i < backCount; i++)
            {
                layerSpeeds[i] = 1.0f / (i + 1);  // Set kecepatan berdasarkan urutan layer, lebih besar index = lebih lambat
            }
        }

        BackSpeedCalculate(backCount);
    }

    // Update is called once per frame
    void BackSpeedCalculate(int backCount)
    {
        farthestBack = 0f; // Initialize farthestBack

        for (int i = 0; i < backCount; i++)
        {
            if ((backgrounds[i].transform.position.z - cam.position.z) > farthestBack)
            {
                farthestBack = backgrounds[i].transform.position.z - cam.position.z;
            }
        }

        for (int i = 0; i < backCount; i++)
        {
            backspeed[i] = 1 - (backgrounds[i].transform.position.z - cam.position.z) / farthestBack;
        }
    }

    private void LateUpdate()
    {
        // Menghitung pergerakan kamera
        distance = cam.position.x - camStartPos.x;

        // Mengupdate posisi latar belakang
        transform.position = new Vector3(cam.position.x, transform.position.y, 0);

        // Menggerakkan latar belakang dengan kecepatan berbeda berdasarkan layer
        for (int i = 0; i < backgrounds.Length; i++)
        {
            // Kecepatan setiap background berdasarkan layerSpeeds dan kecepatan parallax umum
            float speed = backspeed[i] * parallaxSpeed * layerSpeeds[i];
            mat[i].SetTextureOffset("_MainTex", new Vector2(distance * speed, 0));
        }
    }
}
