using UnityEngine;

[RequireComponent(typeof(XRINetworkPlayer))]
public class XRAvatarVisuals : MonoBehaviour
{
    /// Head Renderers to change rendering mode for local players.
    [Header("Renderer References"), SerializeField, Tooltip("Head Renderers to change rendering mode for local players.")]
    protected Renderer[] m_HeadRends;
    
    /// Reference to the attached XRINetworkPlayerAvatar component.
    protected XRINetworkPlayer m_NetworkPlayerAvatar;
    
    public virtual void Awake()
    {
        if (!TryGetComponent(out m_NetworkPlayerAvatar))
        {
            Utils.LogError("XRAvatarVisuals requires a XRINetworkPlayerAvatar component to be attached to the same GameObject. Disabling this component now.");
            enabled = false;
            return;
        }

        m_NetworkPlayerAvatar.onSpawnedLocal += PlayerSpawnedLocal;
    }
    
    public virtual void OnDestroy()
    {
        m_NetworkPlayerAvatar.onSpawnedLocal -= PlayerSpawnedLocal;
    }
    
    public virtual void PlayerSpawnedLocal()
    {
        int layer = LayerMask.NameToLayer("Mirror");
        foreach (var r in m_HeadRends)
        {
            r.gameObject.layer = layer;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
    }
}
