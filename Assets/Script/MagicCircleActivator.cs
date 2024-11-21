using UnityEngine;

public class MagicCircleActivator : MonoBehaviour
{
    [SerializeField] private GameObject magicCirclePrefab;  // Prefab magic circle yang ingin diaktifkan

    private void Start()
    {
        // Pastikan magic circle dimulai dalam keadaan tidak aktif
        magicCirclePrefab.SetActive(false);
    }

    private void Update()
    {
        // Cek jika semua musuh sudah dikalahkan
        if (LightGrunt.activeEnemies <= 0 && MiddleBot.activeEnemies <= 0)
        {
            // Aktifkan magic circle setelah semua musuh kalah, jika belum aktif
            if (!magicCirclePrefab.activeSelf)
            {
                magicCirclePrefab.SetActive(true);  // Aktifkan magic circle
                Debug.Log("All enemies defeated. Magic circle activated.");
            }
        }
        else
        {
            // Jika masih ada musuh, pastikan magic circle tetap tidak aktif
            if (magicCirclePrefab.activeSelf)
            {
                magicCirclePrefab.SetActive(false);  // Nonaktifkan magic circle
                Debug.Log("Enemies are still present. Magic circle deactivated.");
            }
        }
    }
}
