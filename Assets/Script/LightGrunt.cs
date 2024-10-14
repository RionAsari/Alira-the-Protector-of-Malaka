using UnityEngine;

public class Enemy : MonoBehaviour
{
    private bool isDisabled = false;

    public void Disable()
    {
        isDisabled = true;
        // Add any additional code to visually or behaviorally disable the enemy
    }

    public void Enable()
    {
        isDisabled = false;
        // Add any additional code to visually or behaviorally enable the enemy
    }

    private void Update()
    {
        if (isDisabled)
        {
            return; // Prevent any movement or actions while disabled
        }

        // Enemy behavior here...
    }
}
