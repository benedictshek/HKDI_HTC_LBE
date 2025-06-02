using Unity.Netcode;
using UnityEngine;

public class StartServerClient : MonoBehaviour
{
    private NetworkManager m_NetworkManager;

    private void Awake()
    {
        m_NetworkManager = GetComponent<NetworkManager>();
    }
    
    private void Start()
    {
        if (Application.isEditor)
        {
            m_NetworkManager.StartServer();
        }
        else
        {
            m_NetworkManager.StartClient();
        }
    }
}
