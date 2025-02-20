using System;
using System.Collections.Generic;
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
  private List<Entity> _entities; // List to store all entities (coins, etc.)
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

    // Parse the Entities layer from the Tiled map.
    var entityLayer = _tilemapManager.TiledMap.GetLayer<TiledMapObjectLayer>("Entities");
    if (entityLayer != null) {
      Console.WriteLine($"Entities layer found with {entityLayer.Objects.Length} objects.");
      foreach (TiledMapObject obj in entityLayer.Objects) {
        // Determine the object type from either the built-in Type or custom property "type"
        var objType = string.Empty;
        if (!string.IsNullOrEmpty(obj.Type)) {
          objType = obj.Type.ToLower();
        }
        else if (obj.Properties.ContainsKey("type")) {
          objType = obj.Properties["type"].ToString().ToLower();
        }

        // Adjust position so that the spawn occurs at the top left of the object rectangle.
        // (For tile objects, Tiled usually places the position at the bottom left.)
        var position = new Vector2(obj.Position.X, obj.Position.Y - obj.Size.Height);
        Console.WriteLine($"Found object of type '{objType}' at position {position}");

        switch (objType) {
          case "player":
            _playerEntity = new PlayerEntity(Game.Content, _tilemapManager, _sfxManager);
            _playerEntity.Position = position;
            _entities.Add(_playerEntity);
            break;
          case "coin":
            var coin = new CoinEntity(Game.Content);
            coin.Position = position;
            _entities.Add(coin);
            break;
          // Add additional cases for other entity types as needed.
          default:
            Console.WriteLine($"Unrecognized entity type '{objType}'.");
            break;
        }
      }
    }
    else {
      Console.WriteLine("Entities layer not found!");
    }

    // Fallback: if no player entity was created from the map, create one at a default position.
    if (_playerEntity == null) {
      Console.WriteLine("No player entity found in the Entities layer; creating default player.");
      _playerEntity = new PlayerEntity(Game.Content, _tilemapManager, _sfxManager);
      _playerEntity.Position = new Vector2(100, 100); // default position
      _entities.Add(_playerEntity);
    }

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

    if (_playerEntity != null) {
      _camera.Update(_playerEntity.Position, _tilemapManager.TiledMap);
    }
  }

  public override void Draw(SpriteBatch spriteBatch) {
    // --- Step 1: Draw the tilemap without the foreground layer ---
    // Save original visibility for each layer.
    var originalVisibilities = new Dictionary<string, bool>();
    foreach (TiledMapLayer layer in _tilemapManager.TiledMap.Layers) {
      originalVisibilities[layer.Name] = layer.IsVisible;
      // Hide the foreground layer so it is not drawn in this pass.
      if (layer.Name.Equals("Foreground", StringComparison.OrdinalIgnoreCase)) {
        layer.IsVisible = false;
      }
    }

    // Draw the tilemap (this draws Background and Main layers).
    _tilemapManager.TiledMapRenderer.Draw(_camera.Transform);

    // --- Step 2: Draw the entities (sprites) ---
    spriteBatch.Begin(transformMatrix: _camera.Transform);
    foreach (Entity entity in _entities) {
      entity.Draw(spriteBatch);
    }

    spriteBatch.End();

    // --- Step 3: Draw only the foreground layer with point filtering ---
    // Hide all layers except the Foreground layer.
    foreach (TiledMapLayer layer in _tilemapManager.TiledMap.Layers) {
      if (!layer.Name.Equals("Foreground", StringComparison.OrdinalIgnoreCase)) {
        layer.IsVisible = false;
      }
      else {
        layer.IsVisible = true;
      }
    }

    // Set the sampler state to point clamp to avoid blur.
    Game.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
    // Draw the tilemap (this now draws only the Foreground layer).
    _tilemapManager.TiledMapRenderer.Draw(_camera.Transform);

    // --- Step 4: Restore original layer visibilities ---
    foreach (TiledMapLayer layer in _tilemapManager.TiledMap.Layers) {
      if (originalVisibilities.ContainsKey(layer.Name)) {
        layer.IsVisible = originalVisibilities[layer.Name];
      }
    }
  }
}