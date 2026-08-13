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
}