using System;
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


    private Rigidbody2D _rb;
    private PlayerInput _input;

    private float _moveSpeed;
    private bool _isGrounded;


    private float _verticalVelocity;
    private float _horizontalVelocity;


    private float _initialVerticalVelocity;
    private float _gravity;
    private float _timeTillPeakVerticalVel;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
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

    private void Start()
    {
        _gravity = -(2f * _movementStats.jumpHeight) / Mathf.Pow(_timeTillPeakVerticalVel, 2f);
        _initialVerticalVelocity = Mathf.Abs(_gravity) * _timeTillPeakVerticalVel;
    }

    private void Update()
    {
        ReadInputs();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        HandleJump();
        HandleMovement();
        ApplyMovement();
    }

    private void ReadInputs()
    {
        _moveSpeed = _movementStats.maxSpeed;
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

        // Mathf.MoveTowards makes it use constant speedchange
        _horizontalVelocity = Mathf.MoveTowards(_horizontalVelocity, targetSpeed, speedChange * Time.fixedDeltaTime);
    }

    private void ApplyMovement()
    {
        _rb.linearVelocity = new Vector2(_horizontalVelocity, _verticalVelocity);
    }


    private void HandleJump()
    {

    }

    private void HandleJumpPerformed()
    {

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
