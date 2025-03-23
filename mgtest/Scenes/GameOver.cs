using mgtest.Abstracts;
using mgtest.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Scenes;

public class GameOver(Game1 game) : Scene(game) {
  private double _elapsedTime;

  public override void Update(GameTime gameTime) {
    base.Update(gameTime);

    _elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

    if (_elapsedTime >= Settings.DisplaySecs) {
      Game.SceneManager.ChangeScene(new Title(Game));
      _elapsedTime = -1;
    }
  }

  public override void Draw(SpriteBatch spriteBatch) {
    base.Draw(spriteBatch);

    spriteBatch.Begin();
    BitmapFont.DrawString(spriteBatch, "GAME OVER", new Vector2(114, 76), Color.White);
    spriteBatch.End();
  }
}