using System.Collections;
using UnityEngine;

public class RandomPositionChanger : MonoBehaviour
{
public float changeInterval = 2f; // Time in seconds between position changes
public Vector2 xRange = new Vector2(-5f, 5f); // Range for X axis
public Vector2 yRange = new Vector2(0f, 0f); // Range for Y axis (fixed if same value)
public Vector2 zRange = new Vector2(-5f, 5f); // Range for Z axis

private void Start()
{
// Start the coroutine to change position
StartCoroutine(ChangePosition());
}

private IEnumerator ChangePosition()
{
while (true)
{
// Generate a random position within specified ranges
Vector3 randomPosition = new Vector3(
Random.Range(xRange.x, xRange.y), // Random X within xRange
Random.Range(yRange.x, yRange.y), // Random Y within yRange
Random.Range(zRange.x, zRange.y) // Random Z within zRange
);

// Move the GameObject to the new position
transform.position = randomPosition;

// Wait for the specified interval
yield return new WaitForSeconds(changeInterval);
}
}
}
