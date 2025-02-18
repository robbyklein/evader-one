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
  // Normal gravity vs. water gravity
  private readonly float _gravityNormal = 300f;
  private readonly float _gravityWater = 100f;
  private float _currentGravity;

  private readonly float _friction = 0.9f;
  private readonly float _jumpForce = 100f;
  private readonly float _maxSpeed = 400f;
  private readonly float _moveSpeed = 400f;

  private readonly Entity _owner;
  private readonly List<RectangleF> _collisionRects;
  private readonly List<RectangleF> _waterRects;

  private bool _jumpRequested;
  private Vector2 _velocity;
  private float _xRemainder;
  private float _yRemainder;

  public float MoveInputX = 0f;
  public bool IsFacingRight = true;
  public bool IsGrounded { get; private set; }

  public Physics(Entity owner, List<RectangleF> collisionRects, List<RectangleF> waterRects) {
    _owner = owner;
    _collisionRects = collisionRects;
    _waterRects = waterRects;
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

    _velocity.X *= _friction;
    if (Math.Abs(_velocity.X) < 0.1f) {
      _velocity.X = 0f;
    }

    // 4) Apply gravity
    if (!IsGrounded) {
      _velocity.Y += _currentGravity * dt;
    }

    // 5) Jump logic
    if (IsGrounded && _jumpRequested) {
      _velocity.Y = -_jumpForce;
      IsGrounded = false;
      SfxManager.PlaySound(Sfx.Jump);
    }

    _jumpRequested = false;

    // 6) Move
    MoveX(_velocity.X * dt);
    MoveY(_velocity.Y * dt);

    CheckIfGrounded();
  }

  public void Draw(SpriteBatch spriteBatch) {
  }

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
    const int playerWidth = 4;
    const int playerHeight = 8;
    var playerRect = new RectangleF(newPosition.X + 2, newPosition.Y, playerWidth, playerHeight);

    // Check collision with "solid" rectangles only
    foreach (RectangleF rect in _collisionRects) {
      if (playerRect.Intersects(rect)) {
        return true;
      }
    }

    return false;
  }

  private void CheckIfGrounded() {
    const int playerWidth = 6;
    const int playerHeight = 8;
    var playerRect = new RectangleF(_owner.Position.X, _owner.Position.Y + 1, playerWidth, playerHeight);
    IsGrounded = false;

    foreach (RectangleF rect in _collisionRects) {
      if (playerRect.Intersects(rect)) {
        IsGrounded = true;
        break;
      }
    }
  }

  private bool IsInWater() {
    // Check if player's rectangle intersects any water rect.
    // Use roughly the same rectangle as for collision
    const int playerWidth = 4;
    const int playerHeight = 8;
    var playerRect = new RectangleF(_owner.Position.X + 2, _owner.Position.Y, playerWidth, playerHeight);

    foreach (RectangleF waterRect in _waterRects) {
      if (playerRect.Intersects(waterRect)) {
        return true;
      }
    }

    return false;
  }
}