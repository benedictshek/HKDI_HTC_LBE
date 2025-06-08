using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public class MovingPlatform : NetworkBehaviour
{
    public float moveDistance = 3f;
    public float moveSpeed = 0.5f;
    [SerializeField] private float waitTimeToGoUp = 3f;
    [SerializeField] private float waitTimeOnTop = 10f;

    private Vector3 _originalPosition;
    private Vector3 _targetPosition;
    private bool _isMoving;
    
    private HashSet<ulong> _playersOnPlatform = new HashSet<ulong>();

    private void Start()
    {
        _originalPosition = transform.position;
        _targetPosition = _originalPosition + Vector3.up * moveDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!IsServer) return; // Only the server handles logic
        
        if (other.CompareTag("Player"))
        {
            var networkObject = other.GetComponentInParent<NetworkObject>();
            if (networkObject != null)
            {
                _playersOnPlatform.Add(networkObject.OwnerClientId);
                NotifyClientOnPlatformClientRpc(networkObject.OwnerClientId, true);

                if (!_isMoving)
                    StartCoroutine(MovePlatformRoutine());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(!IsServer) return; // Only the server handles logic
        
        if (other.CompareTag("Player"))
        {
            var networkObject = other.GetComponentInParent<NetworkObject>();
            if (networkObject != null)
            {
                _playersOnPlatform.Remove(networkObject.OwnerClientId);
                NotifyClientOnPlatformClientRpc(networkObject.OwnerClientId, false);
            }
        }
    }
    
    [ClientRpc]
    private void NotifyClientOnPlatformClientRpc(ulong clientId, bool isOnPlatform)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            XRPlatformFollower.Instance.SetOnPlatform(isOnPlatform, this);
        }
    }

    private IEnumerator MovePlatformRoutine()
    {
        _isMoving = true;

        while (_playersOnPlatform.Count > 0)
        {
            // Wait before move up
            yield return new WaitForSeconds(waitTimeToGoUp);

            if (_playersOnPlatform.Count == 0)
            {
                _isMoving = false;
                yield break; // Exit coroutine if no player
            }

            // Move up, Wait on top, Move back down
            yield return StartCoroutine(MoveToPosition(_targetPosition));
            yield return new WaitForSeconds(waitTimeOnTop);
            yield return StartCoroutine(MoveToPosition(_originalPosition));
        }
        
        _isMoving = false;
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target; // Snap to exact position
    }
}
