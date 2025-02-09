using System;
using System.Collections.Generic;
using mgtest.Entities;
using mgtest.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace mgtest.Components;

public class Physics : IComponent {
  private readonly float _friction = 0.9f;
  private readonly float _gravity = 400f;
  private readonly float _jumpForce = 200f;
  private readonly float _maxSpeed = 400f;
  private readonly float _moveSpeed = 400f;

  private readonly Entity _owner;
  private readonly List<RectangleF> _collisionRects;

  private bool _jumpRequested;
  private Vector2 _velocity;
  private float _xRemainder;
  private float _yRemainder;

  public float MoveInputX = 0f;
  public bool IsFacingRight = true;
  public bool IsGrounded { get; private set; }

  public Physics(Entity owner, List<RectangleF> collisionRects) {
    _owner = owner;
    _collisionRects = collisionRects;
  }

  public void Update(GameTime gameTime) {
    var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
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

    if (!IsGrounded) {
      _velocity.Y += _gravity * dt;
    }

    if (IsGrounded && _jumpRequested) {
      _velocity.Y = -_jumpForce;
      IsGrounded = false;
    }

    _jumpRequested = false;

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
    const int playerWidth = 8;
    const int playerHeight = 8;
    var playerRect = new RectangleF(newPosition.X, newPosition.Y, playerWidth, playerHeight);
    foreach (RectangleF rect in _collisionRects) {
      if (playerRect.Intersects(rect)) {
        return true;
      }
    }

    return false;
  }

  private void CheckIfGrounded() {
    const int playerWidth = 8;
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
}