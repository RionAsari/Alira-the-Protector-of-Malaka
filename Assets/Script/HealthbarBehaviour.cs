using UnityEngine;
using UnityEngine.UI;

public class HealthbarBehaviour : MonoBehaviour
{
    public Slider Slider;          // Reference to the health slider
    public Color Low;             // Color for low health
    public Color Medium;          // Color for medium health
    public Color High;            // Color for high health
    public Vector3 Offset = new Vector3(0, 2f, 0); // Sesuaikan nilai Y sesuai kebutuhan

    private void Start()
    {
        // Initialize the slider as inactive at the start
        Slider.gameObject.SetActive(false);
    }

    public void SetHealth(float health, float maxHealth)
    {
        // Always show the slider if health is not full
        Slider.gameObject.SetActive(true); 

        Debug.Log($"Health set to: {health}, Max Health: {maxHealth}"); // Debug log
        Slider.value = health;                             // Set current health
        Slider.maxValue = maxHealth;                       // Set max health

        // Change color based on the health percentage
        float healthPercentage = health / maxHealth;
        if (health == maxHealth) // Full health (100%)
        {
            Slider.fillRect.GetComponentInChildren<Image>().color = Color.green; // Set to green
        }
        else if (healthPercentage <= 0.8f && healthPercentage > 0.4f) // 80%-40%
        {
            Slider.fillRect.GetComponentInChildren<Image>().color = Color.yellow; // Set to yellow
        }
        else if (health > 0) // 39%-1
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
