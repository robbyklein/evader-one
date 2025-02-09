using System;
using System.Collections.Generic;
using mgtest.Config;
using mgtest.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Entities;

public class FpsDisplay {
  private readonly Game _game;
  private readonly string[] fps = new string[4] { "", "", "", "" };
  private List<Texture2D> _numbers = new();


  public FpsDisplay(Game game) {
    _game = game;
  }

  public void Load() {
    _numbers = new List<Texture2D>();
    for (var i = 0; i < 10; i++) {
      _numbers.Add(_game.Content.Load<Texture2D>("numbers/" + i));
    }
  }

  public void Unload() {
    _numbers.Clear();
  }

  public void Update(GameTime gameTime) {
    var roundedFps = (int)MathF.Round(FpsManager.Fps);
    var fpsString = roundedFps.ToString().PadLeft(4, ' ');

    // Store each character in the array
    for (var i = 0; i < 4; i++) {
      fps[i] = fpsString[i].ToString();
    }
  }

  public void Draw(SpriteBatch spriteBatch) {
    var startX = Settings.VirtualWidth - Settings.TileSize * 4 - 8;
    var startY = 8;

    for (var i = 0; i < 4; i++) {
      if (fps[i] != " ") {
        // Skip empty spaces
        var digit = int.Parse(fps[i]);
        spriteBatch.Draw(_numbers[digit], new Vector2(startX + i * Settings.TileSize, startY), Color.White);
      }
    }
  }
}