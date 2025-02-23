using System.Collections.Generic;
using mgtest.Entities;
using mgtest.Interfaces;
using mgtest.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteSheet = mgtest.Types.SpriteSheet;

namespace mgtest.Components;

public class SpriteAnimator : IComponent {
  // Dependencies
  private readonly Entity _owner;
  private readonly IPhysics _physics;
  private readonly Dictionary<CharacterAnimationType, AnimationDefinition> _animations;
  private readonly SpriteSheet _spriteSheet;

  // State
  private CharacterAnimationType _currentAnimationType;
  private AnimationDefinition _currentAnimDef;
  private float _timer;
  private int _currentFrameIndex;

  // Lifecycle
  public SpriteAnimator(
    Entity owner,
    SpriteSheet spriteSheet,
    Dictionary<CharacterAnimationType, AnimationDefinition> animations,
    IPhysics physics
  ) {
    _owner = owner;
    _spriteSheet = spriteSheet;
    _physics = physics;
    _animations = animations;

    SetAnimation(CharacterAnimationType.Idle);
  }

  public void Update(GameTime gameTime) {
    var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
    _timer += dt;

    CharacterAnimationType nextAnimation = _physics == null
      ? _currentAnimationType
      : GetNextAnimationTypeFromPhysics();

    if (nextAnimation != _currentAnimationType) {
      SetAnimation(nextAnimation);
    }

    if (_timer >= _currentAnimDef.FrameTime) {
      _currentFrameIndex++;

      if (_currentFrameIndex > _currentAnimDef.EndFrame) {
        _currentFrameIndex = _currentAnimDef.StartFrame;
      }

      _timer = 0f;
    }
  }

  public void Draw(SpriteBatch spriteBatch) {
    int row, col;
    if (_currentAnimDef.AnimateAcrossColumns) {
      row = _currentAnimDef.Row;
      col = _currentFrameIndex;
    }
    else {
      col = _currentAnimDef.Row;
      row = _currentFrameIndex;
    }

    var sourceRect = new Rectangle(
      col * _spriteSheet.FrameWidth,
      row * _spriteSheet.FrameHeight,
      _spriteSheet.FrameWidth,
      _spriteSheet.FrameHeight
    );

    var spriteEffect = SpriteEffects.None;
    if (_physics != null) {
      spriteEffect = _physics.IsFacingRight
        ? SpriteEffects.None
        : SpriteEffects.FlipHorizontally;

      if (_physics.IsWallSliding) {
        spriteEffect = _physics.IsTouchingWallRight
          ? SpriteEffects.FlipHorizontally
          : SpriteEffects.None;
      }
    }

    spriteBatch.Draw(
      _spriteSheet.Texture,
      _owner.Position,
      sourceRect,
      Color.White,
      _owner.Rotation,
      Vector2.Zero,
      _owner.Scale,
      spriteEffect,
      0f
    );
  }

  // Helpers
  private void SetAnimation(CharacterAnimationType animType) {
    if (!_animations.TryGetValue(animType, out AnimationDefinition newAnimDef)) {
      throw new KeyNotFoundException(
        $"Animation type {animType} not found in _animations dictionary."
      );
    }

    _currentAnimationType = animType;
    _currentAnimDef = newAnimDef;
    _timer = 0f;
    _currentFrameIndex = _currentAnimDef.StartFrame;
  }

  private CharacterAnimationType GetNextAnimationTypeFromPhysics() {
    if (_physics == null) {
      return _currentAnimationType;
    }

    if (_physics.IsWallSliding) {
      return CharacterAnimationType.WallSlide;
    }

    if (!_physics.IsGrounded) {
      return CharacterAnimationType.Jump;
    }

    if (_physics.IsGrounded && _physics.MoveInputX != 0f) {
      return CharacterAnimationType.Walk;
    }

    return CharacterAnimationType.Idle;
  }
}