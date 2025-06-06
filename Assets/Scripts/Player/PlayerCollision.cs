using Unity.XR.CoreUtils;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private XROrigin _xrOrigin;
    private Transform _playerHead;
    
    private CapsuleCollider _playerCollider;

    private void Awake()
    {
        _xrOrigin = GetComponentInParent<XROrigin>();
        _playerCollider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        _playerHead = _xrOrigin.Camera.transform;
    }

    private void LateUpdate()
    {
        var height = _playerHead.position.y - _xrOrigin.transform.position.y;
        _playerCollider.height = height;
        transform.position = _playerHead.position - Vector3.up * height / 2;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lift"))
        {
            Debug.Log("Player stepped on the lift.");
            AdjustPlayerHeight(other.transform.position.y);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Lift"))
        {
            Debug.Log("Player left the lift.");
            AdjustPlayerHeight(0);
        }
    }

    private void AdjustPlayerHeight(float stepHeight)
    {
        _xrOrigin.transform.position = new Vector3(_xrOrigin.transform.position.x, stepHeight, _xrOrigin.transform.position.z);
    }
}
