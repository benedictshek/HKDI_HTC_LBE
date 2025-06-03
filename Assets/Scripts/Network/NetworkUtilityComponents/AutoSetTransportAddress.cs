using UnityEngine;
using Unity.Netcode.Transports.UTP;

public class AutoSetTransportAddress : MonoBehaviour
{
    void Awake()
    {
        var transport = GetComponent<UnityTransport>();
        if (transport != null)
        {
            string localIP = NetworkUtils.GetLocalIPAddress();
            transport.ConnectionData.Address = localIP;
            Debug.Log($"[Netcode] Auto-set transport address to: {localIP}");
        }
    }
}
