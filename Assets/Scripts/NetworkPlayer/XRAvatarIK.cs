using UnityEngine;

public class XRAvatarIK : MonoBehaviour
{
    /// Transform for the Network Player Head.
    [SerializeField, Tooltip("Transform for the Network Player Head.")] Transform m_HeadTransform;
    
    /// Torso Parent Transform.
    [SerializeField, Tooltip("Torso Parent Transform.")] Transform m_TorsoParentTransform;
    
    /// Root of the Head Visuals.
    [SerializeField, Tooltip("Root of the Head Visuals.")] Transform m_HeadVisualsRoot;
    
    /// Neck Transform.
    [SerializeField, Tooltip("Neck Transform.")] Transform m_Neck;
    
    /// Offset to be applied to the head height.
    [SerializeField, Tooltip("Offset to be applied to the head height.")] float m_HeadHeightOffset = .3f;
    
    /// Theshold to where body rotation appoximation is applied.
    [Range(0, 360.0f)]
    [SerializeField, Tooltip("Theshold to where body rotation appoximation is applied.")] float m_RotateThreshold = 25.0f;
    
    /// Speed at which the body rotates.
    [SerializeField, Tooltip("Speed at which the body rotates.")] float m_RotateSpeed = 3.0f;
    
    /// Transform associated with this script.
    Transform m_Transform;
    
    /// Rotation destination for the Y euler value.
    float m_DestinationY;
    
    private void Start()
    {
        m_Transform = GetComponent<Transform>();
        m_DestinationY = m_HeadTransform.transform.eulerAngles.y;
    }
    
    private void Update()
    {
        // Update Head.
        m_HeadVisualsRoot.position = m_HeadTransform.position;
        m_HeadVisualsRoot.position -= m_HeadTransform.up * m_HeadHeightOffset;
        m_Neck.rotation = m_HeadTransform.rotation;

        // Update Body.
        m_Transform.position = m_HeadTransform.position;
        m_TorsoParentTransform.rotation = Quaternion.Slerp(m_TorsoParentTransform.rotation, Quaternion.Euler(new Vector3(0, m_DestinationY, 0)), Time.deltaTime * m_RotateSpeed);

        // Rotate Body if past threshold.
        if (Mathf.Abs(m_TorsoParentTransform.eulerAngles.y - m_HeadTransform.eulerAngles.y) >= m_RotateThreshold)
        {
            m_DestinationY = m_HeadTransform.transform.eulerAngles.y;
        }

        // Update scale.
        m_HeadVisualsRoot.localScale = m_HeadTransform.localScale;
    }
}
