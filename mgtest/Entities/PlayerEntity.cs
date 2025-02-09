using System.Collections.Generic;
using mgtest.Components;
using mgtest.Managers;
using mgtest.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Entities;

public class PlayerEntity : Entity {
  // Configuration
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

  public PlayerEntity(ContentManager content, TilemapManager tilemapManager) {
    // start location
    Position = new Vector2(100.5f, 100.5f);

    // Initialize SpriteSheet
    var spriteSheet = new SpriteSheet {
      Rows = 48,
      Columns = 14,
      Texture = content.Load<Texture2D>("sprites/player")
    };

    // Initialize Components
    var physicsComponent = new Physics(
      this, tilemapManager.GroundTiles,
      tilemapManager.TiledMap.TileWidth,
      tilemapManager.TiledMap.TileHeight
    );
    AddComponent(physicsComponent);

    var possessedComponent = new PlayerPossessed(this);
    AddComponent(possessedComponent);

    var animationComponent = new SpriteAnimator(this, spriteSheet, _animations);
    AddComponent(animationComponent);
  }
}