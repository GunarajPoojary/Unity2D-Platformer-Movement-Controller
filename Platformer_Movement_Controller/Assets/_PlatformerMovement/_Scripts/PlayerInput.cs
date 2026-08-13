using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private Vector2 _moveInput;

    public Vector2 MoveInput
    {
        get
        {
            return _moveInput;
        }
    }

    public Action OnJumpPerformed;
    public Action OnJumpCanceled;

    private void Update()
    {
        _moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetButtonDown("Jump"))
        {
            OnJumpPerformed?.Invoke();
        }

        if (Input.GetButtonUp("Jump"))
        {
            OnJumpCanceled?.Invoke();
        }
    }
}