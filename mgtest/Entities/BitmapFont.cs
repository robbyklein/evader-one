using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class BitmapFont {
  private const int TileWidth = 8;
  private const int TileHeight = 8;
  private const int Columns = 8;

  // Update this if your new font has different supported characters
  private static readonly string FontChars = "0123456789CWHPT _-/abcdefghijklmnopqrstuvwxyz:+!";

  private readonly Texture2D _texture;
  private readonly Dictionary<char, int> _charMap = new();

  public BitmapFont(Texture2D texture) {
    _texture = texture;

    for (var i = 0; i < FontChars.Length; i++) {
      _charMap[FontChars[i]] = i;
    }
  }

  public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color) {
    Vector2 pos = position;

    foreach (char c in text) {
      if (_charMap.TryGetValue(c, out int index)) {
        int col = index % Columns;
        int row = index / Columns;

        var sourceRect = new Rectangle(
          col * TileWidth,
          row * TileHeight,
          TileWidth,
          TileHeight
        );

        spriteBatch.Draw(_texture, pos, sourceRect, color);
      }

      pos.X += TileWidth;
    }
  }

  public void DrawPaddedNumber(SpriteBatch spriteBatch, int number, int totalDigits, Vector2 position, Color color) {
    string padded = number.ToString().PadLeft(totalDigits, '0');
    DrawString(spriteBatch, padded, position, color);
  }
}