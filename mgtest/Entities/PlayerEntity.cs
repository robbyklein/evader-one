using System.Collections.Generic;
using mgtest.Components;
using mgtest.Managers;
using mgtest.Types;
using mgtest.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Entities;

public class PlayerEntity : Entity {
  // Configuration for animations
  private readonly Dictionary<CharacterAnimationType, AnimationDefinition> _animations = new() {
    [CharacterAnimationType.Idle] = new AnimationDefinition {
      Row = 14,
      StartFrame = 0,
      EndFrame = 1,
      FrameTime = 0.4f
    },
    [CharacterAnimationType.Walk] = new AnimationDefinition {
      Row = 9,
      StartFrame = 0,
      EndFrame = 1,
      FrameTime = 0.15f
    },
    [CharacterAnimationType.Jump] = new AnimationDefinition {
      Row = 10,
      StartFrame = 0,
      EndFrame = 0,
      FrameTime = 100f
    }
  };

  // Configuration for colliders
  private readonly PhysicsSize _physicsSize = new(4, 8, 2);

  public PlayerEntity(ContentManager content, TilemapManager tilemapManager, SfxManager sfxManager) {
    Position = new Vector2(50f, 20f);

    var spriteSheet = new SpriteSheet {
      Rows = 48,
      Columns = 14,
      Texture = content.Load<Texture2D>("sprites/player")
    };

    // Initialize Components:
    var physicsComponent =
      new Physics(this, _physicsSize, sfxManager, tilemapManager.CollisionRectangles, tilemapManager.WaterRectangles);
    AddComponent(physicsComponent);

    var possessedComponent = new PlayerPossessed(this);
    AddComponent(possessedComponent);

    var animationComponent = new SpriteAnimator(this, spriteSheet, _animations);
    AddComponent(animationComponent);
  }
}