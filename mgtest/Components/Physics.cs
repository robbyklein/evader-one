using System;
using System.Collections.Generic;
using mgtest.Entities;
using mgtest.Interfaces;
using mgtest.Types;
using mgtest.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace mgtest.Components;

public class Physics(
  Entity owner,
  SfxManager sfxManager,
  List<RectangleF> collisionRects,
  List<RectangleF> waterRects,
  Collider collider
) : IPhysics {
  // Settings
  private const float GravityNormal = 300f;
  private const float GravityWater = 100f;
  private const float Friction = 0.9f;
  private const float JumpForce = 100f;
  private const float MaxSpeed = 400f;
  private const float MoveSpeed = 400f;
  private const float WallSlideGravityMultiplier = 0.3f;
  private const float MaxWallSlideSpeed = 15f;
  private const float WallJumpHorizontalForce = 75f;

  // Internal State
  private float _currentGravity;
  private Vector2 _velocity;
  private float _xRemainder;
  private float _yRemainder;
  private bool _jumpRequested;
  private bool _isTouchingWallLeft;

  // Public state
  public float MoveInputX { get; set; } = 0f;
  public bool IsTouchingWallRight { get; private set; }
  public bool IsFacingRight { get; private set; } = true;
  public bool IsGrounded { get; private set; }


  // Lifecycle
  public void Update(GameTime gameTime) {
    var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
    UpdateGravity();
    UpdateMovementX(dt);
    UpdateWallCheck();
    ApplyGravity(dt);
    ApplyJump();
    ApplyMovement(dt);
    UpdateGrounded();
  }

  public void Draw(SpriteBatch spriteBatch) {
  }

  // Frame udpates
  private void UpdateGravity() {
    _currentGravity = IsInWater() ? GravityWater : GravityNormal;
  }

  private void UpdateMovementX(float dt) {
    // Velocity
    _velocity.X += MoveInputX * MoveSpeed * dt;
    _velocity.X = MathHelper.Clamp(_velocity.X, -MaxSpeed, MaxSpeed);

    // Facing
    if (MoveInputX > 0.01f) {
      IsFacingRight = true;
    }
    else if (MoveInputX < -0.01f) {
      IsFacingRight = false;
    }

    // Friction
    _velocity.X *= Friction;
    if (Math.Abs(_velocity.X) < 0.1f) {
      _velocity.X = 0f;
    }
  }

  private void ApplyGravity(float dt) {
    if (IsGrounded) {
      return;
    }

    // Copy so we don't effect class state
    float gravityToUse = _currentGravity;

    // Lessen gravity if wall sliding
    bool pressedAgainstWall = PressedAgainstWall();

    if (pressedAgainstWall && _velocity.Y > 0f) {
      gravityToUse *= WallSlideGravityMultiplier;
    }

    // Apply gravity
    _velocity.Y += gravityToUse * dt;

    // Max sliding speed
    if (pressedAgainstWall && _velocity.Y > MaxWallSlideSpeed) {
      _velocity.Y = MaxWallSlideSpeed;
    }
  }

  private void ApplyJump() {
    if (!_jumpRequested) {
      return;
    }

    // Ground jumps
    if (IsGrounded) {
      Jump();
    }
    // Wall jumps
    else if (PressedAgainstWall()) {
      WallJump();
    }

    // Reset jump
    _jumpRequested = false;
  }

  private void Jump() {
    _velocity.Y = -JumpForce;
    sfxManager.PlaySound(Sfx.Jump);
  }

  private void ApplyMovement(float dt) {
    MoveX(_velocity.X * dt);
    MoveY(_velocity.Y * dt);
  }

  private void UpdateWallCheck() {
    // Reset state
    _isTouchingWallLeft = false;
    IsTouchingWallRight = false;

    // Only matter when off ground
    if (IsGrounded) {
      return;
    }

    // Check both directions
    Vector2 leftCheckPos = owner.Position + new Vector2(-1, 0); // 1 pixel left
    Vector2 rightCheckPos = owner.Position + new Vector2(+1, 0); // 1 pixel right

    // Update if touching
    if (IsColliding(leftCheckPos)) {
      _isTouchingWallLeft = true;
    }

    if (IsColliding(rightCheckPos)) {
      IsTouchingWallRight = true;
    }
  }

  private void UpdateGrounded() {
    // Check one pixel below the current position
    RectangleF nextPosition = collider.GetProjectedBounds(
      owner.Position + new Vector2(0, 1)
    );

    IsGrounded = false;

    foreach (RectangleF rect in collisionRects) {
      if (nextPosition.Intersects(rect)) {
        IsGrounded = true;
        break;
      }
    }
  }

  // Handlers
  private void WallJump() {
    // Horizontal force
    if (PressedAgainstLeftWall()) {
      _velocity.X = +WallJumpHorizontalForce;
      IsFacingRight = true;
    }
    else if (PressedAgainstRightWall()) {
      _velocity.X = -WallJumpHorizontalForce;
      IsFacingRight = false;
    }

    // Vertical force
    _velocity.Y = -JumpForce;
    IsGrounded = false;

    // Play sfx
    sfxManager.PlaySound(Sfx.Jump);
  }

  private void MoveX(float distance) {
    // Accumulate the movement remainder to handle subpixel movement
    _xRemainder += distance;

    // Determine the integer movement amount by rounding the remainder
    var move = (int)MathF.Round(_xRemainder);

    if (move != 0) {
      // Subtract the applied movement from the remainder
      _xRemainder -= move;

      // Determine movement direction (1 for right, -1 for left)
      int sign = Math.Sign(move);

      // Process movement one step at a time
      while (move != 0) {
        // Calculate the next position
        var nextPos = new Vector2(owner.Position.X + sign, owner.Position.Y);

        // Check for collision at the next position
        if (IsColliding(nextPos)) {
          _velocity.X = 0; // Stop movement on collision
          break;
        }

        // Apply movement step
        owner.Position.X += sign;
        move -= sign;
      }
    }
  }

  private void MoveY(float distance) {
    // Accumulate the movement remainder to handle subpixel movement
    _yRemainder += distance;

    // Determine the integer movement amount by rounding the remainder
    var move = (int)MathF.Round(_yRemainder);

    if (move != 0) {
      // Subtract the applied movement from the remainder
      _yRemainder -= move;

      // Determine movement direction (1 for down, -1 for up)
      int sign = Math.Sign(move);

      // Process movement one step at a time
      while (move != 0) {
        // Calculate the next position
        var nextPos = new Vector2(owner.Position.X, owner.Position.Y + sign);

        // Check for collision at the next position
        if (IsColliding(nextPos)) {
          // If colliding while moving downward, mark as grounded
          if (sign > 0) {
            IsGrounded = true;
          }

          _velocity.Y = 0; // Stop vertical movement on collision
          break;
        }

        // Apply movement step
        owner.Position.Y += sign;
        move -= sign;
      }
    }
  }

  // Helpers
  private bool PressedAgainstLeftWall() {
    return _isTouchingWallLeft && MoveInputX < -0.1f;
  }

  private bool PressedAgainstRightWall() {
    return IsTouchingWallRight && MoveInputX > +0.1f;
  }

  private bool PressedAgainstWall() {
    return PressedAgainstLeftWall() || PressedAgainstRightWall();
  }

  private bool IsColliding(Vector2 newPosition) {
    // Get colliders next position
    RectangleF nextPosition = collider.GetProjectedBounds(newPosition);

    // Check for collisions
    foreach (RectangleF rect in collisionRects) {
      if (nextPosition.Intersects(rect)) {
        return true;
      }
    }

    return false;
  }

  private bool IsInWater() {
    RectangleF playerRect = collider.Bounds;

    foreach (RectangleF waterRect in waterRects) {
      if (playerRect.Intersects(waterRect)) {
        return true;
      }
    }

    return false;
  }

  // Externals
  public void RequestJump() {
    _jumpRequested = true;
  }

  public bool IsWallSliding {
    get {
      if (IsGrounded) {
        return false;
      }

      // Must be pressing into the wall
      bool pressingLeftOnLeftWall = _isTouchingWallLeft && MoveInputX < 0f;
      bool pressingRightOnRightWall = IsTouchingWallRight && MoveInputX > 0f;
      bool movingDown = _velocity.Y > 0f;
      return (pressingLeftOnLeftWall || pressingRightOnRightWall) && movingDown;
    }
  }
}