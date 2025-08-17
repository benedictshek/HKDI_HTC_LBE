using Unity.Netcode;
using UnityEngine;

public class GameFlowManager : NetworkBehaviour
{
    private DayNightController dayNightController;
    
    public float interval = 45f;
    private float timer;
    private bool hasTransitioned;
    
    private void Awake()
    {
        dayNightController = GetComponent<DayNightController>();
    }

    private void Update()
    {
        if (!IsServer || !dayNightController || hasTransitioned) return;

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;

            // Trigger the day night transition
            bool nextIsNight = !dayNightController.IsNight(); // Add helper method if needed
            dayNightController.TriggerTransition(nextIsNight);
            hasTransitioned = true; // Set the flag to true after transition
        }
    }
}
