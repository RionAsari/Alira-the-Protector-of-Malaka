using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;
    private Transform playerTransform; // Reference to the player transform
    public GameObject bow;
    public Transform bowTransform;
    public bool canFire;
    private float timer;
    public float timeBetweenFiring;
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        playerTransform = transform.parent; // Assuming the bow is a child of the player
    }

    void Update()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mousePos - transform.position;

        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        // Adjust rotation based on the player's facing direction
        if (playerTransform.localScale.x < 0) // If player is facing left
        {
            rotZ += 180; // Flip the rotation
        }

        transform.rotation = Quaternion.Euler(0, 0, rotZ);
        if (!canFire)
        {
          timer += Time.deltaTime;
          if(timer > timeBetweenFiring)
          {
            canFire = true;
            timer = 0;
          }
        }

        if (Input.GetMouseButton(0) && canFire)
        {
          canFire = false;
          Instantiate(bow, bowTransform.position, Quaternion.identity);
        }
    }
}
