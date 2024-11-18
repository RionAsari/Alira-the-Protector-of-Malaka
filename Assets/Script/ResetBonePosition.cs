using UnityEngine;

public class SyncBoneWithCharacter : MonoBehaviour
{
    public Transform characterTransform; // Transform dari karakter
    public Transform boneTransform;      // Transform dari bone yang ingin disinkronkan

    void Start()
    {
        // Menyinkronkan posisi awal karakter dan bone
        if (characterTransform != null && boneTransform != null)
        {
            boneTransform.position = characterTransform.position;
            boneTransform.rotation = characterTransform.rotation;
            boneTransform.localScale = characterTransform.localScale;
        }
    }

    void Update()
    {
        // Pastikan posisi bone tetap sama dengan karakter setiap frame
        if (characterTransform != null && boneTransform != null)
        {
            boneTransform.position = characterTransform.position;
            boneTransform.rotation = characterTransform.rotation;
            boneTransform.localScale = characterTransform.localScale;
        }
    }
}
