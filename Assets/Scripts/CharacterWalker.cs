using UnityEngine;

public class CharacterWalker : MonoBehaviour
{
    public Vector3 walkDirection = Vector3.forward;  // Direction to move
    public float walkSpeed = 2.0f;      // Movement speed

    void Update()
    {
        // Move in the specified direction
        transform.position += walkDirection.normalized * walkSpeed * Time.deltaTime;
    }
}
