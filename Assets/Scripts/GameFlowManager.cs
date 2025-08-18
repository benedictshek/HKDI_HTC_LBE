using Unity.Netcode;
using UnityEngine;

public class GameFlowManager : NetworkBehaviour
{
    private DayNightController dayNightController;
    private PlaneFlyController planeFlyController;
    
    public float initialDelay = 45f;
    
    private float timer;
    private bool hasTriggeredPlane;
    private bool hasTransitioned;

    public GameObject[] cars;
    
    private void Awake()
    {
        dayNightController = GetComponent<DayNightController>();
        planeFlyController = GetComponent<PlaneFlyController>();
    }

    private void Update()
    {
        if (!IsServer || hasTransitioned) return;

        timer += Time.deltaTime;
        
        if (timer >= initialDelay && !hasTriggeredPlane)
        {
            hasTriggeredPlane = true;
            planeFlyController.StartFlight(this);
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
