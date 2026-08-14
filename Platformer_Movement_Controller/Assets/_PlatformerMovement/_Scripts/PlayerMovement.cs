using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(PlayerInput), typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private const float MOVEMENT_THRESHOLD = 0.01f;
    [SerializeField] private PlayerMovementDataSO _movementStats;
    [SerializeField] private float _groundCheckDistance;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private LayerMask _groundLayer;

#if UNITY_EDITOR
    [Header("Debugging")]
    [SerializeField] private bool _toggleGizmos;
    [SerializeField] private TMP_Text _gravityLabel;
    [SerializeField] private TMP_Text _initialJumpVelLabel;
    [SerializeField] private TMP_Text _verticalVelLabel;
    [SerializeField] private TMP_Text _horizontalVelLabel;
    [SerializeField] private float _lineThickness = 5f;
    [SerializeField] private Color _verticalArrowColor = Color.green;
    [SerializeField] private Color _horizontalArrowColor = Color.yellow;
    [SerializeField] private float _arrowWidth = 10f;
    [SerializeField] private float _arrowHeight = 10f;
    [SerializeField] private Vector3 _offset = Vector2.down;
#endif

    private Rigidbody2D _rb;
    private float _gravity;
    private float _initialJumpVelocity; // velocity required to reach jump apex
    private PlayerInput _input;
    // private int _jumpsRemaining;
    private bool _isGrounded;
    private float _verticalVelocity;
    private float _jumpBufferCounter;
    private float _horizontalVelocity;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();

        SetupVariables();
    }

    private void SetupVariables()
    {
        /*position equation p(t) = p0 + v0*t + (1/2)*g*t^2
                  We only care about height gained, so set p0 = 0
                  then eq becomes h(t) = v0*t + (1/2)*g*t^2
                  At the apex (t = timeTillJumpApex), vertical velocity is 0: 
                  after differentiating we get v(t) = v0 + g*t = 0 v(t) velocity at apex is zero then v0 = -g*t
                  Substituting v0 = -g*t into h(t) eliminates the v0*t term: h = -g*t^2 + (1/2)*g*t^2 = -(1/2)*g*t^2
                  then g = -2h / t^2
                */
        _gravity = -(2f * _movementStats.JumpHeight) / Mathf.Pow(_movementStats.TimeTillJumpApex, 2) * _movementStats.GravityMuliplier;

        /* From v = g * t: the launch speed needed to reach 0 velocity (the apex)
          after timeTillJumpApex seconds under that gravity
        */
        _initialJumpVelocity = Mathf.Abs(_gravity) * _movementStats.TimeTillJumpApex;
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
        ApplyGravity();
        JumpInputBuffer();
        HandleMovement();
        ApplyMovement();
    }

    private void JumpInputBuffer()
    {
        // Jump input pressed countdown each frame
        _jumpBufferCounter -= Time.fixedDeltaTime;

        // Perform jump if the countdown/buffer counter hasn't reached zero and player hit the ground 
        if (_isGrounded && _jumpBufferCounter > 0f)
        {
            ExecuteJump();
            _jumpBufferCounter = 0f;
        }
    }

    private void ExecuteJump()
    {
        _verticalVelocity = _initialJumpVelocity;
        // _jumpsRemaining++;
    }

    private void CheckGrounded()
    {
        // bool wasGrounded = _isGrounded;

        _isGrounded = Physics2D.BoxCast(
            _groundCheckPoint.position,
            _groundCheckSize,
            0f,
            Vector2.down,
            _groundCheckDistance,
            _groundLayer);

        // Reset jumps when we land
        // if (_isGrounded && !wasGrounded)
        // {
        //     _jumpsRemaining = 0;
        // }
    }



    private void HandleMovement()
    {
        // The target speed will be max speed when we press input otherwise it will be zero
        float targetSpeed = _input.MoveInput.x * _movementStats.MaxSpeed;
        float speedChange;

        if (Mathf.Abs(targetSpeed) > MOVEMENT_THRESHOLD)
        {
            // If the sign (i.e. positive or negative) of our input direction doesn't match our movement, it means we're turning around
            if (Mathf.Sign(targetSpeed) != Mathf.Sign(_horizontalVelocity))
                speedChange = _movementStats.TurnSpeed;
            else
                speedChange = _isGrounded ? _movementStats.GroundAcceleration : _movementStats.AirAcceleration;
        }
        else
        {
            speedChange = _isGrounded ? _movementStats.GroundDecceleration : _movementStats.AirDeceleration;
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
        _jumpBufferCounter = _movementStats.JumpBufferTime;
    }

    private void ExecuteAirJump()
    {
        if (!_isGrounded && CanAirJump())
            ExecuteJump();
    }

    private bool CanAirJump()
    {
        // 
        return false;
    }

    private void HandleJumpCanceled()
    {
        if (_rb.linearVelocityY > 0)
        {
            _verticalVelocity *= _movementStats.JumpCutMultiplier;
        }
    }

    private void ApplyGravity()
    {
        if (_isGrounded)
        {
            _verticalVelocity = -_movementStats.GroundedVerticalVelocity;
            return;
        }

        _verticalVelocity += _gravity * Time.fixedDeltaTime;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!_toggleGizmos) return;

        var velocity = Vector3.up * _verticalVelocity;

        GizmosUtils.DrawWireArrow(
            transform.position,
            _offset,
            velocity,
            _arrowWidth,
            _arrowHeight,
            _lineThickness,
            _verticalArrowColor);

        GizmosUtils.DrawWireArrow(
            transform.position,
            _offset,
            Vector3.right * _horizontalVelocity,
            _arrowWidth,
            _arrowHeight,
            _lineThickness,
            _horizontalArrowColor);

        if (_groundCheckPoint == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            _groundCheckPoint.position,
            _groundCheckSize);
    }
#endif
}