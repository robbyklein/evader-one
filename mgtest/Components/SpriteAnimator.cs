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
  private readonly Physics _physics;
  private readonly Dictionary<CharacterAnimationType, AnimationDefinition> _animations;
  private readonly SpriteSheet _spriteSheet;

  // State
  private CharacterAnimationType _currentAnimationType;
  private AnimationDefinition _currentAnimDef;
  private float _timer;
  private int _currentFrameCol;

  public SpriteAnimator(
    Entity owner,
    SpriteSheet spriteSheet,
    Dictionary<CharacterAnimationType, AnimationDefinition> animations
  ) {
    // Initialize everything
    _owner = owner;
    _spriteSheet = spriteSheet;
    _physics = _owner.GetComponent<Physics>();
    _animations = animations;

    // Set initial animation
    SetAnimation(CharacterAnimationType.Idle);
  }

  public void SetAnimation(CharacterAnimationType animType) {
    // Ensure the animation exists in the dictionary
    if (!_animations.TryGetValue(animType, out AnimationDefinition newAnimDef)) {
      throw new KeyNotFoundException($"Animation type {animType} not found in _animations dictionary.");
    }

    // If already set, still update `_currentAnimDef` to ensure it's not null
    if (_currentAnimationType == animType) {
      _currentAnimDef = newAnimDef; // This ensures `_currentAnimDef` is always set
      return;
    }

    _currentAnimationType = animType;
    _currentAnimDef = newAnimDef;
    _timer = 0f;
    _currentFrameCol = _currentAnimDef.StartFrame;
  }

  public void Update(GameTime gameTime) {
    // Update the timer
    var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
    _timer += deltaTime;

    // Update animation type 
    CharacterAnimationType nextAnimation = GetNextAnimationType();
    if (nextAnimation != _currentAnimationType) {
      SetAnimation(nextAnimation);
    }

    // Update the sprite
    if (_timer >= _currentAnimDef.FrameTime) {
      _currentFrameCol++;

      if (_currentFrameCol > _currentAnimDef.EndFrame) {
        _currentFrameCol = _currentAnimDef.StartFrame;
      }

      _timer = 0f;
    }
  }

  public CharacterAnimationType GetNextAnimationType() {
    if (_physics.MoveInputX != 0 && _physics.IsGrounded) {
      return CharacterAnimationType.Walk;
    }

    if (!_physics.IsGrounded) {
      return CharacterAnimationType.Jump;
    }

    return CharacterAnimationType.Idle;
  }

  public void Draw(SpriteBatch spriteBatch) {
    int row = _currentAnimDef.Row;
    var sourceRect = new Rectangle(
      _currentFrameCol * _spriteSheet.FrameWidth,
      row * _spriteSheet.FrameHeight,
      _spriteSheet.FrameWidth,
      _spriteSheet.FrameHeight
    );

    // Determine the sprite effect based on movement direction
    SpriteEffects spriteEffect = _physics.IsFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

    spriteBatch.Draw(
      _spriteSheet.Texture,
      _owner.Position,
      sourceRect,
      Color.White,
      _owner.Rotation,
      Vector2.Zero, // Origin (top-left corner)
      _owner.Scale,
      spriteEffect, // Apply flipping effect
      0f
    );
  }
}