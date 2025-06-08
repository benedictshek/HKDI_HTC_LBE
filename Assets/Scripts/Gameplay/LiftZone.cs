using Unity.Netcode;
using UnityEngine;

public class LiftZone : MonoBehaviour
{
    [SerializeField] private MovingPlatform platform;
    private float _platformHeightOffset;

    private void Awake()
    {
        _platformHeightOffset = platform.GetComponent<Collider>().bounds.size.y / 2;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Check if this is the local player
        if (other.GetComponentInParent<NetworkObject>().IsOwner)
        {
            TryLiftPlayer();
        }
    }

    private void TryLiftPlayer()
    {
        float platformTopY = platform.transform.position.y + _platformHeightOffset;

        var xrOrigin = XRPlatformFollower.Instance;
        if (xrOrigin != null)
        {
            Vector3 pos = xrOrigin.transform.position;
            xrOrigin.transform.position = new Vector3(pos.x, platformTopY, pos.z);
        }
    }
}
