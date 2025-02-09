using System;
using mgtest.Config;
using mgtest.Entities;
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
  public FpsDisplay FpsDisplay;
  public FpsManager FpsManager;
  public SceneManager SceneManager;
  public TilemapManager TilemapManager;

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
    // Initialize managers
    _screenScaler = new ScreenScaler(_graphics, Settings.VirtualWidth, Settings.VirtualHeight);
    _windowManager = new WindowManager(Window, _graphics);
    SceneManager = new SceneManager(new Playground(this));
    TilemapManager = new TilemapManager(this);
    FpsManager = new FpsManager();
    FpsDisplay = new FpsDisplay(this);
    _renderTarget = new RenderTarget2D(GraphicsDevice, Settings.VirtualWidth, Settings.VirtualHeight);

    base.Initialize();
  }


  protected override void LoadContent() {
    // Base
    _spriteBatch = new SpriteBatch(GraphicsDevice);

    // Global content
    FpsDisplay.Load();

    // Scene content
    SceneManager.LoadContent();
  }

  protected override void Update(GameTime gameTime) {
    // Exit game
    if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
      Exit();
    }

    // Global updates
    FpsManager.Update(gameTime);
    FpsDisplay.Update(gameTime);

    //Scene updates
    SceneManager.Update(gameTime);
    base.Update(gameTime);
  }


  protected override void Draw(GameTime gameTime) {
    // Draw everything to the render target (low resolution)
    GraphicsDevice.SetRenderTarget(_renderTarget);
    GraphicsDevice.Clear(Color.Black);

    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
    FpsDisplay.Draw(_spriteBatch);
    SceneManager.Draw(_spriteBatch);
    _spriteBatch.End();

    // Switch back to the screen
    GraphicsDevice.SetRenderTarget(null);
    GraphicsDevice.Clear(Color.Black);

    // Draw the render target to the screen WITHOUT rounding pixels
    WindowScaleInfo scale = _screenScaler.CalculateScale();
    var destinationRect = new Rectangle(scale.OffsetX, scale.OffsetY, scale.ScaledWidth, scale.ScaledHeight);

    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
    _spriteBatch.Draw(_renderTarget, destinationRect, Color.White); // No integer rounding here!
    _spriteBatch.End();

    base.Draw(gameTime);
  }
}