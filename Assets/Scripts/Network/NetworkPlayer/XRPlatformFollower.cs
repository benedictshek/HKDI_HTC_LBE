using UnityEngine;

public class XRPlatformFollower : MonoBehaviour
{
    public static XRPlatformFollower Instance;

    private bool _isOnPlatform;
    private Transform _platformTransform;
    private Vector3 _lastPlatformPosition;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetOnPlatform(bool status, MovingPlatform platform)
    {
        _isOnPlatform = status;

        if (status)
        {
            _platformTransform = platform.transform;
            _lastPlatformPosition = _platformTransform.position;
        }
        else
        {
            // Player fell off platform, reset to base level (e.g., y = 0)
            _platformTransform = null;

            Vector3 pos = transform.position;
            pos.y = 0f;
            transform.position = pos;
        }
    }

    private void Update()
    {
        if (!_isOnPlatform || _platformTransform == null)
            return;

        Vector3 platformDelta = _platformTransform.position - _lastPlatformPosition;
        transform.position += platformDelta;
        _lastPlatformPosition = _platformTransform.position;
    }
}
