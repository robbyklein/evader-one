using System;
using mgtest.Config;
using mgtest.Managers;
using mgtest.Scenes;
using mgtest.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace mgtest;

public class Game1 : Game {
  private readonly GraphicsDeviceManager _graphics;
  private RenderTarget2D _renderTarget;
  private ScreenScaler _screenScaler;
  private SpriteBatch _spriteBatch;
  private WindowManager _windowManager;
  public SceneManager SceneManager;

  public Game1() {
    // Settings & Configuration
    Content.RootDirectory = "Content";
    IsMouseVisible = true;
    IsFixedTimeStep = true;
    TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);

    // Setup graphics manager
    _graphics = new GraphicsDeviceManager(this);
    _graphics.PreferredBackBufferWidth = Settings.VirtualWidth * Settings.WindowScale;
    _graphics.PreferredBackBufferHeight = Settings.VirtualHeight * Settings.WindowScale;
    _graphics.SynchronizeWithVerticalRetrace = true;
    _graphics.ApplyChanges();
  }

  protected override void Initialize() {
    _screenScaler = new ScreenScaler(_graphics, Settings.VirtualWidth, Settings.VirtualHeight);
    _windowManager = new WindowManager(Window, _graphics);
    SceneManager = new SceneManager(new Splash(this), GraphicsDevice);
    _renderTarget = new RenderTarget2D(GraphicsDevice, Settings.VirtualWidth, Settings.VirtualHeight);

    base.Initialize();
  }


  protected override void LoadContent() {
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    SceneManager.LoadContent();
  }

  protected override void Update(GameTime gameTime) {
    if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
      Exit();
    }

    SceneManager.Update(gameTime);
    base.Update(gameTime);
  }


  protected override void Draw(GameTime gameTime) {
    // Draw everything to the render target (low resolution)
    GraphicsDevice.SetRenderTarget(_renderTarget);
    GraphicsDevice.Clear(Color.Black);

    SceneManager.Draw(_spriteBatch);

    // Switch back to the screen
    GraphicsDevice.SetRenderTarget(null);
    GraphicsDevice.Clear(Color.Black);

    // Draw the render target to the screen WITHOUT rounding pixels
    WindowScaleInfo scale = _screenScaler.CalculateScale();
    var destinationRect = new Rectangle(scale.OffsetX, scale.OffsetY, scale.ScaledWidth, scale.ScaledHeight);

    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
    _spriteBatch.Draw(_renderTarget, destinationRect, Color.White);
    _spriteBatch.End();

    base.Draw(gameTime);
  }
}