using System;
using System.Collections.Generic;
using mgtest.Components;
using mgtest.Data;
using mgtest.Entities;
using mgtest.Managers;
using mgtest.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Tiled;

namespace mgtest.Abstracts;

public abstract class GameScene(Game1 game) : Scene(game) {
  // State
  protected List<Entity> Entities = new();
  protected Entity Player;

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

    if (Player != null) {
      Camera.Update(Player.Position, TilemapManager.TiledMap);
    }

    if (GameData.GameFinished || !GameData.GameStarted) {
      return;
    }

    foreach (Entity entity in Entities) {
      entity.Update(gameTime);
    }


    // Perform collision checks (e.g. coin collisions)
    CheckCollisions();
  }

  protected virtual void CheckCollisions() {
    var playerCollider = Player?.GetComponent<Collider>();
    if (playerCollider == null) {
      return;
    }

    foreach (Entity entity in new List<Entity>(Entities)) {
      if (entity == Player) {
        continue;
      }

      var entityCollider = entity.GetComponent<Collider>();
      if (entityCollider != null && playerCollider.Bounds.Intersects(entityCollider.Bounds)) {
        entity.OnCollision(Player, SfxManager, Entities);
        Player.OnCollision(entity, SfxManager, Entities);
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