using System;
using System.Collections.Generic;
using mgtest.Data;
using mgtest.Entities;
using mgtest.Interfaces;
using mgtest.Types;
using mgtest.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace mgtest.Components;

public class Physics : IPhysics {
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

  private readonly Entity owner;
  private readonly SfxManager sfxManager;
  private readonly List<RectangleF> collisionRects;
  private readonly List<RectangleF> waterRects;
  private readonly List<RectangleF> noWallRects;
  private readonly Collider collider;
  private readonly RectangleF finishRect;

  // Updated constructor now includes noWallRects.
  public Physics(Entity owner, SfxManager sfxManager, List<RectangleF> collisionRects, List<RectangleF> waterRects,
    List<RectangleF> noWallRects, Collider collider, RectangleF finishRect) {
    this.owner = owner;
    this.sfxManager = sfxManager;
    this.collisionRects = collisionRects;
    this.waterRects = waterRects;
    this.noWallRects = noWallRects;
    this.collider = collider;
    this.finishRect = finishRect;
  }

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
    CheckFinish();
  }

  public void CheckFinish() {
    if (IsInFinish()) {
      GameData.GameFinished = true;
    }
  }

  public void Draw(SpriteBatch spriteBatch) {
    // Drawing logic if needed
  }

  // Frame updates
  private void UpdateGravity() {
    _currentGravity = IsInWater() ? GravityWater : GravityNormal;
  }

  private void UpdateMovementX(float dt) {
    _velocity.X += MoveInputX * MoveSpeed * dt;
    _velocity.X = MathHelper.Clamp(_velocity.X, -MaxSpeed, MaxSpeed);

    if (MoveInputX > 0.01f) {
      IsFacingRight = true;
    }
    else if (MoveInputX < -0.01f) {
      IsFacingRight = false;
    }

    _velocity.X *= Friction;
    if (Math.Abs(_velocity.X) < 0.1f) {
      _velocity.X = 0f;
    }
  }

  private void ApplyGravity(float dt) {
    if (IsGrounded) {
      return;
    }

    float gravityToUse = _currentGravity;
    bool pressedAgainstWall = PressedAgainstWall();

    if (pressedAgainstWall && _velocity.Y > 0f) {
      gravityToUse *= WallSlideGravityMultiplier;
    }

    _velocity.Y += gravityToUse * dt;

    if (pressedAgainstWall && _velocity.Y > MaxWallSlideSpeed) {
      _velocity.Y = MaxWallSlideSpeed;
    }
  }

  private void ApplyJump() {
    if (!_jumpRequested) {
      return;
    }

    if (IsGrounded) {
      Jump();
    }
    else if (PressedAgainstWall()) {
      WallJump();
    }

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
    _isTouchingWallLeft = false;
    IsTouchingWallRight = false;

    if (IsGrounded) {
      return;
    }

    Vector2 leftCheckPos = owner.Position + new Vector2(-1, 0);
    Vector2 rightCheckPos = owner.Position + new Vector2(1, 0);

    // Use the filtered collision check for wall detection.
    if (IsCollidingForWallCheck(leftCheckPos)) {
      _isTouchingWallLeft = true;
    }

    if (IsCollidingForWallCheck(rightCheckPos)) {
      IsTouchingWallRight = true;
    }
  }

  private void UpdateGrounded() {
    RectangleF nextPosition = collider.GetProjectedBounds(owner.Position + new Vector2(0, 1));
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
    if (PressedAgainstLeftWall()) {
      _velocity.X = +WallJumpHorizontalForce;
      IsFacingRight = true;
    }
    else if (PressedAgainstRightWall()) {
      _velocity.X = -WallJumpHorizontalForce;
      IsFacingRight = false;
    }

    _velocity.Y = -JumpForce;
    IsGrounded = false;
    sfxManager.PlaySound(Sfx.Jump);
  }

  private void MoveX(float distance) {
    _xRemainder += distance;
    var move = (int)MathF.Round(_xRemainder);

    if (move != 0) {
      _xRemainder -= move;
      int sign = Math.Sign(move);

      while (move != 0) {
        var nextPos = new Vector2(owner.Position.X + sign, owner.Position.Y);
        if (IsColliding(nextPos)) {
          _velocity.X = 0;
          break;
        }

        owner.Position.X += sign;
        move -= sign;
      }
    }
  }

  private void MoveY(float distance) {
    _yRemainder += distance;
    var move = (int)MathF.Round(_yRemainder);

    if (move != 0) {
      _yRemainder -= move;
      int sign = Math.Sign(move);

      while (move != 0) {
        var nextPos = new Vector2(owner.Position.X, owner.Position.Y + sign);
        if (IsColliding(nextPos)) {
          if (sign > 0) {
            IsGrounded = true;
          }

          _velocity.Y = 0;
          break;
        }

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
    return IsTouchingWallRight && MoveInputX > 0.1f;
  }

  private bool PressedAgainstWall() {
    return PressedAgainstLeftWall() || PressedAgainstRightWall();
  }

  // Standard collision check for general movement (uses all collision rectangles).
  private bool IsColliding(Vector2 newPosition) {
    RectangleF nextPosition = collider.GetProjectedBounds(newPosition);
    foreach (RectangleF rect in collisionRects) {
      if (nextPosition.Intersects(rect)) {
        return true;
      }
    }

    return false;
  }

  // New: Collision check used for wall detection that ignores rectangles from the noWallRects list.
  private bool IsCollidingForWallCheck(Vector2 newPosition) {
    RectangleF nextPosition = collider.GetProjectedBounds(newPosition);
    foreach (RectangleF rect in collisionRects) {
      // Skip rectangles marked as "no walls"
      if (noWallRects.Contains(rect)) {
        continue;
      }

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

  private bool IsInFinish() {
    RectangleF playerRect = collider.Bounds;

    if (playerRect.Intersects(finishRect)) {
      return true;
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

      bool pressingLeftOnLeftWall = _isTouchingWallLeft && MoveInputX < 0f;
      bool pressingRightOnRightWall = IsTouchingWallRight && MoveInputX > 0f;
      bool movingDown = _velocity.Y > 0f;
      return (pressingLeftOnLeftWall || pressingRightOnRightWall) && movingDown;
    }
  }
}