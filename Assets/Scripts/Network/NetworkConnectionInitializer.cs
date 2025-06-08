using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkConnectionInitializer : MonoBehaviour
{
    [SerializeField] private float retryInterval = 5.0f; // Seconds between retries
    [SerializeField] private float retryTimeout = 60f; // Total time to retry before giving up
    
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }

        GUILayout.EndArea();
    }
    
    private void StartButtons()
    {
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
    }
    
    private void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
                        NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }
#endif
    
    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("VR device - start as Client");
            StartCoroutine(TryConnectToServer());
        }
    }
    
    private IEnumerator TryConnectToServer()
    {
        float elapsed = 0f;

        while (!NetworkManager.Singleton.IsConnectedClient && elapsed < retryTimeout)
        {
            if (!NetworkManager.Singleton.IsClient)
            {
                Debug.Log("[Client] Attempting to connect to server...");
                NetworkManager.Singleton.StartClient();
            }

            float waitTime = 0f;
            while (waitTime < retryInterval)
            {
                if (NetworkManager.Singleton.IsConnectedClient)
                {
                    Debug.Log("[Client] Successfully connected to server!");
                    yield break;
                }

                waitTime += Time.deltaTime;
                yield return null;
            }

            elapsed += retryInterval;
            Debug.LogWarning("[Client] Failed to connect. Retrying...");
        }

        if (!NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.LogError("[Client] Failed to connect after timeout.");
        }
    }
}
