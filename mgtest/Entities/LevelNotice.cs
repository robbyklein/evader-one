using mgtest.Abstracts;
using mgtest.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Entities;

public class LevelNotice {
  private readonly Game1 _game;
  private readonly LevelScene _scene;
  private readonly Texture2D _pixel;

  private string _text = "";

  public LevelNotice(Game1 game, LevelScene scene) {
    _game = game;
    _scene = scene;

    _pixel = new Texture2D(game.GraphicsDevice, 1, 1);
    _pixel.SetData(new[] { Color.White });
  }

  public void Update(GameTime gameTime) {
  }

  public void SetText(string text) {
    _text = text;
  }


  public void Draw(SpriteBatch spriteBatch) {
    // Only draw when text is present
    if (_text == "") {
      return;
    }

    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

    // Bar
    int screenWidth = Settings.VirtualWidth;
    int screenHeight = Settings.VirtualHeight;
    var barHeight = 16;
    var barRect = new Rectangle(0, (screenHeight - barHeight) / 2, screenWidth, barHeight);
    spriteBatch.Draw(_pixel, barRect, Color.White);

    // Text
    int textWidth = _text.Length * Settings.TileSize;
    var textHeight = 8;
    var textPosition = new Vector2((screenWidth - textWidth) / 2, (screenHeight - textHeight) / 2);
    _scene.BitmapFont.DrawString(spriteBatch, _text, textPosition, Color.Black);

    spriteBatch.End();
  }
}