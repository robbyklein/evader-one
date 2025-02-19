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
  // Settings
  private readonly float _gravityNormal = 300f;
  private readonly float _gravityWater = 100f;
  private readonly float _friction = 0.9f;
  private readonly float _jumpForce = 100f;
  private readonly float _maxSpeed = 400f;
  private readonly float _moveSpeed = 400f;
  private readonly PhysicsSize _physicsSize;

  // Dependencies
  private readonly Entity _owner;
  private readonly SfxManager _sfxManager;
  private readonly List<RectangleF> _collisionRects;
  private readonly List<RectangleF> _waterRects;

  // State
  private float _currentGravity;
  private bool _jumpRequested;
  private Vector2 _velocity;
  private float _xRemainder;
  private float _yRemainder;

  // External inputs/flags
  public float MoveInputX = 0f;
  public bool IsFacingRight = true;
  public bool IsGrounded { get; private set; }

  // Constructor
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

  public void Update(GameTime gameTime) {
    var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

    // 1) Determine if player is in water
    bool isInWater = IsInWater();

    // 2) Choose the correct gravity
    _currentGravity = isInWater ? _gravityWater : _gravityNormal;

    // 3) Horizontal movement
    _velocity.X += MoveInputX * _moveSpeed * dt;
    _velocity.X = MathHelper.Clamp(_velocity.X, -_maxSpeed, _maxSpeed);

    if (MoveInputX > 0) {
      IsFacingRight = true;
    }
    else if (MoveInputX < 0) {
      IsFacingRight = false;
    }

    // Apply friction
    _velocity.X *= _friction;
    if (Math.Abs(_velocity.X) < 0.1f) {
      _velocity.X = 0f;
    }

    // 4) Apply gravity (if not grounded)
    if (!IsGrounded) {
      _velocity.Y += _currentGravity * dt;
    }

    // 5) Jump logic
    if (IsGrounded && _jumpRequested) {
      _velocity.Y = -_jumpForce;
      IsGrounded = false;
      _sfxManager.PlaySound(Sfx.Jump);
    }

    _jumpRequested = false;

    // 6) Move
    MoveX(_velocity.X * dt);
    MoveY(_velocity.Y * dt);

    // 7) Check grounded
    CheckIfGrounded();
  }

  public void Draw(SpriteBatch spriteBatch) {
    // No drawing in the physics component
  }

  // Request a jump from external code (e.g., input handling)
  public void RequestJump() {
    _jumpRequested = true;
  }

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
          // If we hit something while moving down, we are grounded
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
      _owner.Position.Y + _physicsSize.OffsetY + 1, // +1 for ground check
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
}