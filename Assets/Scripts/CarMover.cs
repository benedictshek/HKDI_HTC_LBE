using UnityEngine;
using System.Collections;

public class CarMover : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 5f;
    public float waitCrossing = 5f;
    public float resetCar = 1f;

    public Transform[] wheels;
    public float wheelRotationSpeed = 180f;

    private int currentWaypointIndex = 0;
    private bool isWaiting = false;

    void Start()
    {
        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
            currentWaypointIndex = 1;
        }
    }

    void Update()
    {
        if (isWaiting || waypoints.Length < 2) return;

        Transform target = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        RotateWheels();

        // Reached target waypoint
        if (transform.position == target.position)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                StartCoroutine(ResetCar());
            }
            else if (currentWaypointIndex == 2) // Wait at waypoint 1
            {
                StartCoroutine(WaitAtCrossing(waitCrossing));
            }
        }
    }

    void RotateWheels()
    {
        if (wheels == null || wheels.Length == 0 || isWaiting) return;

        float rotationAmount = wheelRotationSpeed * Time.deltaTime;
        foreach (Transform wheel in wheels)
        {
            wheel.Rotate(Vector3.right, rotationAmount, Space.Self);
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
        yield return new WaitForSeconds(resetCar);

        transform.position = waypoints[0].position;
        currentWaypointIndex = 1;

        isWaiting = false;
    }
}
