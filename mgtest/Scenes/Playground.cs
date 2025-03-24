using mgtest.Abstracts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Scenes;

public class Playground : LevelScene {
  public Playground(Game1 game, string levelString) : base(game, levelString) {
  }

  public override void LoadContent() {
    base.LoadContent(LevelInfo.Tilemap, LevelInfo.SongAsset);
  }

  public override void UnloadContent() {
    Player = null;
    Entities.Clear();
    TilemapManager = null;
  }

  public override void Update(GameTime gameTime) {
    var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

    InputManager.Update(gameTime);

    base.Update(gameTime);
  }

  public override void Draw(SpriteBatch spriteBatch) {
    base.Draw(spriteBatch);
  }
}