using UnityEngine;
using System.Collections;

public class CarMover : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 5f;
    private int currentWaypointIndex = 0;
    private bool isWaiting = false;

    public float waitForCross = 5f;
    public float waitForReset = 1f;

    void Start()
    {
        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
            FaceNextWaypoint();
        }
    }

    void Update()
    {
        if (waypoints.Length == 0 || isWaiting) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = targetWaypoint.position - transform.position;
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        // Smooth rotation toward direction
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 5f);
        }

        // Arrived at target waypoint
        if (direction.magnitude < 0.2f)
        {
            currentWaypointIndex++;

            // At the end of the path
            if (currentWaypointIndex >= waypoints.Length)
            {
                StartCoroutine(ResetCar());
            }
            // At pedestrian crossing (waypoint 1)
            else if (currentWaypointIndex == 2) // just reached waypoint 1 (going to 2)
            {
                StartCoroutine(WaitAtCrossing(waitForCross));
            }
        }
    }

    IEnumerator WaitAtCrossing(float waitTime)
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        isWaiting = false;
    }

    IEnumerator ResetCar()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitForReset);

        // Reset to start
        currentWaypointIndex = 1; // next target after reset
        transform.position = waypoints[0].position;
        FaceNextWaypoint();

        isWaiting = false;
    }

    private void FaceNextWaypoint()
    {
        if (waypoints.Length >= 2)
        {
            Vector3 direction = waypoints[1].position - waypoints[0].position;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
}
