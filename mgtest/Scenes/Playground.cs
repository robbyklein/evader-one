using mgtest.Abstracts;
using mgtest.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace mgtest.Scenes;

public class Playground : LevelScene {
  // Fade settings for music.
  private const float FadeDuration = 5.0f;
  private const float TargetVolume = 1.0f;

  public Playground(Game1 game, string levelString) : base(game, levelString) {
    MediaPlayer.IsRepeating = true;
  }

  public override void LoadContent() {
    base.LoadContent(LevelInfo.Tilemap, LevelInfo.SongAsset);
  }

  public override void UnloadContent() {
    Player = null;
    Entities.Clear();
    TilemapManager = null;
  }

  public override void Update(GameTime gameTime) {
    var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
    MediaPlayer.Volume = AudioUtilities.UpdateVolume(MediaPlayer.Volume, TargetVolume, FadeDuration, dt);

    InputManager.Update(gameTime);

    base.Update(gameTime);
  }

  public override void Draw(SpriteBatch spriteBatch) {
    base.Draw(spriteBatch);
  }
}