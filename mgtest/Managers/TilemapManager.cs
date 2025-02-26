using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace mgtest.Managers;

public enum Map {
  Splash,
  Playground,
  Title
}

public enum MapLayer {
  Foreground,
  Background,
  Entities,
  Collision
}

public class TilemapManager {
  private readonly Game game;

  public TiledMap TiledMap;
  public TiledMapRenderer TiledMapRenderer;
  public List<RectangleF> CollisionRectangles { get; private set; }

  public List<RectangleF> WaterRectangles { get; private set; }

  // New list to hold objects that should not count as walls for wall jumping.
  public List<RectangleF> NoWallRectangles { get; private set; }

  public TilemapManager(Game game) {
    this.game = game;
  }

  public void Update(GameTime gameTime) {
    TiledMapRenderer?.Update(gameTime);
  }

  public void Draw() {
    TiledMapRenderer?.Draw();
  }

  public void LoadMap(Map map) {
    switch (map) {
      case Map.Splash:
        TiledMap = game.Content.Load<TiledMap>("levels/splash");
        break;
      case Map.Playground:
        TiledMap = game.Content.Load<TiledMap>("levels/playground");
        break;
      case Map.Title:
        TiledMap = game.Content.Load<TiledMap>("levels/title");
        break;
    }

    TiledMapRenderer = new TiledMapRenderer(game.GraphicsDevice, TiledMap);
    LoadHybridCollisions();
  }

  private void LoadHybridCollisions() {
    CollisionRectangles = new List<RectangleF>();
    WaterRectangles = new List<RectangleF>();
    NoWallRectangles = new List<RectangleF>();

    // 1. Load collisions from the dedicated object layer (if available)
    var collisionLayer = TiledMap.GetLayer<TiledMapObjectLayer>("Collision");
    if (collisionLayer != null) {
      foreach (TiledMapObject obj in collisionLayer.Objects) {
        var rect = new RectangleF(obj.Position.X, obj.Position.Y, obj.Size.Width, obj.Size.Height);

        if (obj.Properties.TryGetValue("tag", out string tag)) {
          if (tag == "solid") {
            CollisionRectangles.Add(rect);
          }
          else if (tag == "water") {
            WaterRectangles.Add(rect);
          }
          else if (tag == "no walls") {
            // Collide normally but ignore for wall jumping.
            CollisionRectangles.Add(rect);
            NoWallRectangles.Add(rect);
          }
        }
        else {
          CollisionRectangles.Add(rect);
        }
      }
    }

    // 2. Load collisions from tile layers (embedded collision data)
    foreach (TiledMapTileLayer tileLayer in TiledMap.TileLayers) {
      for (var y = 0; y < tileLayer.Height; y++) {
        for (var x = 0; x < tileLayer.Width; x++) {
          TiledMapTile tile = tileLayer.GetTile((ushort)x, (ushort)y);
          if (tile.IsBlank) {
            continue;
          }

          TiledMapTileset tileSet = TiledMap.GetTilesetByTileGlobalIdentifier(tile.GlobalIdentifier);
          if (tileSet == null) {
            continue;
          }

          int firstGlobal = TiledMap.GetTilesetFirstGlobalIdentifier(tileSet);
          int localTileId = tile.GlobalIdentifier - firstGlobal;

          TiledMapTilesetTile tileDefinition = tileSet.Tiles.FirstOrDefault(t => t.LocalTileIdentifier == localTileId);
          if (tileDefinition != null) {
            foreach (TiledMapObject collisionObject in tileDefinition.Objects) {
              float worldX = x * TiledMap.TileWidth + collisionObject.Position.X;
              float worldY = y * TiledMap.TileHeight + collisionObject.Position.Y;
              var rect = new RectangleF(worldX, worldY, collisionObject.Size.Width, collisionObject.Size.Height);

              if (collisionObject.Properties.TryGetValue("tag", out string tag)) {
                if (tag == "water") {
                  WaterRectangles.Add(rect);
                }
                else if (tag == "solid") {
                  CollisionRectangles.Add(rect);
                }
                else if (tag == "no walls") {
                  CollisionRectangles.Add(rect);
                  NoWallRectangles.Add(rect);
                }
              }
              else {
                CollisionRectangles.Add(rect);
              }
            }
          }
        }
      }
    }
  }

  public void UnloadMap() {
    CollisionRectangles?.Clear();
    WaterRectangles?.Clear();
    NoWallRectangles?.Clear();

    TiledMapRenderer?.Dispose();
    TiledMapRenderer = null;
    TiledMap = null;
  }

  public TiledMapObjectLayer GetLayer(MapLayer layer) {
    switch (layer) {
      case MapLayer.Foreground:
        return TiledMap.GetLayer<TiledMapObjectLayer>("Foreground");
      case MapLayer.Background:
        return TiledMap.GetLayer<TiledMapObjectLayer>("Background");
      case MapLayer.Entities:
        return TiledMap.GetLayer<TiledMapObjectLayer>("Entities");
      case MapLayer.Collision:
        return TiledMap.GetLayer<TiledMapObjectLayer>("Collision");
      default:
        return null;
    }
  }
}