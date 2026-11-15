using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform _target;

    [Header("Framing & Smoothing")]
    [SerializeField] private Vector3 _offset = new Vector3(0f, 1.5f, -10f);
    [SerializeField] private float _smoothTime = 0.15f;
    [SerializeField] private float _lookAheadX = 1.2f;

    private Vector3 _currentVelocity;
    private Rigidbody2D _targetRb;

    private void Awake()
    {
        if (_target == null)
        {
            Debug.LogError("[CameraFollow2D] Target transform is not assigned.");
            enabled = false;
            return;
        }

        _target.TryGetComponent(out _targetRb);
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        // Subtle horizontal look-ahead based on current movement direction
        float lookAhead = 0f;
        if (_targetRb != null && Mathf.Abs(_targetRb.velocity.x) > 0.1f)
        {
            lookAhead = Mathf.Sign(_targetRb.velocity.x) * _lookAheadX;
        }

        Vector3 desiredPosition = _target.position + _offset + new Vector3(lookAhead, 0f, 0f);
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, _smoothTime);
    }
}