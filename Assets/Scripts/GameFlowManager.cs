using Unity.Netcode;
using UnityEngine;

public class GameFlowManager : NetworkBehaviour
{
    private DayNightController dayNightController;
    private PlaneFlyController planeFlyController;
    
    public float rooftopDuration = 30f;
    public float planeDelay = 45f;
    
    private float timer;
    private bool hasTriggeredPlane;
    private bool hasTransitioned;
    private bool hasMovedToGround;

    public GameObject[] cars;
    
    [Header("Player XR Origin")]
    public GameObject xrOrigin; // Assign the player's XROrigin in the editor
    
    private void Awake()
    {
        dayNightController = GetComponent<DayNightController>();
        planeFlyController = GetComponent<PlaneFlyController>();
    }

    private void Update()
    {
        if (!IsServer || hasTransitioned) return;

        timer += Time.deltaTime;
        
        // Phase 1 → Phase 2: Move to ground after rooftopDuration
        if (timer >= rooftopDuration && !hasMovedToGround)
        {
            hasMovedToGround = true;
            MovePlayerToGround();
        }
        
        if (timer >= planeDelay + rooftopDuration && !hasTriggeredPlane && hasMovedToGround)
        {
            hasTriggeredPlane = true;
            planeFlyController.StartFlight(this);
        }
    }
    
    private void MovePlayerToGround()
    {
        if (xrOrigin != null)
        {
            xrOrigin.transform.position = Vector3.zero;
            MovePlayerToGroundClientRpc();
        }
    }

    [ClientRpc]
    private void MovePlayerToGroundClientRpc()
    {
        if (xrOrigin != null)
        {
            xrOrigin.transform.position = Vector3.zero;
        }
    }
    
    // Called by PlaneFlyController when plane flight completes
    public void OnPlaneFlightCompleted()
    {
        if (hasTransitioned) return;

        bool nextIsNight = !dayNightController.IsNight();
        dayNightController.TriggerTransition(nextIsNight);
        hasTransitioned = true;

        if (IsServer)
        {
            foreach (GameObject car in cars)
            {
                car.SetActive(false);
            }
        }
        HideCarsClientRpc(); // Hide on all clients
    }
    
    // Hides cars on all clients
    [ClientRpc]
    private void HideCarsClientRpc()
    {
        foreach (GameObject car in cars)
        {
            car.SetActive(false);
        }
    }
}
