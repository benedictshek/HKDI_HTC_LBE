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
        var height = _playerHead.position.y -  _xrOrigin.transform.position.y;
        _playerCollider.height = height;
        transform.position = _playerHead.position - Vector3.up * height / 2;
    }
}
