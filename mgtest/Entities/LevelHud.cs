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


    // // Render base
    // for (var i = 0; i < 40; i++) {
    //   spriteBatch.Draw(_hudSprites, new Vector2(Settings.TileSize * i, 0), _blankTile, Color.White);
    //   spriteBatch.Draw(_hudSprites, new Vector2(Settings.TileSize * i, 8), _dividerTile, Color.White);
    // }
    //
    // // Coins
    // spriteBatch.Draw(_hudSprites, new Vector2(Settings.TileSize * 1, 0), _coinTile, Color.White);
    // DrawNumber(spriteBatch, GameData.Coins, new Vector2(Settings.TileSize * 2, 0), 0, false);
    // spriteBatch.Draw(_hudSprites, new Vector2(Settings.TileSize * 3, 0), _slashTile, Color.White);
    // DrawNumber(spriteBatch, _scene.LevelInfo.Coins, new Vector2(Settings.TileSize * 4, 0), 0, false);
    //
    // // Stage
    // spriteBatch.Draw(_hudSprites, new Vector2(Settings.TileSize * 6, 0), _worldTile, Color.White);
    // DrawNumber(spriteBatch, GameData.World, new Vector2(Settings.TileSize * 7, 0), 0, false);
    // spriteBatch.Draw(_hudSprites, new Vector2(Settings.TileSize * 8, 0), _hyphenTile, Color.White);
    // DrawNumber(spriteBatch, GameData.Level, new Vector2(Settings.TileSize * 9, 0), 0, false);
    //
    // // Lifes
    // spriteBatch.Draw(_hudSprites, new Vector2(Settings.TileSize * 11, 0), _heartTile, Color.White);
    // DrawNumber(spriteBatch, GameData.Lifes, new Vector2(Settings.TileSize * 12, 0));
    //
    // // Points
    // spriteBatch.Draw(_hudSprites, new Vector2(27 * Settings.TileSize, 0), _pointsTile, Color.White);
    // DrawNumber(spriteBatch, GameData.Points, new Vector2(28 * Settings.TileSize, 0), 6);
    //
    // // Time
    // spriteBatch.Draw(_hudSprites, new Vector2(35 * Settings.TileSize, 0), _timeTile, Color.White);
    // DrawNumber(spriteBatch, _scene.LevelInfo.Time, new Vector2(36 * Settings.TileSize, 0), 3);

    spriteBatch.End();
  }

  private void DrawNumber(SpriteBatch spriteBatch, int number, Vector2 position, int padLength = 2,
    bool padWithZeros = true) {
    // string numString = padWithZeros ? number.ToString().PadLeft(padLength, '0') : number.ToString();
    //
    // for (var i = 0; i < numString.Length; i++) {
    //   int digit = numString[i] - '0';
    //   spriteBatch.Draw(_hudSprites, position + new Vector2(i * Settings.TileSize, 0), _digitTiles[digit], Color.White);
    // }
  }
}