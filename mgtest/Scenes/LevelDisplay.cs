using mgtest.Abstracts;
using mgtest.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Scenes;

public class LevelDisplay(Game1 game) : Scene(game) {
  private double _elapsedTime;

  public override void Update(GameTime gameTime) {
    base.Update(gameTime);

    _elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

    if (_elapsedTime >= 3) {
      Game.SceneManager.ChangeScene(new Playground(Game));
      _elapsedTime = -1;
    }
  }

  public override void Draw(SpriteBatch spriteBatch) {
    base.Draw(spriteBatch);

    spriteBatch.Begin();
    BitmapFont.DrawString(spriteBatch, $"{GameData.World} - {GameData.Level}", new Vector2(114, 76), Color.White);
    spriteBatch.End();
  }
}