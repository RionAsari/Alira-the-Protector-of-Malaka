using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    Material mat;
    float distance;

    [Range(0,0.5f)]
    public float speed=0.2f;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        distance += Time.deltaTime*speed;
        mat.SetTextureOffset("_MainTex", Vector2.right * distance);
    }
}
