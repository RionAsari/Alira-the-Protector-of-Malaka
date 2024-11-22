using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class CutsceneHandler : MonoBehaviour
{
    public PlayableDirector playableDirector; // Drag Timeline here from the Inspector

    void Start()
    {
        // Pastikan Timeline di-start di awal
        playableDirector.Play();
    }

    void Update()
    {
        // Cek jika Timeline sudah selesai
        if (playableDirector.state == PlayState.Paused && !SceneManager.GetSceneByName("CutScene").isLoaded)
        {
            // Pindah ke scene CutScene
            SceneManager.LoadScene("CutScene");
        }
    }
}
