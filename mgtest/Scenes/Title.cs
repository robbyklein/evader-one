using mgtest.Abstracts;
using mgtest.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace mgtest.Scenes;

public class Title : Scene {
  private TiledMap _tiledMap;
  private TiledMapRenderer _tiledMapRenderer;
  private readonly InputManager _inputManager;
  private Song _musicPlayer;

  public Title(Game1 game) : base(game) {
    _inputManager = new InputManager();

    MediaPlayer.IsRepeating = true;
  }

  public override void LoadContent() {
    _tiledMap = Game.Content.Load<TiledMap>("levels/title");
    _tiledMapRenderer = new TiledMapRenderer(Game.GraphicsDevice, _tiledMap);
    _musicPlayer = Game.Content.Load<Song>("music/one");

    MediaPlayer.Play(_musicPlayer);
  }

  public override void UnloadContent() {
    _tiledMapRenderer = null;
    _tiledMap = null;
  }

  public override void Update(GameTime gameTime) {
    _tiledMapRenderer.Update(gameTime);
    _inputManager.Update(gameTime);

    if (
      _inputManager.IsActionPressed(InputAction.Jump) ||
      _inputManager.IsActionPressed(InputAction.Start)
    ) {
      Game.SceneManager.ChangeScene(new Playground(Game));
    }
  }

  public override void Draw(SpriteBatch spriteBatch) {
    Game.GraphicsDevice.Clear(Color.Black);
    _tiledMapRenderer.Draw();
  }
}