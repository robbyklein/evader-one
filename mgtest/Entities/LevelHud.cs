using System.Collections.Generic;
using mgtest.Config;
using mgtest.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Entities;

public class LevelHud {
  private readonly Game1 _game;
  private readonly Texture2D _hudSprites;
  private readonly Dictionary<int, Rectangle> _digitTiles = new();

  private readonly Rectangle _coinTile = new(16, 8, 8, 8);
  private readonly Rectangle _worldTile = new(24, 8, 8, 8);
  private readonly Rectangle _heartTile = new(32, 8, 8, 8);
  private readonly Rectangle _pointsTile = new(40, 8, 8, 8);
  private readonly Rectangle _timeTile = new(48, 8, 8, 8);
  private readonly Rectangle _blankTile = new(56, 8, 8, 8);
  private readonly Rectangle _dividerTile = new(0, 16, 8, 8);
  private readonly Rectangle _hyphenTile = new(8, 16, 8, 8);

  private int _time = 300;
  private float _timeAccumulator;

  public LevelHud(Game1 game) {
    _game = game;
    _hudSprites = game.Content.Load<Texture2D>("sprites/hud");

    for (var i = 0; i < 8; i++) {
      _digitTiles[i] = new Rectangle(i * 8, 0, 8, 8);
    }

    _digitTiles[8] = new Rectangle(0, 8, 8, 8);
    _digitTiles[9] = new Rectangle(8, 8, 8, 8);
  }

  public void Update(GameTime gameTime) {
    _timeAccumulator += (float)gameTime.ElapsedGameTime.TotalSeconds;

    while (_timeAccumulator >= 1f) {
      _timeAccumulator -= 1f;
      if (_time > 0) {
        _time--;
      }
    }
  }

  public void Draw(SpriteBatch spriteBatch) {
    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

    // Render base
    for (var i = 0; i < 40; i++) {
      spriteBatch.Draw(_hudSprites, new Vector2(Settings.TileSize * i, 0), _blankTile, Color.White);
      spriteBatch.Draw(_hudSprites, new Vector2(Settings.TileSize * i, 8), _dividerTile, Color.White);
    }

    // Coins
    spriteBatch.Draw(_hudSprites, new Vector2(Settings.TileSize * 1, 0), _coinTile, Color.White);
    DrawNumber(spriteBatch, GameData.Coins, new Vector2(Settings.TileSize * 2, 0), 3);

    // Stage
    spriteBatch.Draw(_hudSprites, new Vector2(Settings.TileSize * 6, 0), _worldTile, Color.White);
    DrawNumber(spriteBatch, GameData.World, new Vector2(Settings.TileSize * 7, 0));
    spriteBatch.Draw(_hudSprites, new Vector2(Settings.TileSize * 8, 0), _hyphenTile, Color.White);
    DrawNumber(spriteBatch, GameData.Level, new Vector2(Settings.TileSize * 9, 0), 0, false);

    // Lifes
    spriteBatch.Draw(_hudSprites, new Vector2(Settings.TileSize * 11, 0), _heartTile, Color.White);
    DrawNumber(spriteBatch, GameData.Lifes, new Vector2(Settings.TileSize * 12, 0));

    // Points
    spriteBatch.Draw(_hudSprites, new Vector2(27 * Settings.TileSize, 0), _pointsTile, Color.White);
    DrawNumber(spriteBatch, GameData.Points, new Vector2(28 * Settings.TileSize, 0), 6);

    // Time
    spriteBatch.Draw(_hudSprites, new Vector2(35 * Settings.TileSize, 0), _timeTile, Color.White);
    DrawNumber(spriteBatch, _time, new Vector2(36 * Settings.TileSize, 0), 3);

    spriteBatch.End();
  }

  private void DrawNumber(SpriteBatch spriteBatch, int number, Vector2 position, int padLength = 2,
    bool padWithZeros = true) {
    string numString = padWithZeros ? number.ToString().PadLeft(padLength, '0') : number.ToString();

    for (var i = 0; i < numString.Length; i++) {
      int digit = numString[i] - '0';
      spriteBatch.Draw(_hudSprites, position + new Vector2(i * Settings.TileSize, 0), _digitTiles[digit], Color.White);
    }
  }
}