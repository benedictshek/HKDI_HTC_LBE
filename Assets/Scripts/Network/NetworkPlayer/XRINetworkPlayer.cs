using System;
using Unity.Netcode;
using Unity.XR.CoreUtils;
using UnityEngine;

public class XRINetworkPlayer : NetworkBehaviour
{
    [Header("Avatar Transform References"), Tooltip("Assign to local avatar transform.")]
    /// Non-Local player transforms.
    public Transform head;
    
    public CapsuleCollider collider;
    
    /// Action called when the Local Player is finished spawning in.
    public Action onSpawnedLocal;
    
    /// Reference to the local player XR Origin
    protected XROrigin m_XROrigin;
    
    /// Internal references to the Local Player Transforms.
    protected Transform m_HeadOrigin;
    
    protected virtual void LateUpdate()
    {
        if (!IsOwner) return;

        // Set transforms to be replicated with ClientNetworkTransforms
        head.SetPositionAndRotation(m_HeadOrigin.position, m_HeadOrigin.rotation);
        
        var height = m_HeadOrigin.position.y;
        collider.height = height;
        collider.transform.position = m_HeadOrigin.position - Vector3.up * height / 2;
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            // Get Origin and set head.
            m_XROrigin = FindFirstObjectByType<XROrigin>();
            if (m_XROrigin != null)
            {
                m_HeadOrigin = m_XROrigin.Camera.transform;
            }
            else
            {
                Utils.Log("No XR Rig Available", 1);
            }
            
            SetupLocalPlayer();
        }
    }
    
    /// <remarks>Only called on the Local Player.</remarks>
    protected virtual void SetupLocalPlayer()
    {
        onSpawnedLocal?.Invoke();
    }
}
