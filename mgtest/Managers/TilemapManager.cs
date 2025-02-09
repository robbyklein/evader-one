using System.Collections.Generic;
using Microsoft.Xna.Framework;
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
  public HashSet<Point> GroundTiles;
  public TiledMap TiledMap;
  public TiledMapRenderer TiledMapRenderer;

  public TilemapManager(Game game) {
    _game = game;
  }

  public void LoadMap(Map map) {
    GroundTiles = new HashSet<Point>();

    var assetName = MapAssetNames.TryGetValue(map, out var name) ? name : "levels/unknown";
    TiledMap = _game.Content.Load<TiledMap>(assetName);
    TiledMapRenderer = new TiledMapRenderer(_game.GraphicsDevice, TiledMap);

    // Process tile properties
    foreach (var layer in TiledMap.TileLayers) {
      for (var y = 0; y < layer.Height; y++) {
        for (var x = 0; x < layer.Width; x++) {
          var tile = layer.GetTile((ushort)x, (ushort)y);
          var globalTileId = tile.GlobalIdentifier & 0x3FFFFFFF;
          MapTileProperties(globalTileId, x, y);
        }
      }
    }
  }

  public void UnloadMap() {
    GroundTiles?.Clear();
    GroundTiles = null;

    TiledMapRenderer?.Dispose();
    TiledMapRenderer = null;

    TiledMap = null;
  }

  public void Update(GameTime gameTime) {
    if (TiledMapRenderer != null) {
      TiledMapRenderer.Update(gameTime);
    }
  }

  public void Draw() {
    if (TiledMapRenderer != null) {
      TiledMapRenderer.Draw();
    }
  }

  private void MapTileProperties(int globalTileId, int x, int y) {
    if (TiledMap == null) {
      return;
    }

    var tileset = TiledMap.GetTilesetByTileGlobalIdentifier(globalTileId);
    if (tileset == null) {
      return;
    }

    var firstGid = TiledMap.GetTilesetFirstGlobalIdentifier(tileset);
    var localTileId = globalTileId - firstGid;

    var tileData = tileset.Tiles.Find(t => t.LocalTileIdentifier == localTileId);
    if (tileData?.Properties != null && tileData.Properties.TryGetValue("ground", out var value) && value == "true") {
      GroundTiles.Add(new Point(x, y));
    }
  }
}