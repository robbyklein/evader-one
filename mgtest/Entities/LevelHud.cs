using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Entities;

public class LevelHud {
  private readonly Game1 _game;
  private readonly BitmapFont _bitmapFont;

  // Example HUD properties
  public int Score { get; set; }
  public int Health { get; set; }

  public LevelHud(Game1 game, BitmapFont bitmapFont) {
    _game = game;
    _bitmapFont = bitmapFont;
  }

  public void Update(GameTime gameTime) {
    // Update HUD properties as needed.
  }

  public void Draw(SpriteBatch spriteBatch) {
    // Draw the HUD in screen space.
    spriteBatch.Begin();
    _bitmapFont.DrawString(spriteBatch, $"SCORE: {Score}", new Vector2(10, 10), Color.White);
    _bitmapFont.DrawString(spriteBatch, $"HEALTH: {Health}", new Vector2(10, 20), Color.White);
    spriteBatch.End();
  }
}