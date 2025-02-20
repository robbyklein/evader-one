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
  private readonly Physics? _physics; // optional
  private readonly Dictionary<CharacterAnimationType, AnimationDefinition> _animations;
  private readonly SpriteSheet _spriteSheet;

  // State
  private CharacterAnimationType _currentAnimationType;
  private AnimationDefinition _currentAnimDef;
  private float _timer;

  // Our frame index, which might represent columns or rows depending on AnimateAcrossColumns
  private int _currentFrameIndex;

  public SpriteAnimator(
    Entity owner,
    SpriteSheet spriteSheet,
    Dictionary<CharacterAnimationType, AnimationDefinition> animations
  ) {
    _owner = owner;
    _spriteSheet = spriteSheet;
    _physics = _owner.GetComponent<Physics>(); // might be null
    _animations = animations;

    // Start with a default animation
    SetAnimation(CharacterAnimationType.Idle);
  }

  public void SetAnimation(CharacterAnimationType animType) {
    if (!_animations.TryGetValue(animType, out AnimationDefinition newAnimDef)) {
      throw new KeyNotFoundException(
        $"Animation type {animType} not found in _animations dictionary."
      );
    }

    // Always update the animation definition and reset frame state,
    // even if the animation type is already the current one.
    _currentAnimationType = animType;
    _currentAnimDef = newAnimDef;
    _timer = 0f;
    _currentFrameIndex = _currentAnimDef.StartFrame;
  }

  public void Update(GameTime gameTime) {
    var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
    _timer += dt;

    // If we have physics, figure out which animation to use
    CharacterAnimationType nextAnimation = _physics == null
      ? _currentAnimationType
      : GetNextAnimationTypeFromPhysics();

    if (nextAnimation != _currentAnimationType) {
      SetAnimation(nextAnimation);
    }

    // Advance frames if enough time has passed
    if (_timer >= _currentAnimDef.FrameTime) {
      // Increase frame index
      _currentFrameIndex++;
      // If we exceed EndFrame, wrap around
      if (_currentFrameIndex > _currentAnimDef.EndFrame) {
        _currentFrameIndex = _currentAnimDef.StartFrame;
      }

      // Reset timer
      _timer = 0f;
    }
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

  public void Draw(SpriteBatch spriteBatch) {
    // Figure out row & column from AnimateAcrossColumns
    int row, col;
    if (_currentAnimDef.AnimateAcrossColumns) {
      // Animate horizontally
      row = _currentAnimDef.Row;
      col = _currentFrameIndex;
    }
    else {
      // Animate vertically
      col = _currentAnimDef.Row;
      row = _currentFrameIndex;
    }

    var sourceRect = new Rectangle(
      col * _spriteSheet.FrameWidth,
      row * _spriteSheet.FrameHeight,
      _spriteSheet.FrameWidth,
      _spriteSheet.FrameHeight
    );

    // Default flipping logic
    var spriteEffect = SpriteEffects.None;
    if (_physics != null) {
      spriteEffect = _physics.IsFacingRight
        ? SpriteEffects.None
        : SpriteEffects.FlipHorizontally;

      // If we want the sprite to always face the wall while sliding
      if (_physics.IsWallSliding) {
        spriteEffect = _physics.IsTouchingWallRight
          ? SpriteEffects.FlipHorizontally
          : SpriteEffects.None;
      }
    }


    // Draw the sprite
    spriteBatch.Draw(
      _spriteSheet.Texture,
      _owner.Position,
      sourceRect,
      Color.White,
      _owner.Rotation,
      Vector2.Zero, // origin
      _owner.Scale,
      spriteEffect,
      0f
    );
  }
}