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

    public float jumpHeight = 5; // in Units
}