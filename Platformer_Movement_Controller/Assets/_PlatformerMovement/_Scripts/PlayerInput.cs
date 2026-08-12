using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    // Make sure to use project wide input action
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private InputActionReference _jumpAction;

    private Vector2 _moveInput;
    private bool _shouldRun;

    public Vector2 MoveInput
    {
        get
        {
            return _moveInput;
        }
    }

    public bool ShouldRun
    {
        get
        {
            return _shouldRun;
        }
    }

    public Action OnJumpPerformed;
    public Action OnJumpCanceled;


    private void Update()
    {
        _moveInput = _moveAction.action.ReadValue<Vector2>();

        if (_jumpAction.action.WasPerformedThisFrame())
        {
            OnJumpPerformed?.Invoke();
        }

        if (_jumpAction.action.WasReleasedThisFrame())
        {
            OnJumpCanceled?.Invoke();
        }
    }
}