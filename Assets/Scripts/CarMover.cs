using UnityEngine;
using Unity.Netcode;
using System.Collections;
using Unity.Netcode.Components;

public class CarMover : NetworkBehaviour
{
    public Transform[] waypoints;
    public float speed = 5f;
    public float waitCrossing = 5f;
    public float resetCar = 1f;

    public Transform[] wheels;
    public float wheelRotationSpeed = 180f;

    private int currentWaypointIndex = 0;

    // Networked version of isWaiting
    private NetworkVariable<bool> isWaiting = new NetworkVariable<bool>(false, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    private bool hasResetOnce = false; // Track if car has reset once
    private bool stopAtCrossing = false; // After first reset, stop at crossing

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
        if (IsServer)
        {
            if (isWaiting.Value || waypoints.Length < 2 || stopAtCrossing) return;

            Transform target = waypoints[currentWaypointIndex];
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            if (transform.position == target.position)
            {
                currentWaypointIndex++;

                if (currentWaypointIndex >= waypoints.Length)
                {
                    StartCoroutine(ResetCar());
                }
                else if (currentWaypointIndex == 2)
                {
                    if (hasResetOnce)
                    {
                        stopAtCrossing = true;
                        return;
                    }

                    StartCoroutine(WaitAtCrossing(waitCrossing));
                }
            }
        }

        // Rotate wheels on all clients, but only if not waiting
        if (!isWaiting.Value && !stopAtCrossing)
        {
            RotateWheels();
        }
    }

    void RotateWheels()
    {
        if (wheels == null || wheels.Length == 0) return;

        float rotationAmount = wheelRotationSpeed * Time.deltaTime;
        foreach (Transform wheel in wheels)
        {
            wheel.Rotate(Vector3.right, rotationAmount, Space.Self);
        }
    }

    IEnumerator WaitAtCrossing(float waitTime)
    {
        isWaiting.Value = true;
        yield return new WaitForSeconds(waitTime);
        isWaiting.Value = false;
    }

    IEnumerator ResetCar()
    {
        isWaiting.Value = true;
        yield return new WaitForSeconds(resetCar);

        if (TryGetComponent<NetworkTransform>(out var netTransform))
        {
            netTransform.Teleport(waypoints[0].position, transform.rotation, transform.localScale);
        }
        else
        {
            transform.position = waypoints[0].position; // Fallback
        }

        currentWaypointIndex = 1;
        isWaiting.Value = false;
        hasResetOnce = true;
    }
}
