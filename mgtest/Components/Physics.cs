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

public class Physics : IComponent {
  // ---------------------------------------------------------
  // Basic Physics Settings
  // ---------------------------------------------------------
  private readonly float _gravityNormal = 300f;
  private readonly float _gravityWater = 100f;
  private readonly float _friction = 0.9f;
  private readonly float _jumpForce = 100f;
  private readonly float _maxSpeed = 400f;
  private readonly float _moveSpeed = 400f;

  // ---------------------------------------------------------
  // Wall Slide / Jump
  // ---------------------------------------------------------
  private readonly float _wallSlideGravityMultiplier = 0.3f;
  private readonly float _maxWallSlideSpeed = 15f;
  private readonly float _wallJumpHorizontalForce = 75f;

  // ---------------------------------------------------------
  // Dependencies
  // ---------------------------------------------------------
  private readonly Entity _owner;
  private readonly SfxManager _sfxManager;
  private readonly List<RectangleF> _collisionRects;
  private readonly List<RectangleF> _waterRects;

  // We'll use the existing Collider instead of our own rectangle
  private readonly Collider _collider;

  // ---------------------------------------------------------
  // State
  // ---------------------------------------------------------
  private float _currentGravity;
  private Vector2 _velocity;
  private float _xRemainder;
  private float _yRemainder;
  private bool _jumpRequested;

  // ---------------------------------------------------------
  // External inputs/flags
  // ---------------------------------------------------------
  public float MoveInputX = 0f;
  public bool IsFacingRight = true;
  public bool IsGrounded { get; private set; }

  // Wall checks
  public bool IsTouchingWallLeft { get; private set; }
  public bool IsTouchingWallRight { get; private set; }

  public Physics(
    Entity owner,
    SfxManager sfxManager,
    List<RectangleF> collisionRects,
    List<RectangleF> waterRects
  ) {
    _owner = owner;
    _sfxManager = sfxManager;
    _collisionRects = collisionRects;
    _waterRects = waterRects;

    // Ensure there's a Collider on this entity
    _collider = _owner.GetComponent<Collider>()
                ?? throw new InvalidOperationException(
                  "Physics requires a Collider component on the same entity!"
                );
  }

  public void Update(GameTime gameTime) {
    var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

    // 1) Determine if in water -> set gravity
    bool isInWater = IsInWater();
    _currentGravity = isInWater ? _gravityWater : _gravityNormal;

    // 2) Horizontal movement
    _velocity.X += MoveInputX * _moveSpeed * dt;
    _velocity.X = MathHelper.Clamp(_velocity.X, -_maxSpeed, _maxSpeed);

    // Update facing direction
    if (MoveInputX > 0.01f) {
      IsFacingRight = true;
    }
    else if (MoveInputX < -0.01f) {
      IsFacingRight = false;
    }

    // Apply friction
    _velocity.X *= _friction;
    if (Math.Abs(_velocity.X) < 0.1f) {
      _velocity.X = 0f;
    }

    // 3) Update wall checks
    UpdateWallCheck();

    // 4) Apply gravity (considering wall-slide)
    if (!IsGrounded) {
      float gravityToUse = _currentGravity;

      // If pressing into wall while falling, reduce gravity
      bool pressingLeftOnLeftWall = IsTouchingWallLeft && MoveInputX < 0f;
      bool pressingRightOnRightWall = IsTouchingWallRight && MoveInputX > 0f;
      if ((pressingLeftOnLeftWall || pressingRightOnRightWall) && _velocity.Y > 0f) {
        gravityToUse *= _wallSlideGravityMultiplier;
      }

      _velocity.Y += gravityToUse * dt;

      // Optionally clamp downward speed during wall-slide
      if (
        (pressingLeftOnLeftWall || pressingRightOnRightWall) &&
        _velocity.Y > _maxWallSlideSpeed
      ) {
        _velocity.Y = _maxWallSlideSpeed;
      }
    }

    // 5) Jump logic
    if (_jumpRequested) {
      if (IsGrounded) {
        // Normal jump
        _velocity.Y = -_jumpForce;
        _sfxManager.PlaySound(Sfx.Jump);
      }
      else {
        // Must be on a wall AND pressing into the wall:
        bool pressingLeftAgainstLeftWall =
          IsTouchingWallLeft && MoveInputX < -0.1f;
        bool pressingRightAgainstRightWall =
          IsTouchingWallRight && MoveInputX > +0.1f;

        if (pressingLeftAgainstLeftWall || pressingRightAgainstRightWall) {
          WallJump();
        }
      }

      _jumpRequested = false;
    }

    // 6) Movement & collisions
    MoveX(_velocity.X * dt);
    MoveY(_velocity.Y * dt);

    // 7) Check if grounded
    CheckIfGrounded();
  }

