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

public class TilemapManager(Game game) {
  // Dependencies

  // State
  public TiledMap TiledMap;
  public TiledMapRenderer TiledMapRenderer;
  public List<RectangleF> CollisionRectangles { get; private set; }
  public List<RectangleF> WaterRectangles { get; private set; }

  // Lifecycle
  public void Update(GameTime gameTime) {
    TiledMapRenderer?.Update(gameTime);
  }

  public void Draw() {
    TiledMapRenderer?.Draw();
  }

  // Helpers
  public void LoadMap(Map map) {
    // Setup tilemap
    TiledMap = game.Content.Load<TiledMap>("levels/playground");
    TiledMapRenderer = new TiledMapRenderer(game.GraphicsDevice, TiledMap);

    // Load collisions
    var collisionLayer = TiledMap.GetLayer<TiledMapObjectLayer>("Collision");
    LoadCollisions(collisionLayer);
  }

  private void LoadCollisions(TiledMapObjectLayer collisionLayer) {
    // initialize lists
    CollisionRectangles = new List<RectangleF>();
    WaterRectangles = new List<RectangleF>();

    foreach (TiledMapObject obj in collisionLayer.Objects) {
      // Anything importanty will have a tag
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

  public void UnloadMap() {
    CollisionRectangles?.Clear();
    WaterRectangles?.Clear();

    TiledMapRenderer?.Dispose();
    TiledMapRenderer = null;
    TiledMap = null;
  }
}