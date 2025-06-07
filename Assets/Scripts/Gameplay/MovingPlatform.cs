using UnityEngine;
using System.Collections;
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

    private int _playersOnPlatform;
    private bool IsAnyPlayerOnPlatform() => _playersOnPlatform > 0;

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
            Debug.Log("Player entered platform");
            _playersOnPlatform += 1;

            // Only start coroutine if not already moving
            if (!_isMoving)
            {
                StartCoroutine(MovePlatformRoutine());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(!IsServer) return; // Only the server handles logic
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited platform");
            _playersOnPlatform -= 1;
        }
    }

    private IEnumerator MovePlatformRoutine()
    {
        _isMoving = true;

        while (true)
        {
            // Wait on bottom
            yield return new WaitForSeconds(waitTimeToGoUp);

            if (!IsAnyPlayerOnPlatform())
            {
                _isMoving = false;
                yield break; // Exit coroutine if no player
            }

            // Move up
            yield return StartCoroutine(MoveToPosition(_targetPosition));

            // Wait on top
            yield return new WaitForSeconds(waitTimeOnTop);

            // Move down
            yield return StartCoroutine(MoveToPosition(_originalPosition));

            // Check if player still on it
            if (!IsAnyPlayerOnPlatform())
            {
                _isMoving = false;
                yield break; // Exit if no player to start loop again
            }
        }
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