  public void Draw(SpriteBatch spriteBatch) {
    // No-op for Physics
  }

  public void RequestJump() {
    _jumpRequested = true;
  }

  private void WallJump() {
    // Determine direction based on which wall you're touching
    if (IsTouchingWallLeft) {
      _velocity.X = +_wallJumpHorizontalForce;
      IsFacingRight = true;
    }
    else if (IsTouchingWallRight) {
      _velocity.X = -_wallJumpHorizontalForce;
      IsFacingRight = false;
    }

    // Vertical force
    _velocity.Y = -_jumpForce;
    IsGrounded = false;

    _sfxManager.PlaySound(Sfx.Jump);
  }

  // ---------------------------------------------------------
  // Movement + Collision
  // ---------------------------------------------------------
  private void MoveX(float distance) {
    _xRemainder += distance;
    var move = (int)MathF.Round(_xRemainder);
    if (move != 0) {
      _xRemainder -= move;
      int sign = Math.Sign(move);

      while (move != 0) {
        var nextPos = new Vector2(_owner.Position.X + sign, _owner.Position.Y);
        if (IsColliding(nextPos)) {
          _velocity.X = 0;
          break;
        }

        _owner.Position.X += sign;
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
        var nextPos = new Vector2(_owner.Position.X, _owner.Position.Y + sign);
        if (IsColliding(nextPos)) {
          // If we hit something while moving down => grounded
          if (sign > 0) {
            IsGrounded = true;
          }

          _velocity.Y = 0;
          break;
        }

        _owner.Position.Y += sign;
        move -= sign;
      }
    }
  }

  /// <summary>
  ///   Checks if the entity’s collider would intersect any solid tile at the given position.
  /// </summary>
  private bool IsColliding(Vector2 newPosition) {
    // "Project" the collider as if the entity were at newPosition
    RectangleF testRect = _collider.GetProjectedBounds(newPosition);

    foreach (RectangleF rect in _collisionRects) {
      if (testRect.Intersects(rect)) {
        return true;
      }
    }

    return false;
  }

  private void CheckIfGrounded() {
    // Check one pixel below the current position
    RectangleF testRect = _collider.GetProjectedBounds(
      _owner.Position + new Vector2(0, 1)
    );

    IsGrounded = false;
    foreach (RectangleF rect in _collisionRects) {
      if (testRect.Intersects(rect)) {
        IsGrounded = true;
        break;
      }
    }
  }

  private bool IsInWater() {
    // Use the current collider bounds
    RectangleF playerRect = _collider.Bounds;
    foreach (RectangleF waterRect in _waterRects) {
      if (playerRect.Intersects(waterRect)) {
        return true;
      }
    }

    return false;
  }

  private void UpdateWallCheck() {
    IsTouchingWallLeft = false;
    IsTouchingWallRight = false;

    if (!IsGrounded) {
      Vector2 leftCheckPos = _owner.Position + new Vector2(-1, 0);
      Vector2 rightCheckPos = _owner.Position + new Vector2(+1, 0);

      if (IsColliding(leftCheckPos)) {
        IsTouchingWallLeft = true;
      }

      if (IsColliding(rightCheckPos)) {
        IsTouchingWallRight = true;
      }
    }
  }

  // For the animator if needed
  public bool IsWallSliding {
    get {
      if (IsGrounded) {
        return false;
      }

      // Must be pressing into the wall
      bool pressingLeftOnLeftWall = IsTouchingWallLeft && MoveInputX < 0f;
      bool pressingRightOnRightWall = IsTouchingWallRight && MoveInputX > 0f;
      bool movingDown = _velocity.Y > 0f;
      return (pressingLeftOnLeftWall || pressingRightOnRightWall) && movingDown;
    }
  }
}