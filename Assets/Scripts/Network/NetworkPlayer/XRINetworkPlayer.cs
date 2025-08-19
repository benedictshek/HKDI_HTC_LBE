using System;
using Unity.Netcode;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class XRINetworkPlayer : NetworkBehaviour
{
    [Header("Avatar Transform References"), Tooltip("Assign to local avatar transform.")]
    /// Non-Local player transforms.
    public Transform head;
    
    /// Action called when the Local Player is finished spawning in.
    public Action onSpawnedLocal;
    
    /// Reference to the local player XR Origin
    protected XROrigin m_XROrigin;
    
    /// Internal references to the Local Player Transforms.
    protected Transform m_HeadOrigin;
    
    private bool isInitialized = false;
    
    private void OnEnable()
    {
        // Subscribe to scene load complete event
        NetworkManager.SceneManager.OnLoadComplete += OnSceneLoadComplete;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        NetworkManager.SceneManager.OnLoadComplete -= OnSceneLoadComplete;
    }
    
    private void OnSceneLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        // Only reinitialize for the local player
        if (!IsOwner || clientId != NetworkManager.LocalClientId)
            return;

        Debug.Log($"[XRINetworkPlayer] Scene '{sceneName}' loaded. Reinitializing XR references.");
        SetupXRReferences();
    }
    
    private void SetupXRReferences()
    {
        // Try to find the XR Origin
        m_XROrigin = FindFirstObjectByType<XROrigin>();
        if (m_XROrigin != null)
        {
            m_HeadOrigin = m_XROrigin.Camera.transform;
            isInitialized = true;
            
            SetupLocalPlayer();
        }
        else
        {
            Debug.LogWarning("[XRINetworkPlayer] XR Origin not found in scene.");
            isInitialized = false;
        }
    }
    
    protected virtual void LateUpdate()
    {
        if (!IsOwner || !isInitialized) return;

        // Set transforms to be replicated with ClientNetworkTransforms
        head.SetPositionAndRotation(m_HeadOrigin.position, m_HeadOrigin.rotation);
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            SetupXRReferences();
            // Get Origin and set head.
            /*m_XROrigin = FindFirstObjectByType<XROrigin>();
            if (m_XROrigin != null)
            {
                m_HeadOrigin = m_XROrigin.Camera.transform;
            }
            else
            {
                Utils.Log("No XR Rig Available", 1);
            }
            
            SetupLocalPlayer();*/
        }
    }
    
    /// <remarks>Only called on the Local Player.</remarks>
    protected virtual void SetupLocalPlayer()
    {
        onSpawnedLocal?.Invoke();
    }
}
