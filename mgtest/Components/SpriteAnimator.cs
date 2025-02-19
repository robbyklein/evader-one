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
    _owner = owner;
    _spriteSheet = spriteSheet;
    _physics = _owner.GetComponent<Physics>();
    _animations = animations;

    // Start idle
    SetAnimation(CharacterAnimationType.Idle);
  }

  public void SetAnimation(CharacterAnimationType animType) {
    // Ensure it exists
    if (!_animations.TryGetValue(animType, out AnimationDefinition newAnimDef)) {
      throw new KeyNotFoundException(
        $"Animation type {animType} not found in _animations dictionary."
      );
    }

    // If same as current, just update definition
    if (_currentAnimationType == animType) {
      _currentAnimDef = newAnimDef;
      return;
    }

    // Otherwise switch animation
    _currentAnimationType = animType;
    _currentAnimDef = newAnimDef;
    _timer = 0f;
    _currentFrameCol = _currentAnimDef.StartFrame;
  }

  public void Update(GameTime gameTime) {
    var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
    _timer += dt;

    // Figure out which animation we should be in
    CharacterAnimationType nextAnimation = GetNextAnimationType();
    if (nextAnimation != _currentAnimationType) {
      SetAnimation(nextAnimation);
    }

    // Advance frames if enough time
    if (_timer >= _currentAnimDef.FrameTime) {
      _currentFrameCol++;
      if (_currentFrameCol > _currentAnimDef.EndFrame) {
        _currentFrameCol = _currentAnimDef.StartFrame;
      }

      _timer = 0f;
    }
  }

  public CharacterAnimationType GetNextAnimationType() {
    // 1) If wall sliding, use that animation if it exists
    if (_physics.IsWallSliding) {
      return CharacterAnimationType.WallSlide;
    }

    // 2) If airborne and not wall sliding
    if (!_physics.IsGrounded) {
      return CharacterAnimationType.Jump;
    }

    // 3) If grounded + horizontal input
    if (_physics.IsGrounded && _physics.MoveInputX != 0f) {
      return CharacterAnimationType.Walk;
    }

    // 4) Otherwise idle
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

    // Default flipping logic based on facing
    SpriteEffects spriteEffect = _physics.IsFacingRight
      ? SpriteEffects.None
      : SpriteEffects.FlipHorizontally;

    // If you want the sprite to always face the wall while sliding,
    // you could override the facing here. Example:
    if (_physics.IsWallSliding) {
      if (_physics.IsTouchingWallRight) {
        spriteEffect = SpriteEffects.FlipHorizontally; // face left
      }
      else {
        spriteEffect = SpriteEffects.None; // face right
      }
    }

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