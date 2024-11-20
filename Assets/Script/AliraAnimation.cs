using UnityEngine;

public class Alira : MonoBehaviour
{
    private Animator animator;
    private bool isMoving;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Periksa apakah player sedang bergerak
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        isMoving = horizontal != 0 || vertical != 0;

        // Atur parameter animasi berdasarkan keadaan
        animator.SetBool("isIdle", !isMoving);
    }
}
