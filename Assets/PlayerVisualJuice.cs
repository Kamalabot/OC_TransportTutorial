using UnityEngine;
using PrimeTween;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerVisualJuice : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _visualRoot;

    [Header("Squash & Stretch Tuning")]
    [SerializeField] private Vector3 _jumpStretchScale = new Vector3(0.75f, 1.3f, 1f);
    [SerializeField] private Vector3 _landSquashScale = new Vector3(1.3f, 0.7f, 1f);
    [SerializeField] private float _tweenDuration = 0.14f;

    [Header("Movement Tilt")]
    [SerializeField] private float _maxTiltAngle = 8f;
    [SerializeField] private float _tiltSpeed = 12f;

    private Rigidbody2D _rb;
    private Tween _scaleTween;
    private bool _wasRising;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        if (_visualRoot == null)
        {
            Debug.LogError("[PlayerVisualJuice] Assign the child Visuals transform.");
            enabled = false;
        }
    }

    private void Update()
    {
        float vy = _rb.velocity.y;
        float vx = _rb.velocity.x;

        // Detect upward jump impulse
        if (!_wasRising && vy > 2.5f)
        {
            _wasRising = true;
            TriggerScalePunch(_jumpStretchScale);
        }
        // Detect landing (vertical velocity drops near zero after falling)
        else if (_wasRising && Mathf.Abs(vy) < 0.05f)
        {
            _wasRising = false;
            TriggerScalePunch(_landSquashScale);
        }

        // Subtle lean into horizontal velocity
        float targetTilt = -Mathf.Clamp(vx / 10f, -1f, 1f) * _maxTiltAngle;
        Quaternion targetRot = Quaternion.Euler(0f, 0f, targetTilt);
        _visualRoot.localRotation = Quaternion.Slerp(_visualRoot.localRotation, targetRot, Time.deltaTime * _tiltSpeed);
    }

    public void TriggerScalePunch(Vector3 targetScale)
    {
        _scaleTween.Stop();
        _visualRoot.localScale = targetScale;
        _scaleTween = Tween.Scale(_visualRoot, Vector3.one, _tweenDuration, Ease.OutBack);
    }

    private void OnDestroy()
    {
        _scaleTween.Stop();
    }
}