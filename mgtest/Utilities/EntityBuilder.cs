using System;
using System.Collections.Generic;
using mgtest.Entities;
using mgtest.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Tiled;

namespace mgtest.Utilities;

public static class EntityBuilder {
  public static void LoadEntities(
    TiledMapObjectLayer layer,
    List<Entity> entities,
    ref Entity playerEntity,
    ContentManager content,
    TilemapManager tilemapManager,
    SfxManager sfxManager,
    InputManager inputManager
  ) {
    foreach (TiledMapObject obj in layer.Objects) {
      // Get the object type
      var objType = string.Empty;

      if (!string.IsNullOrEmpty(obj.Type)) {
        objType = obj.Type.ToLower();
      }
      else if (obj.Properties.ContainsKey("type")) {
        objType = obj.Properties["type"].ToString().ToLower();
      }
      
      // Create object position
      var position = new Vector2(obj.Position.X, obj.Position.Y - obj.Size.Height);

      switch (objType) {
        case "player":
          playerEntity = new PlayerEntity(content, tilemapManager, sfxManager, inputManager);
          playerEntity.Position = position;
          entities.Add(playerEntity);
          break;
        case "coin":
          var coin = new CoinEntity(content);
          coin.Position = position;
          entities.Add(coin);
          break;
      }
    }
  }
}