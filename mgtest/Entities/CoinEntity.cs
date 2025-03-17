using System.Collections.Generic;
using mgtest.Components;
using mgtest.Data;
using mgtest.Types;
using mgtest.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Entities;

public class CoinEntity : Entity {
  private readonly Dictionary<CharacterAnimationType, AnimationDefinition> _animations = new() {
    [CharacterAnimationType.Idle] = new AnimationDefinition {
      Row = 4,
      StartFrame = 4,
      EndFrame = 7,
      FrameTime = 0.1f,
      AnimateAcrossColumns = false
    }
  };

  public CoinEntity(ContentManager content) {
    Position = new Vector2(150f, 100f);

    var spriteSheet = new SpriteSheet {
      Rows = 8,
      Columns = 10,
      Texture = content.Load<Texture2D>("sprites/items")
    };

    var collider = new Collider(this);
    AddComponent(collider);

    var animationComponent = new SpriteAnimator(this, spriteSheet, _animations, new NullPhysics());
    AddComponent(animationComponent);
  }

  public override void OnCollision(Entity other, SfxManager sfxManager, List<Entity> entities) {
    if (other is PlayerEntity) {
      sfxManager.PlaySound(Sfx.Collect);
      GameData.Coins++;
      entities.Remove(this);
    }
  }
}