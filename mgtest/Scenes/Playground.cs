using mgtest.Abstracts;
using mgtest.Components;
using mgtest.Entities;
using mgtest.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace mgtest.Scenes;

public class Playground : Scene {
  private Song _musicPlayer;
  private Entity _playerEntity;
  private Camera _camera;

  public Playground(Game1 game) : base(game) {
  }

  public override void LoadContent() {
    // Load the map
    Game.TilemapManager.LoadMap(Map.Playground);

    // Create your player
    _playerEntity = new PlayerEntity(Game.Content, Game.TilemapManager);

    // Create the camera
    _camera = new Camera(Game.GraphicsDevice);

    // Music
    _musicPlayer = Game.Content.Load<Song>("music/one");
    MediaPlayer.IsRepeating = true;
    MediaPlayer.Play(_musicPlayer);
  }

  public override void UnloadContent() {
    _playerEntity = null;
    _camera = null;
  }

  public override void Update(GameTime gameTime) {
    Game.TilemapManager.Update(gameTime);
    _playerEntity.Update(gameTime);

    // Follow the player
    _camera.Update(_playerEntity.Position, Game.TilemapManager.TiledMap);
  }

  public override void Draw(SpriteBatch spriteBatch) {
    // 1) Draw the TiledMap with camera transform
    //    The TiledMapRenderer has an overload that takes the matrix
    Game.TilemapManager.TiledMapRenderer.Draw(_camera.Transform);

    // 2) Draw your entities with the same transform
    spriteBatch.Begin(transformMatrix: _camera.Transform);
    _playerEntity.Draw(spriteBatch);
    spriteBatch.End();
  }
}