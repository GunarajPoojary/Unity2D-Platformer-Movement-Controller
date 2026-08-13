using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(PlayerInput), typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private const float MOVEMENT_THRESHOLD = 0.01f;

    [SerializeField] private PlayerMovementStats _movementStats;
    [SerializeField] private float _groundCheckDistance;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private LayerMask _groundLayer;

#if UNITY_EDITOR
    [Header("Debugging")]
    [SerializeField] private TMP_Text _gravityLabel;
    [SerializeField] private TMP_Text _initialJumpVelLabel;
    [SerializeField] private TMP_Text _verticalVelLabel;
    [SerializeField] private TMP_Text _horizontalVelLabel;
#endif

    private Rigidbody2D _rb;
    private float _gravity;
    private float _initialJumpVelocity; // velocity required to reach jump apex
    private PlayerInput _input;

    private bool _isGrounded;
    private float _verticalVelocity;
    private float _horizontalVelocity;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();

        /*position equation p(t) = p0 + v0*t + (1/2)*g*t^2
          We only care about height gained, so set p0 = 0
          then eq becomes h(t) = v0*t + (1/2)*g*t^2
          At the apex (t = timeTillJumpApex), vertical velocity is 0: 
          after differentiating we get v(t) = v0 + g*t = 0 v(t) velocity at apex is zero then v0 = -g*t
          Substituting v0 = -g*t into h(t) eliminates the v0*t term: h = -g*t^2 + (1/2)*g*t^2 = -(1/2)*g*t^2
          then g = -2h / t^2
        */
        _gravity = -(2f * _movementStats.jumpHeight) / Mathf.Pow(_movementStats.timeTillJumpApex, 2);

        /* From v = g * t: the launch speed needed to reach 0 velocity (the apex)
          after timeTillJumpApex seconds under that gravity
        */
        _initialJumpVelocity = Mathf.Abs(_gravity) * _movementStats.timeTillJumpApex;
    }

    private void OnEnable()
    {
        _input.OnJumpPerformed += HandleJumpPerformed;
        _input.OnJumpCanceled += HandleJumpCanceled;
    }

    private void OnDisable()
    {
        _input.OnJumpPerformed -= HandleJumpPerformed;
        _input.OnJumpCanceled -= HandleJumpCanceled;
    }

#if UNITY_EDITOR
    private void Update()
    {
        _gravityLabel.text = "Gravity: " + _gravity.ToString();
        _initialJumpVelLabel.text = "Initial Jump Velocity: " + _initialJumpVelocity.ToString();
        _verticalVelLabel.text = "Vertical Velocity: " + _verticalVelocity.ToString();
        _horizontalVelLabel.text = "Horizontal Velocity: " + _horizontalVelocity.ToString();
    }
#endif

    private void FixedUpdate()
    {
        CheckGrounded();
        HandleJump();
        HandleMovement();
        ApplyMovement();
    }

    private void CheckGrounded()
    {
        _isGrounded = Physics2D.BoxCast(_groundCheckPoint.position, _groundCheckSize, 0f, Vector2.down, _groundCheckDistance, _groundLayer);
    }



    private void HandleMovement()
    {
        // The target speed will be max speed when we press input otherwise it will be zero
        float targetSpeed = _input.MoveInput.x * _movementStats.maxSpeed;
        float speedChange;

        if (Mathf.Abs(targetSpeed) > MOVEMENT_THRESHOLD)
        {
            // If the sign (i.e. positive or negative) of our input direction doesn't match our movement, it means we're turning around
            if (Mathf.Sign(targetSpeed) != Mathf.Sign(_horizontalVelocity))
                speedChange = _movementStats.turnSpeed;
            else
                speedChange = _isGrounded ? _movementStats.groundAcceleration : _movementStats.airAcceleration;
        }
        else
        {
            speedChange = _isGrounded ? _movementStats.groundDecceleration : _movementStats.airDeceleration;
        }

        // Mathf.MoveTowards uses constant speedchange
        _horizontalVelocity = Mathf.MoveTowards(_horizontalVelocity, targetSpeed, speedChange * Time.fixedDeltaTime);
    }

    private void ApplyMovement()
    {
        _rb.linearVelocity = new Vector2(_horizontalVelocity, _verticalVelocity);
    }

    private void HandleJumpPerformed()
    {
        if (!_isGrounded) return;

        _verticalVelocity = _initialJumpVelocity;
    }

    private void HandleJump()
    {
        if (!_isGrounded)
            _verticalVelocity += _gravity * Time.fixedDeltaTime;
    }

    private void HandleJumpCanceled()
    {

    }

    private void OnDrawGizmosSelected()
    {
        if (_groundCheckPoint == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
    }
}
