using System;
using UnityEngine;

[Serializable]
public class PlayerMovementStats
{
    public float maxSpeed = 10;

    public float turnSpeed = 80f;
    public float groundAcceleration = 52f;
    public float groundDecceleration = 52f;
    public float airAcceleration;
    public float airDeceleration;

    [Header("Jump")]
    public float jumpHeight = 3f;
    public float timeTillJumpApex = 0.4f;
    public float groundedGravity = 0.1f;

    [Tooltip("How much vertical velocity is kept if jump is released early (variable jump height).")]
    [Range(0f, 1f)] public float jumpCutMultiplier = 0.5f;
    public float maxFallSpeed = 20f;
}