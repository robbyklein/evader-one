using mgtest.Abstracts;
using mgtest.Entities;
using mgtest.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Scenes;

public class Playground : Scene {
  private Entity _playerEntity;

  public Playground(Game1 game) : base(game) {
  }

  public override void LoadContent() {
    // Map
    Game.TilemapManager.LoadMap(Map.Playground);

    // Entities
    _playerEntity = new PlayerEntity(Game.Content, Game.TilemapManager);
  }

  public override void UnloadContent() {
    _playerEntity = null;
  }

  public override void Update(GameTime gameTime) {
    Game.TilemapManager.Update(gameTime);
    _playerEntity.Update(gameTime);
  }

  public override void Draw(SpriteBatch spriteBatch) {
    Game.TilemapManager.Draw();
    _playerEntity.Draw(spriteBatch);
  }
}