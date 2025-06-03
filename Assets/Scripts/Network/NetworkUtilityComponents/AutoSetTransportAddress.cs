using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Transports.UTP;

public class AutoSetTransportAddress : MonoBehaviour
{
    void Awake()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            NetworkUtils.GetLocalIPAddress(),  // Get the IP address
            (ushort)7777,
            "0.0.0.0"
        );
    }
}
