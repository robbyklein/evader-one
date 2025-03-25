using mgtest.Abstracts;
using mgtest.Types;
using mgtest.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace mgtest.Scenes;

public class Title : GameScene {
  // Blink fields for "PRESS START!" text.
  private float _blinkTimer;
  private bool _showPressStart = true;
  private const float BlinkInterval = 1.0f; // 1 second

  // Fade settings for music.
  private const float FadeDuration = 5.0f;
  private const float TargetVolume = 1.0f;

  public Title(Game1 game) : base(game) {
    MediaPlayer.IsRepeating = true;
  }

  public override void LoadContent() {
    base.LoadContent("levels/title", "music/title");
    MediaPlayer.Volume = 0.0f;
  }

  public override void Update(GameTime gameTime) {
    var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

    // Fade in music.
    MediaPlayer.Volume = AudioUtilities.UpdateVolume(MediaPlayer.Volume, TargetVolume, FadeDuration, dt);

    // Check for start press
    InputManager.Update(gameTime);


    if (InputManager.IsActionPressed(InputAction.Start)) {
      SfxManager.PlaySound(Sfx.MenuSelect);
      Game.SceneManager.ChangeScene(new Difficulty(Game));
    }


    // Update blink timer for "PRESS START!" text.
    _blinkTimer += dt;
    if (_blinkTimer >= BlinkInterval) {
      _blinkTimer -= BlinkInterval;
      _showPressStart = !_showPressStart;
    }

    // Update common game logic (tilemap, entities, camera, collision checks).
    base.Update(gameTime);
  }

  public override void Draw(SpriteBatch spriteBatch) {
    // Draw common content: tilemap and entities.
    base.Draw(spriteBatch);

    // Draw title-specific UI on top (blinking "PRESS START!").
    spriteBatch.Begin(transformMatrix: Camera.Transform);
    if (_showPressStart) {
      BitmapFont.DrawString(spriteBatch, "press start!", new Vector2(114, 76), Color.White);
    }

    spriteBatch.End();
  }
}