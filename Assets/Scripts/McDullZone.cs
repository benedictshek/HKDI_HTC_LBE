using Unity.Netcode;
using UnityEngine;

public class McDullZone : NetworkBehaviour
{
    public GameObject McDull;
    public GameFlowManager gameFlowManager;
    public MeshRenderer McDullRenderer;

    private bool hasTirggered = true;

    private void Start()
    {
        McDull.SetActive(false);
        DeActiveZone();
    }

    private void DeActiveZone()
    {
        McDullRenderer.enabled = false;
    }

    public void ActiveZone()
    {
        hasTirggered = false;
        McDullRenderer.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<NetworkObject>().IsOwner && !hasTirggered)
        {
            hasTirggered = true;
            ShowMcDullOnServer();
            //gameFlowManager.TriggerDayNightTransition();
        }
    }
    
    private void ShowMcDullOnServer()
    {
        // Activate the McDull GameObject
        McDull.SetActive(true);
        
        // Notify all clients to activate McDull
        ShowMcDullOnClientRpc();
    }

    [ClientRpc]
    private void ShowMcDullOnClientRpc()
    {
        McDull.SetActive(true);
    }
}
