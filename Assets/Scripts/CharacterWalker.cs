using UnityEngine;

public class CharacterWalker : MonoBehaviour
{
    public Transform target;                 // Target position to walk to
    public float walkSpeed = 2f;             // Movement speed
    public float stopDistance = 0.1f;        // How close before stopping

    private bool isWalking = true;

    void Update()
    {
        if (isWalking && target != null)
        {
            Vector3 direction = target.position - transform.position;
            direction.y = 0f; // Ignore vertical difference

            // Check distance to target
            if (direction.magnitude > stopDistance)
            {
                // Move toward target
                Vector3 move = direction.normalized * walkSpeed * Time.deltaTime;
                transform.position += move;
            }
            else
            {
                // Stop walking
                isWalking = false;
            }
        }
    }
}
