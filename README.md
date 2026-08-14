# 2D Platformer Movement

A physics-based 2D character controller for Unity, built around `Rigidbody2D`.

## Features

- **Acceleration-driven movement** — separate accel/deccel values for ground and air, plus a dedicated turn speed so direction changes feel snappy instead of floaty.
- **Coyote time** — short grace window to jump after walking off a ledge.
- **Jump buffering** — presses just before landing still trigger a jump.
- **Variable jump height** — releasing jump early cuts the arc short.
- **Ground/ceiling detection** via `Physics2D.BoxCast`.