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

public struct PhysicsSize {
  public int Width;
  public int Height;
  public int OffsetX;
  public int OffsetY;

  public PhysicsSize(int width = 8, int height = 8, int offsetX = 0, int offsetY = 0) {
    Width = width;
    Height = height;
    OffsetX = offsetX;
    OffsetY = offsetY;
  }
}

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
  private readonly PhysicsSize _physicsSize;

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

  // ---------------------------------------------------------
  // State
  // ---------------------------------------------------------
  private float _currentGravity;
  private Vector2 _velocity;
  private float _xRemainder;
  private float _yRemainder;
  private bool _jumpRequested;

  // Wall checks

  // ---------------------------------------------------------
  // External inputs/flags
  // ---------------------------------------------------------
  public float MoveInputX = 0f;
  public bool IsFacingRight = true;
  public bool IsGrounded { get; private set; }

  // ---------------------------------------------------------
  // Constructor
  // ---------------------------------------------------------
  public Physics(
    Entity owner,
    PhysicsSize physicsSize,
    SfxManager sfxManager,
    List<RectangleF> collisionRects,
    List<RectangleF> waterRects) {
    _owner = owner;
    _physicsSize = physicsSize;
    _collisionRects = collisionRects;
    _waterRects = waterRects;
    _sfxManager = sfxManager;
  }

  // ---------------------------------------------------------
  // MonoGame Update
  // ---------------------------------------------------------
  public void Update(GameTime gameTime) {
    var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

    // 1) Check if in water
    bool isInWater = IsInWater();
    _currentGravity = isInWater ? _gravityWater : _gravityNormal;

    // 2) Horizontal movement
    _velocity.X += MoveInputX * _moveSpeed * dt;
    _velocity.X = MathHelper.Clamp(_velocity.X, -_maxSpeed, _maxSpeed);

    // **Update facing direction** based on horizontal input
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

    // 4) Apply gravity (with possible wall-slide)
    if (!IsGrounded) {
      float gravityThisFrame = _currentGravity;

      // If pressing into the wall while falling, reduce gravity
      bool pressingLeftOnLeftWall = IsTouchingWallLeft && MoveInputX < 0f;
      bool pressingRightOnRightWall = IsTouchingWallRight && MoveInputX > 0f;

      if ((pressingLeftOnLeftWall || pressingRightOnRightWall) && _velocity.Y > 0f) {
        gravityThisFrame *= _wallSlideGravityMultiplier;
      }

      _velocity.Y += gravityThisFrame * dt;

      // Optionally clamp downward speed during wall-slide
      if ((pressingLeftOnLeftWall || pressingRightOnRightWall) && _velocity.Y > _maxWallSlideSpeed) {
        _velocity.Y = _maxWallSlideSpeed;
      }
    }

    // 5) Jump (normal or wall-jump)
    if (_jumpRequested) {
      if (IsGrounded) {
        _velocity.Y = -_jumpForce;
      }
      else if (IsTouchingWallLeft || IsTouchingWallRight) {
        WallJump();
      }

      _jumpRequested = false;
    }

    // 6) Movement & collisions
    MoveX(_velocity.X * dt);
    MoveY(_velocity.Y * dt);

    // 7) Check grounded
    CheckIfGrounded();
  }

  public void Draw(SpriteBatch spriteBatch) {
    // No draw logic in Physics
  }

  // ---------------------------------------------------------
  // Public: Request Jump
  // ---------------------------------------------------------
  public void RequestJump() {
    _jumpRequested = true;
  }

  // ---------------------------------------------------------
  // Private: Wall Jump
  // ---------------------------------------------------------
  private void WallJump() {
    // Push off horizontally
    if (IsTouchingWallLeft) {
      _velocity.X = +_wallJumpHorizontalForce;
      IsFacingRight = true;
    }
    else if (IsTouchingWallRight) {
      _velocity.X = -_wallJumpHorizontalForce;
      IsFacingRight = false;
    }

    // Same vertical force as normal jump
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

  private bool IsColliding(Vector2 newPosition) {
    var playerRect = new RectangleF(
      newPosition.X + _physicsSize.OffsetX,
      newPosition.Y + _physicsSize.OffsetY,
      _physicsSize.Width,
      _physicsSize.Height
    );

    foreach (RectangleF rect in _collisionRects) {
      if (playerRect.Intersects(rect)) {
        return true;
      }
    }

    return false;
  }

  private void CheckIfGrounded() {
    var playerRect = new RectangleF(
      _owner.Position.X + _physicsSize.OffsetX,
      _owner.Position.Y + _physicsSize.OffsetY + 1, // +1 pixel for ground check
      _physicsSize.Width,
      _physicsSize.Height
    );

    IsGrounded = false;

    foreach (RectangleF rect in _collisionRects) {
      if (playerRect.Intersects(rect)) {
        IsGrounded = true;
        break;
      }
    }
  }

  private bool IsInWater() {
    var playerRect = new RectangleF(
      _owner.Position.X + _physicsSize.OffsetX,
      _owner.Position.Y + _physicsSize.OffsetY,
      _physicsSize.Width,
      _physicsSize.Height
    );

    foreach (RectangleF waterRect in _waterRects) {
      if (playerRect.Intersects(waterRect)) {
        return true;
      }
    }

    return false;
  }

  // ---------------------------------------------------------
  // Wall Logic
  // ---------------------------------------------------------
  private void UpdateWallCheck() {
    IsTouchingWallLeft = false;
    IsTouchingWallRight = false;

    // Only check for walls if not grounded
    if (!IsGrounded) {
      var leftCheckPos = new Vector2(_owner.Position.X - 1, _owner.Position.Y);
      var rightCheckPos = new Vector2(_owner.Position.X + 1, _owner.Position.Y);

      if (IsColliding(leftCheckPos)) {
        IsTouchingWallLeft = true;
      }

      if (IsColliding(rightCheckPos)) {
        IsTouchingWallRight = true;
      }
    }
  }

  // If you need the animator to detect wall-sliding:
  public bool IsWallSliding {
    get {
      if (IsGrounded) {
        return false;
      }

      // Pressing into the wall?
      bool pressingLeftOnLeftWall = IsTouchingWallLeft && MoveInputX < 0f;
      bool pressingRightOnRightWall = IsTouchingWallRight && MoveInputX > 0f;

      // Actually moving downward?
      bool movingDown = _velocity.Y > 0f;

      return (pressingLeftOnLeftWall || pressingRightOnRightWall) && movingDown;
    }
  }

  // For the animator, if needed:
  public bool IsTouchingWallLeft { get; private set; }

  public bool IsTouchingWallRight { get; private set; }
}