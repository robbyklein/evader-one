using mgtest.Abstracts;
using mgtest.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Entities;

public class LevelHud {
  private readonly Game1 _game;
  private readonly LevelScene _scene;


  public LevelHud(Game1 game, LevelScene scene) {
    _game = game;
    _scene = scene;
  }

  public void Update(GameTime gameTime) {
  }

  private string BuildTop() {
    var left = $" C{GameData.Coins}/{_scene.LevelInfo.Coins} W{GameData.CurrentLevel} H{GameData.Lifes}";
    var right = $"P{GameData.Points.ToString("D6")} T{_scene.LevelInfo.Time.ToString("D3")} ";

    var totalLength = 40;
    int spaceCount = totalLength - left.Length - right.Length;
    if (spaceCount < 0) {
      spaceCount = 0;
    }

    var spacer = new string(' ', spaceCount);

    return left + spacer + right;
  }


  public void Draw(SpriteBatch spriteBatch) {
    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

    _scene.BitmapFont.DrawString(spriteBatch, BuildTop(), Vector2.Zero, Color.White);
    _scene.BitmapFont.DrawString(spriteBatch, "________________________________________", new Vector2(0, 8),
      Color.White);

    spriteBatch.End();
  }
}