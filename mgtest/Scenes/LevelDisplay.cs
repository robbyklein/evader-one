using mgtest.Abstracts;
using mgtest.Config;
using mgtest.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Scenes;

public class LevelDisplay(Game1 game, string levelString) : Scene(game) {
  private double _elapsedTime;

  public override void Update(GameTime gameTime) {
    base.Update(gameTime);

    _elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

    if (_elapsedTime >= Settings.DisplaySecs) {
      Game.SceneManager.ChangeScene(new Playground(Game, levelString));
      _elapsedTime = -1;
    }
  }

  public override void Draw(SpriteBatch spriteBatch) {
    base.Draw(spriteBatch);

    var left = 24;
    var top = 40;
    LevelInfo levelData = LevelData.GetLevelInfo(levelString, GameData.Difficulty.ToString());

    spriteBatch.Begin();
    BitmapFont.DrawString(spriteBatch, $"W{GameData.CurrentLevel}", new Vector2(left, top), Color.White);
    BitmapFont.DrawString(spriteBatch, "+pass:", new Vector2(left, top + 32 + 16), Color.White);
    BitmapFont.DrawString(spriteBatch, $"C{levelData.Coins}", new Vector2(left, top + 48 + 16), Color.White);
    BitmapFont.DrawString(spriteBatch, $"P{levelData.ScoreThreshold}", new Vector2(left, top + 64 + 16), Color.White);
    BitmapFont.DrawString(spriteBatch, $"T{levelData.Coins}", new Vector2(left, top + 80 + 16), Color.White);
    spriteBatch.End();
  }
}