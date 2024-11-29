using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomCursorObject : MonoBehaviour
{
    public GameObject cursorObject;          // GameObject untuk custom cursor
    public float cursorOffset = 10f;        // Offset posisi kursor
    public string mainMenuSceneName = "MainMenu";  // Nama scene untuk Main Menu
    public GameObject pauseMenu;            // Referensi ke GameObject Pause Menu
    public GameObject gameOverMenu;         // Referensi ke GameObject Game Over Menu
    public GameObject settingsPanel;        // Referensi ke GameObject Settings Panel

    void Start()
    {
        UpdateCursorVisibility(); // Set awal kursor
    }

    void Update()
    {
        // Periksa apakah kursor perlu diubah
        if (ShouldUseDefaultCursor())
        {
            Cursor.visible = true;
            cursorObject.SetActive(false);
        }
        else
        {
            Cursor.visible = false;
            cursorObject.SetActive(true);

            // Ambil posisi mouse di layar
            Vector3 mousePosition = Input.mousePosition;

            // Mengubah posisi objek cursor agar mengikuti posisi mouse
            Vector3 screenToWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, cursorOffset));

            // Pastikan posisi Z-nya sesuai dengan kamera atau layer yang kamu gunakan
            screenToWorldPosition.z = 0; // Pastikan Z-nya 0 untuk objek 2D
            cursorObject.transform.position = screenToWorldPosition;
        }
    }

    private bool ShouldUseDefaultCursor()
    {
        // Periksa apakah scene saat ini adalah Main Menu
        if (SceneManager.GetActiveScene().name == mainMenuSceneName)
            return true;

        // Periksa apakah Pause Menu, Game Over Menu, atau Settings Panel aktif
        if (pauseMenu != null && pauseMenu.activeSelf)
            return true;

        if (gameOverMenu != null && gameOverMenu.activeSelf)
            return true;

        if (settingsPanel != null && settingsPanel.activeSelf)
            return true;

        return false;
    }

    private void UpdateCursorVisibility()
    {
        if (ShouldUseDefaultCursor())
        {
            Cursor.visible = true;
            cursorObject.SetActive(false);
        }
        else
        {
            Cursor.visible = false;
            cursorObject.SetActive(true);
        }
    }
}
