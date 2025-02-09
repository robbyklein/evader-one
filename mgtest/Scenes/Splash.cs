using mgtest.Abstracts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace mgtest.Scenes;

public class Splash : Scene {
  private int _displayedTime;
  private TiledMap _tiledMap;
  private TiledMapRenderer _tiledMapRenderer;

  public Splash(Game1 game) : base(game) {
  }

  public override void LoadContent() {
    _tiledMap = Game.Content.Load<TiledMap>("levels/splash");
    _tiledMapRenderer = new TiledMapRenderer(Game.GraphicsDevice, _tiledMap);
  }

  public override void UnloadContent() {
    _tiledMapRenderer = null;
    _tiledMap = null;
  }

  public override void Update(GameTime gameTime) {
    _tiledMapRenderer.Update(gameTime);

    if (_displayedTime > 2000) {
      Game.SceneManager.ChangeScene(new Playground(Game));
    }

    _displayedTime += gameTime.ElapsedGameTime.Milliseconds;
  }

  public override void Draw(SpriteBatch spriteBatch) {
    Game.GraphicsDevice.Clear(Color.White);
    _tiledMapRenderer.Draw();
  }
}