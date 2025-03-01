using System;
using System.Collections.Generic;
using mgtest.Components;
using mgtest.Entities;
using mgtest.Managers;
using mgtest.Types;
using mgtest.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Tiled;

namespace mgtest.Abstracts;

public abstract class GameScene : Scene {
  protected List<Entity> Entities = new();
  protected Entity Player;

  protected GameScene(Game1 game) : base(game) {
  }

  // Overload to load scene-specific content.
  protected virtual void LoadContent(Map map, string songAsset) {
    base.LoadContent();
    TilemapManager.LoadMap(map);
    BackgroundSong = Game.Content.Load<Song>(songAsset);

    TiledMapObjectLayer entityLayer = TilemapManager.GetLayer(MapLayer.Entities);
    EntityBuilder.LoadEntities(
      entityLayer,
      Entities,
      ref Player,
      Game.Content,
      TilemapManager,
      SfxManager,
      InputManager
    );

    MediaPlayer.Volume = 0.0f;
    MediaPlayer.Play(BackgroundSong);
  }

  public override void Update(GameTime gameTime) {
    TilemapManager.Update(gameTime);

    foreach (Entity entity in Entities) {
      entity.Update(gameTime);
    }

    if (Player != null) {
      Camera.Update(Player.Position, TilemapManager.TiledMap);
    }

    // Perform collision checks (e.g. coin collisions)
    CheckCoinCollisions();
  }

  /// <summary>
  ///   Checks for collisions between the player and coin entities.
  /// </summary>
  protected virtual void CheckCoinCollisions() {
    var playerCollider = Player?.GetComponent<Collider>();
    if (playerCollider != null) {
      // Iterate over a copy so that we can modify Entities safely.
      foreach (Entity entity in new List<Entity>(Entities)) {
        if (entity == Player) {
          continue;
        }

        var coinCollider = entity.GetComponent<Collider>();
        if (coinCollider != null && playerCollider.Bounds.Intersects(coinCollider.Bounds)) {
          if (entity is CoinEntity) {
            Entities.Remove(entity);
            SfxManager.PlaySound(Sfx.Collect);
          }
        }
      }
    }
  }

  public override void Draw(SpriteBatch spriteBatch) {
    // Save original layer visibilities.
    var originalVisibilities = new Dictionary<string, bool>();
    foreach (TiledMapLayer layer in TilemapManager.TiledMap.Layers) {
      originalVisibilities[layer.Name] = layer.IsVisible;
      if (layer.Name.Equals("Foreground", StringComparison.OrdinalIgnoreCase)) {
        layer.IsVisible = false;
      }
    }

    // Draw background and main layers.
    TilemapManager.TiledMapRenderer.Draw(Camera.Transform);

    // Draw entities.
    spriteBatch.Begin(transformMatrix: Camera.Transform);
    foreach (Entity entity in Entities) {
      entity.Draw(spriteBatch);
    }

    spriteBatch.End();

    // Draw foreground layers.
    foreach (TiledMapLayer layer in TilemapManager.TiledMap.Layers) {
      layer.IsVisible = layer.Name.Equals("Foreground", StringComparison.OrdinalIgnoreCase);
    }

    Game.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
    TilemapManager.TiledMapRenderer.Draw(Camera.Transform);

    // Restore original visibilities.
    foreach (TiledMapLayer layer in TilemapManager.TiledMap.Layers) {
      if (originalVisibilities.TryGetValue(layer.Name, out bool isVisible)) {
        layer.IsVisible = isVisible;
      }
    }
  }
}