using System.Collections;
using UnityEngine;

public class PlaneFlyController : MonoBehaviour
{
    public GameObject plane;
    private Vector3 startPos;
    public Transform endTransform;
    public float speed = 20f;        // Speed of the plane (units per second)
    public float initialDelay = 15f; // Delay before first flight
    public float resetDelay = 15f;   // Delay after disappearing before reset

    private void Start()
    {
        // Hide the plane at the start
        plane.SetActive(false);
        
        startPos = plane.transform.position;
        
        // Start the flight loop
        StartCoroutine(FlightLoop());
    }

    private IEnumerator FlightLoop()
    {
        // Initial delay before the first flight
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            // Reset position to start, face the end, and show the plane
            plane.transform.position = startPos;
            plane.SetActive(true);

            // Move towards the end position
            while (Vector3.Distance(plane.transform.position, endTransform.position) > 0.1f)
            {
                plane.transform.position = Vector3.MoveTowards(plane.transform.position, endTransform.position, speed * Time.deltaTime);
                yield return null; // Wait for next frame
            }

            // Hide the plane once it reaches the end
            plane.SetActive(false);

            // Wait before resetting and restarting
            yield return new WaitForSeconds(resetDelay);
        }
    }
}
