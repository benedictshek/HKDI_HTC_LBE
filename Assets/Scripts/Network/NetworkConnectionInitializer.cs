using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkConnectionInitializer : MonoBehaviour
{
    [SerializeField] private float retryInterval = 5.0f; // Seconds between retries
    [SerializeField] private float retryTimeout = 60f; // Total time to retry before giving up
    
    private void Start()
    {
        if (Application.isEditor)
        {
            //Debug.Log("Editor - start as Host for testing");
            //NetworkManager.Singleton.StartHost();
            Debug.Log("Editor - start as Server");
            NetworkManager.Singleton.StartServer();
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("VR device - start as Client");
            StartCoroutine(TryConnectToServer());
        }
        else
        {
            Debug.Log("PC build - start as Server");
            NetworkManager.Singleton.StartServer();
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
