using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    // Make sure to use project wide input action
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private InputActionReference _jumpAction;
    [SerializeField] private InputActionReference _runAction;

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


    private void Update()
    {
        _moveInput = _moveAction.action.ReadValue<Vector2>();

        _shouldRun = _runAction.action.IsPressed();

        if (_jumpAction.action.WasPerformedThisFrame())
        {
            OnJumpPerformed?.Invoke();
        }
    }
}