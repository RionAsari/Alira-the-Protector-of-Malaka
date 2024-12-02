using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class CreditSceneController : MonoBehaviour
{
    public PlayableDirector playableDirector;

    void Start()
    {
        // Menambahkan listener ketika animasi selesai
        playableDirector.stopped += OnAnimationFinished;
    }

    // Fungsi ini akan dipanggil saat animasi selesai
    private void OnAnimationFinished(PlayableDirector director)
    {
        // Pindah ke scene Main Menu setelah animasi selesai
        SceneManager.LoadScene("Main Menu");
    }
}
