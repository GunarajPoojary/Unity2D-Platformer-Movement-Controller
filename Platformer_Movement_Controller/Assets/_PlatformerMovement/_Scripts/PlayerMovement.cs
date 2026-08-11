using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInput), typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerMovementStats _movementStats;

    private Rigidbody2D _rb;
    private PlayerInput _input;

    private Vector2 _targetVelocity; // Set the target velocity which is then applied to Rigidbody
    private float _moveSpeed;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _input.OnJumpPerformed += HandleJumpPerformed;
    }

    private void OnDisable()
    {
        _input.OnJumpPerformed -= HandleJumpPerformed;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }




    private void HandleMovement()
    {
        _moveSpeed = _input.ShouldRun ? _movementStats.runSpeed : _movementStats.walkSpeed;

        Debug.Log($"Input vector is {_input.MoveInput}");

        var run = _input.ShouldRun ? "Should run" : "Should Walk";
        Debug.Log($"{run}");

        _targetVelocity = new Vector2(_moveSpeed * _input.MoveInput.x, 0f); ;

        _rb.linearVelocity = _targetVelocity;
    }

    private void HandleJumpPerformed()
    {
        Debug.Log("Jump");
    }
}
