using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlaneFlyController : NetworkBehaviour
{
    public GameObject plane;
    private Vector3 startPos;
    public Transform endTransform;
    public float speed = 15f;
    //public float initialDelay = 15f; // Delay before first flight
    // float resetDelay = 15f;   // Delay after disappearing before reset
    
    private GameFlowManager callbackManager;

    private void Start()
    {
        startPos = plane.transform.position;
        plane.SetActive(false);
    }
    
    public void StartFlight(GameFlowManager manager)
    {
        if (!IsServer) return;

        callbackManager = manager;
        ShowPlaneClientRpc();
        plane.SetActive(true);
        StartCoroutine(PlaneFly());
    }
    
    [ClientRpc]
    private void ShowPlaneClientRpc()
    {
        plane.SetActive(true);
    }

    [ClientRpc]
    private void HidePlaneClientRpc()
    {
        plane.SetActive(false);
    }

    private IEnumerator PlaneFly()
    {
        plane.transform.position = startPos;

        // Move towards the end position
        while (Vector3.Distance(plane.transform.position, endTransform.position) > 0.1f)
        {
            plane.transform.position = Vector3.MoveTowards(plane.transform.position, endTransform.position, speed * Time.deltaTime);
            yield return null; // Wait for next frame
        }

        HidePlaneClientRpc();
        // Hide the plane once it reaches the end
        plane.SetActive(false);

        // Wait before resetting and restarting
        //yield return new WaitForSeconds(resetDelay);
            
        callbackManager?.OnPlaneFlightCompleted();
    }
}
