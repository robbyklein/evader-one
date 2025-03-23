using mgtest.Abstracts;
using mgtest.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace mgtest.Scenes;

public class Splash : Scene {
  private int _displayedTime;
  private TiledMap _tiledMap;
  private TiledMapRenderer _tiledMapRenderer;
  private Song _splashSong;
  private bool _playing;

  public Splash(Game1 game) : base(game) {
  }

  public override void LoadContent() {
    _splashSong = Game.Content.Load<Song>("music/splash");
    _tiledMap = Game.Content.Load<TiledMap>("levels/splash");
    _tiledMapRenderer = new TiledMapRenderer(Game.GraphicsDevice, _tiledMap);
  }

  public override void UnloadContent() {
    _tiledMapRenderer = null;
    _tiledMap = null;
  }

  public override void Update(GameTime gameTime) {
    _tiledMapRenderer.Update(gameTime);

    // Play the splash song once right after fade-in is complete.
    if (!_playing && !Game.SceneManager.IsTransitionActive) {
      MediaPlayer.Play(_splashSong);
      _playing = true;
    }

    // Transition to the Title scene after a set display time.
    if (_displayedTime > Settings.DisplaySecs) {
      Game.SceneManager.ChangeScene(new Title(Game));
    }

    _displayedTime += gameTime.ElapsedGameTime.Milliseconds;
  }

  public override void Draw(SpriteBatch spriteBatch) {
    Game.GraphicsDevice.Clear(Color.Black);
    _tiledMapRenderer.Draw();
  }
}