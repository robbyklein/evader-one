using mgtest.Abstracts;
using mgtest.Data;
using mgtest.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace mgtest.Scenes;

public class Difficulty(Game1 game) : Scene(game) {
  private GameDifficulty _selectedDifficulty = GameDifficulty.Normal;

  private void Next() {
    if (_selectedDifficulty == GameDifficulty.Normal) {
      _selectedDifficulty = GameDifficulty.Hard;
    }
    else if (_selectedDifficulty == GameDifficulty.Hard) {
      _selectedDifficulty = GameDifficulty.Easy;
    }
    else {
      _selectedDifficulty = GameDifficulty.Normal;
    }
  }

  private void Previous() {
    if (_selectedDifficulty == GameDifficulty.Normal) {
      _selectedDifficulty = GameDifficulty.Easy;
    }
    else if (_selectedDifficulty == GameDifficulty.Hard) {
      _selectedDifficulty = GameDifficulty.Normal;
    }
    else {
      _selectedDifficulty = GameDifficulty.Hard;
    }
  }

  private Color GetColorForDifficulty(string option) {
    if (_selectedDifficulty.ToString() == option) {
      return Color.Yellow;
    }

    return Color.White;
  }

  public override void LoadContent() {
    base.LoadContent();
    MediaPlayer.Stop();
  }

  public override void Update(GameTime gameTime) {
    // Process title-specific input.
    InputManager.Update(gameTime);

    if (InputManager.IsActionPressed(InputAction.Start)) {
      SfxManager.PlaySound(Sfx.MenuSelect);
      GameData.Reset(_selectedDifficulty);
      Game.SceneManager.ChangeScene(new LevelDisplay(Game, GameData.CurrentLevel));
    }

    if (InputManager.IsActionPressed(InputAction.MoveUp)) {
      SfxManager.PlaySound(Sfx.MenuChange);
      Previous();
    }

    if (InputManager.IsActionPressed(InputAction.MoveDown)) {
      SfxManager.PlaySound(Sfx.MenuChange);
      Next();
    }
  }


  public override void Draw(SpriteBatch spriteBatch) {
    base.Draw(spriteBatch);


    spriteBatch.Begin();
    BitmapFont.DrawString(spriteBatch, "easy", new Vector2(114, 76), GetColorForDifficulty("Easy"));
    BitmapFont.DrawString(spriteBatch, "normal", new Vector2(114, 86), GetColorForDifficulty("Normal"));
    BitmapFont.DrawString(spriteBatch, "hard", new Vector2(114, 96), GetColorForDifficulty("Hard"));
    spriteBatch.End();
  }
}