using UnityEngine;
using UnityEngine.AI;

public class McDullWalk : MonoBehaviour
{
    public Transform[] waypoints; // Assign 4 waypoints in the Inspector
    public float arrivalThreshold = 0.5f; // Distance to consider as "arrived"
    
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        // Check if the agent is close enough to the current target
        if (!agent.pathPending && agent.remainingDistance <= arrivalThreshold)
        {
            // Move to the next waypoint in the square path
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }
}
