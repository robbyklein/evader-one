using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace mgtest.Managers;

public enum Map {
  Splash,
  Playground
}

public class TilemapManager {
  private static readonly Dictionary<Map, string> MapAssetNames = new() {
    { Map.Splash, "levels/splash" },
    { Map.Playground, "levels/playground" }
  };

  private readonly Game _game;
  public TiledMap TiledMap;
  public TiledMapRenderer TiledMapRenderer;

  // New: List of collision rectangles loaded from an object layer
  public List<RectangleF> CollisionRectangles { get; private set; }

  public TilemapManager(Game game) {
    _game = game;
  }

  public void LoadMap(Map map) {
    string assetName = MapAssetNames.TryGetValue(map, out string name) ? name : "levels/unknown";
    TiledMap = _game.Content.Load<TiledMap>(assetName);
    TiledMapRenderer = new TiledMapRenderer(_game.GraphicsDevice, TiledMap);

    // Load collision objects from the "Colliders" object layer
    CollisionRectangles = new List<RectangleF>();
    var collisionLayer = TiledMap.GetLayer<TiledMapObjectLayer>("Collision");
    if (collisionLayer != null) {
      foreach (TiledMapObject obj in collisionLayer.Objects) {
        Console.WriteLine(obj.Properties);
        // Only add objects that have the tag "solid"
        if (obj.Properties.ContainsKey("tag") && obj.Properties["tag"].ToString() == "solid") {
          var rect = new RectangleF(obj.Position.X, obj.Position.Y, obj.Size.Width, obj.Size.Height);
          CollisionRectangles.Add(rect);
        }
      }
    }
  }

  public void UnloadMap() {
    CollisionRectangles?.Clear();
    CollisionRectangles = null;

    TiledMapRenderer?.Dispose();
    TiledMapRenderer = null;

    TiledMap = null;
  }

  public void Update(GameTime gameTime) {
    TiledMapRenderer?.Update(gameTime);
  }

  public void Draw() {
    TiledMapRenderer?.Draw();
  }
}