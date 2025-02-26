using mgtest.Components;
using mgtest.Managers;
using mgtest.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace mgtest.Abstracts;

public abstract class BaseScene : Scene {
  protected readonly SfxManager SfxManager;
  protected readonly InputManager InputManager;
  protected Camera Camera;
  protected Song BackgroundSong;
  protected BitmapFont BitmapFont;
  protected TilemapManager TilemapManager;

  protected BaseScene(Game1 game) : base(game) {
    SfxManager = new SfxManager();
    InputManager = new InputManager();
    Camera = new Camera(Game.GraphicsDevice);
    TilemapManager = new TilemapManager(Game);
    MediaPlayer.IsRepeating = true;
  }

  public override void LoadContent() {
    LoadCommonContent();
    LoadSceneContent();
  }

  protected virtual void LoadCommonContent() {
    SfxManager.LoadSounds(Game);

    var fontTexture = Game.Content.Load<Texture2D>("sprites/characters");
    BitmapFont = new BitmapFont(fontTexture);
  }

  protected virtual void LoadSceneContent() {
  }
}