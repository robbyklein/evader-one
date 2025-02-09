using System;
using System.Collections.Generic;
using mgtest.Entities;
using mgtest.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Components;

public class Physics : IComponent {
  // Settings
  private readonly float _friction = 0.001f;
  private readonly float _gravity = 400f;
  private readonly float _jumpForce = 200f;
  private readonly float _maxSpeed = 400f;
  private readonly float _moveSpeed = 400f;

  // Dependencies
  private readonly Entity _owner;
  private readonly HashSet<Point> _solidTiles;
  private readonly int _tileHeight;
  private readonly int _tileWidth;

  // State
  private bool _jumpRequested;
  private Vector2 _velocity;
  private float _xRemainder;
  private float _yRemainder;

  // External
  public float MoveInputX = 0f;
  public bool IsFacingRight = true;
  public bool IsGrounded { get; private set; }


  public Physics(Entity owner, HashSet<Point> solidTiles, int tileWidth, int tileHeight) {
    _owner = owner;
    _solidTiles = solidTiles;
    _tileWidth = tileWidth;
    _tileHeight = tileHeight;
  }

  public void Update(GameTime gameTime) {
    var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

    // 1) Horizontal Movement/Acceleration
    _velocity.X += MoveInputX * _moveSpeed * dt;
    _velocity.X = MathHelper.Clamp(_velocity.X, -_maxSpeed, _maxSpeed);

    // ✅ Update facing direction if actively moving
    if (MoveInputX > 0) {
      IsFacingRight = true;
    }
    else if (MoveInputX < 0) {
      IsFacingRight = false;
    }

    // Friction (time-based exponential)
    _velocity.X *= MathF.Pow(_friction, dt);

    // Tiny threshold to prevent jitter
    if (Math.Abs(_velocity.X) < 0.1f) {
      _velocity.X = 0f;
    }

    // 2) Gravity
    if (!IsGrounded) {
      _velocity.Y += _gravity * dt;
    }

    // 3) Jump
    if (IsGrounded && _jumpRequested) {
      _velocity.Y = -_jumpForce;
      IsGrounded = false;
    }

    _jumpRequested = false; // Reset every frame

    // 4) Move using Celeste/TowerFall subpixel approach
    MoveX(_velocity.X * dt);
    MoveY(_velocity.Y * dt);

    // 5) Check if grounded
    CheckIfGrounded();
  }


  public void Draw(SpriteBatch spriteBatch) {
    // This component does not draw anything
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
        Vector2 nextPos = _owner.Position + new Vector2(sign, 0);

        if (IsColliding(nextPos)) {
          _velocity.X = 0;
          break;
        }

        // No collision; take the step
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
        Vector2 nextPos = _owner.Position + new Vector2(0, sign);

        if (IsColliding(nextPos)) {
          // Collided with floor/ceiling
          if (sign > 0) {
            // Landed on the ground
            IsGrounded = true;
          }

          _velocity.Y = 0;
          break;
        }

        // No collision; take the step
        _owner.Position.Y += sign;
        move -= sign;
      }
    }
  }

  private void CheckIfGrounded() {
    // bounding box: 8x16
    var playerWidth = 8;
    var playerHeight = 8;

    float bottomY = _owner.Position.Y + playerHeight;
    float leftX = _owner.Position.X;
    float rightX = _owner.Position.X + (playerWidth - 1);

    var tileY = (int)Math.Floor(bottomY / _tileHeight);
    var tileXLeft = (int)Math.Floor(leftX / _tileWidth);
    var tileXRight = (int)Math.Floor(rightX / _tileWidth);

    IsGrounded = _solidTiles.Contains(new Point(tileXLeft, tileY)) ||
                 _solidTiles.Contains(new Point(tileXRight, tileY));
  }

  private bool IsColliding(Vector2 position) {
    var playerWidth = 8;
    var playerHeight = 8;

    // We'll sample top, middle, bottom (3 points) on left & right edges
    var tileCheckPoints = 3;

    for (var i = 0; i < tileCheckPoints; i++) {
      float t = i / (float)(tileCheckPoints - 1);
      float sampleY = position.Y + t * (playerHeight - 1);
      float leftX = position.X;
      float rightX = position.X + (playerWidth - 1);

      var tileY = (int)Math.Floor(sampleY / _tileHeight);
      var tileXLeft = (int)Math.Floor(leftX / _tileWidth);
      var tileXRight = (int)Math.Floor(rightX / _tileWidth);

      if (_solidTiles.Contains(new Point(tileXLeft, tileY)) ||
          _solidTiles.Contains(new Point(tileXRight, tileY))) {
        return true;
      }
    }

    return false;
  }
}