using UnityEngine;

public class VisibilityController : MonoBehaviour
{
public Camera mainCamera; // Reference to the main camera
public GameObject targetObject; // The GameObject to show/hide
public GameObject switchObject; // The GameObject to show/hide
public float visibilityDistance = 5f; // Distance to check for visibility
public float angleThreshold = 45f; // Angle in degrees to determine visibility

private void Update()
{
if (mainCamera == null)
mainCamera = Camera.main; // Fallback to main camera

if (targetObject == null)
return; // Exit if no target object is assigned

Vector3 directionToTarget = targetObject.transform.position - mainCamera.transform.position;
float distanceToCamera = directionToTarget.magnitude;

// Calculate the angle between the camera's forward direction and the direction to the target object
float angle = Vector3.Angle(mainCamera.transform.forward, directionToTarget.normalized);
//Debug.Log("Calculated Angle to Target: " + angle); // Print the angle to the console

// Show or hide the GameObject based on distance and angle
if (distanceToCamera < visibilityDistance)
{
	if ((angle < angleThreshold) || (angle > angleThreshold + 90f) ) {
		targetObject.SetActive(true); // Show the GameObject
		switchObject.SetActive(false); // Show the GameObject
	}
	else
	{
		targetObject.SetActive(false); // Hide the GameObject
		switchObject.SetActive(true); // Show the GameObject
	}
}
else
{
	targetObject.SetActive(false); // Hide the GameObject
	switchObject.SetActive(false); // Hide the GameObject
}

}
}