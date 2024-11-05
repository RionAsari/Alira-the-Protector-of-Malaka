using UnityEngine;
using UnityEngine.UI;

public class HealthBarMiddleBot : MonoBehaviour
{
    public Slider Slider;          // Reference to the health slider
    public Color Low;             // Color for low health (optional)
    public Color Medium;          // Color for medium health (optional)
    public Color High;            // Color for high health (optional)
    public Vector3 Offset = new Vector3(0, 2f, 0); // Adjust Y value as needed

    private void Start()
    {
        // Optional: If you want the slider to start inactive, remove the following line.
        // Slider.gameObject.SetActive(false); 
    }

    public void SetHealth(float health, float maxHealth)
    {
        // Set max health before current health to prevent visual glitches
        Slider.maxValue = maxHealth;
        Slider.value = health;  // Set current health

        // Activate the slider after setting health values
        Slider.gameObject.SetActive(true);

        // Change color based on the health value ranges
        if (health >= 350f) // 500f - 350f
        {
            Slider.fillRect.GetComponentInChildren<Image>().color = Color.green; // Set to green
        }
        else if (health >= 150f) // 349f - 150f
        {
            Slider.fillRect.GetComponentInChildren<Image>().color = Color.yellow; // Set to yellow
        }
        else if (health > 0) // 149f - 0f
        {
            Slider.fillRect.GetComponentInChildren<Image>().color = Color.red; // Set to red
        }
        else // Health is zero
        {
            Destroy(Slider.gameObject); // Destroy the slider if health is zero
        }
    }

    // New method to update the position of the health bar
    public void UpdatePosition(Vector3 enemyPosition)
    {
        // Update the position of the health bar to follow the enemy
        Slider.transform.position = Camera.main.WorldToScreenPoint(enemyPosition + Offset);
    }

    void Update()
    {
        // Uncomment the following line if you want to continuously update the position of the health bar
        // UpdatePosition(transform.parent.position); // Assuming this script is attached to the health bar UI object
    }
}
