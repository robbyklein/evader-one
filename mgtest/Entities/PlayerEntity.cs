using System.Collections.Generic;
using mgtest.Abstracts;
using mgtest.Components;
using mgtest.Managers;
using mgtest.Types;
using mgtest.Utilities;
using Microsoft.Xna.Framework;
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
    },
    [CharacterAnimationType.WallSlide] = new AnimationDefinition {
      Row = 12,
      StartFrame = 0,
      EndFrame = 0,
      FrameTime = 100f
    }
  };

  public PlayerEntity(Game1 game, TilemapManager tilemapManager, SfxManager sfxManager,
    InputManager inputManager, Scene scene) {
    Position = new Vector2(50f, 20f);

    var spriteSheet = new SpriteSheet {
      Rows = 48,
      Columns = 14,
      Texture = game.Content.Load<Texture2D>("sprites/player")
    };

    // Initialize Components:
    var collider = new Collider(this, 6, 8, 1);
    AddComponent(collider);

    // Add the Physics
    var physics = new Physics(this,
      scene,
      sfxManager,
      tilemapManager.CollisionRectangles,
      tilemapManager.WaterRectangles,
      tilemapManager.NoWallRectangles,
      collider,
      tilemapManager.FinishRectangle
    );
    AddComponent(physics);

    var possessedComponent = new PlayerPossessed(this, sfxManager, physics, inputManager);
    AddComponent(possessedComponent);

    var animationComponent = new SpriteAnimator(this, spriteSheet, _animations, physics);
    AddComponent(animationComponent);
  }
}