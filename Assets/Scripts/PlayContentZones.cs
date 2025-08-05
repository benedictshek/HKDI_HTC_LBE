using UnityEngine;

public class PlayContentZones : MonoBehaviour
{
    public GameObject content;

    private void Awake()
    {
        content.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!content.activeSelf)
        {
            content.SetActive(true);
        }

        // Check if this is the local player
        /*if (other.GetComponentInParent<NetworkObject>().IsOwner)
        {
            TryLiftPlayer();
        }*/
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        if (content.activeSelf)
        {
            content.SetActive(false);
        }
    }
}
