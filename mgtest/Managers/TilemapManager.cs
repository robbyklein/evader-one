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
  // Dependencies
  private readonly Game _game;

  // State
  public TiledMap TiledMap;
  public TiledMapRenderer TiledMapRenderer;
  public List<RectangleF> CollisionRectangles { get; private set; }
  public List<RectangleF> WaterRectangles { get; private set; }


  public TilemapManager(Game game) {
    _game = game;
  }

  public void LoadMap(Map map) {
    // Load the Tiled map
    TiledMap = _game.Content.Load<TiledMap>("levels/playground");
    TiledMapRenderer = new TiledMapRenderer(_game.GraphicsDevice, TiledMap);

    CollisionRectangles = new List<RectangleF>();
    WaterRectangles = new List<RectangleF>();

    var collisionLayer = TiledMap.GetLayer<TiledMapObjectLayer>("Collision");
    if (collisionLayer != null) {
      foreach (TiledMapObject obj in collisionLayer.Objects) {
        // Make sure there's a "tag" property
        if (!obj.Properties.ContainsKey("tag")) {
          continue;
        }

        var tag = obj.Properties["tag"].ToString();
        var rect = new RectangleF(obj.Position.X, obj.Position.Y, obj.Size.Width, obj.Size.Height);

        if (tag == "solid") {
          CollisionRectangles.Add(rect);
        }
        else if (tag == "water") {
          WaterRectangles.Add(rect);
        }
      }
    }
  }

  public void UnloadMap() {
    CollisionRectangles?.Clear();
    WaterRectangles?.Clear();

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