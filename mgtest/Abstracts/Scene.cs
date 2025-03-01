using mgtest.Components;
using mgtest.Managers;
using mgtest.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace mgtest.Abstracts;

public abstract class Scene {
  protected Game1 Game;
  protected readonly SfxManager SfxManager;
  protected readonly InputManager InputManager;
  protected Camera Camera;
  protected Song BackgroundSong;
  protected BitmapFont BitmapFont;
  protected TilemapManager TilemapManager;

  public Scene(Game1 game) {
    Game = game;
    SfxManager = new SfxManager();
    InputManager = new InputManager();
    Camera = new Camera(Game.GraphicsDevice);
    TilemapManager = new TilemapManager(Game);
    MediaPlayer.IsRepeating = true;
  }

  public virtual void LoadContent() {
    SfxManager.LoadSounds(Game);

    var fontTexture = Game.Content.Load<Texture2D>("sprites/characters");
    BitmapFont = new BitmapFont(fontTexture);
  }

  public virtual void UnloadContent() {
  }

  public virtual void Update(GameTime gameTime) {
  }

  public virtual void Draw(SpriteBatch spriteBatch) {
  }
}