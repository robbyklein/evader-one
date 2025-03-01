using mgtest.Abstracts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace mgtest.Managers;

public class SceneManager(Scene initialScene, GraphicsDevice graphicsDevice) {
  // Dependencies
  private Texture2D _blackTexture;

  // State
  private Scene _currentScene = initialScene;
  private Scene _nextScene;

  private enum TransitionState {
    None,
    FadingOut,
    FadingIn
  }

  private TransitionState _transitionState = TransitionState.None;
  private float _transitionTimer;
  private readonly float _fadeDuration = 0.5f;
  private float _transitionAlpha;
  private float _initialVolume = 1.0f;
  private const float GlobalTargetVolume = 1.0f;

  public bool IsTransitionActive => _transitionState != TransitionState.None;

  // Lifecycle 
  public void LoadContent() {
    _blackTexture = new Texture2D(graphicsDevice, 1, 1);
    _blackTexture.SetData([Color.Black]);

    _currentScene.LoadContent();

    _transitionState = TransitionState.FadingIn;
    _transitionTimer = 0f;
    _transitionAlpha = 1f;
    _initialVolume = 0f;
    MediaPlayer.Volume = 0f;
  }

  public void Update(GameTime gameTime) {
    var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

    if (_transitionState == TransitionState.FadingOut) {
      _transitionTimer += dt;
      float progress = _transitionTimer / _fadeDuration;
      _transitionAlpha = progress;
      MediaPlayer.Volume = MathHelper.Lerp(_initialVolume, 0f, progress);

      if (progress >= 1f) {
        _currentScene.UnloadContent();
        _currentScene = _nextScene;
        _nextScene = null;
        _currentScene.LoadContent();

        _transitionState = TransitionState.FadingIn;
        _transitionTimer = 0f;
        _initialVolume = MediaPlayer.Volume;
      }
    }
    else if (_transitionState == TransitionState.FadingIn) {
      _currentScene.Update(gameTime);

      _transitionTimer += dt;
      float progress = _transitionTimer / _fadeDuration;
      _transitionAlpha = 1f - progress;
      MediaPlayer.Volume = MathHelper.Lerp(0f, GlobalTargetVolume, progress);

      if (progress >= 1f) {
        _transitionState = TransitionState.None;
        _transitionAlpha = 0f;
      }
    }
    else {
      _currentScene.Update(gameTime);
    }
  }

  public void Draw(SpriteBatch spriteBatch) {
    _currentScene.Draw(spriteBatch);

    if (_transitionAlpha > 0f) {
      spriteBatch.Begin();
      spriteBatch.Draw(_blackTexture,
        new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height),
        Color.Black * _transitionAlpha);
      spriteBatch.End();
    }
  }

  // Methods
  public void ChangeScene(Scene newScene) {
    if (_transitionState == TransitionState.None) {
      _nextScene = newScene;
      _transitionState = TransitionState.FadingOut;
      _transitionTimer = 0f;
      _initialVolume = MediaPlayer.Volume;
    }
  }
}