using System;
using System.Collections.Generic;
using System.Diagnostics;
using mgtest.Abstracts;
using mgtest.Components;
using mgtest.Entities;
using mgtest.Managers;
using mgtest.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Tiled;

namespace mgtest.Scenes;

public class Playground : Scene {
  private Song _musicPlayer;
  private Entity _playerEntity;
  private List<Entity> _entities;
  private Camera _camera;
  private TilemapManager _tilemapManager;
  private readonly SfxManager _sfxManager;

  public Playground(Game1 game) : base(game) {
    _tilemapManager = new TilemapManager(Game);
    _sfxManager = new SfxManager();
    MediaPlayer.IsRepeating = true;
  }

  public override void LoadContent() {
    _sfxManager.LoadSounds(Game);
    _tilemapManager.LoadMap(Map.Playground);
    _camera = new Camera(Game.GraphicsDevice);
    _musicPlayer = Game.Content.Load<Song>("music/one");
    _entities = new List<Entity>();

    var entityLayer = _tilemapManager.TiledMap.GetLayer<TiledMapObjectLayer>("Entities");
    EntityBuilder.LoadEntities(entityLayer, _entities, ref _playerEntity, Game.Content, _tilemapManager, _sfxManager);

    MediaPlayer.Play(_musicPlayer);
  }

  public override void UnloadContent() {
    _playerEntity = null;
    _entities?.Clear();
    _camera = null;
    _tilemapManager = null;
    _musicPlayer = null;
  }

  public override void Update(GameTime gameTime) {
    _tilemapManager.Update(gameTime);
    foreach (Entity entity in _entities) {
      entity.Update(gameTime);
    }

    // Collision detection between player and coin entities.
    var playerCollider = _playerEntity.GetComponent<Collider>();
    if (playerCollider != null) {
      foreach (Entity entity in new List<Entity>(_entities)) {
        if (entity == _playerEntity) {
          continue;
        }

        var coinCollider = entity.GetComponent<Collider>();
        if (coinCollider != null && playerCollider.Bounds.Intersects(coinCollider.Bounds)) {
          // Debug: Log collision details
          Debug.WriteLine($"Collision detected: Player {playerCollider.Bounds} - Entity {coinCollider.Bounds}");

          // If the colliding entity is a coin, remove it and play a sound.
          if (entity is CoinEntity) {
            _entities.Remove(entity);
            // _sfxManager.PlaySound(Sfx.CollectCoin);
          }
        }
      }
    }

    if (_playerEntity != null) {
      _camera.Update(_playerEntity.Position, _tilemapManager.TiledMap);
    }
  }

  public override void Draw(SpriteBatch spriteBatch) {
    // Save original layer visibilities.
    var originalVisibilities = new Dictionary<string, bool>();
    foreach (TiledMapLayer layer in _tilemapManager.TiledMap.Layers) {
      originalVisibilities[layer.Name] = layer.IsVisible;
      if (layer.Name.Equals("Foreground", StringComparison.OrdinalIgnoreCase)) {
        layer.IsVisible = false;
      }
    }

    // Draw background and main layers.
    _tilemapManager.TiledMapRenderer.Draw(_camera.Transform);

    // Draw entities.
    spriteBatch.Begin(transformMatrix: _camera.Transform);
    foreach (Entity entity in _entities) {
      entity.Draw(spriteBatch);
    }

    spriteBatch.End();

    // Draw the foreground layer with point filtering.
    foreach (TiledMapLayer layer in _tilemapManager.TiledMap.Layers) {
      layer.IsVisible = layer.Name.Equals("Foreground", StringComparison.OrdinalIgnoreCase);
    }

    Game.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
    _tilemapManager.TiledMapRenderer.Draw(_camera.Transform);

    // Restore original visibilities.
    foreach (TiledMapLayer layer in _tilemapManager.TiledMap.Layers) {
      if (originalVisibilities.ContainsKey(layer.Name)) {
        layer.IsVisible = originalVisibilities[layer.Name];
      }
    }
  }
}