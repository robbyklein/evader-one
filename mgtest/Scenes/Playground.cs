using mgtest.Abstracts;
using mgtest.Components;
using mgtest.Entities;
using mgtest.Managers;
using mgtest.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace mgtest.Scenes;

public class Playground : Scene {
  private Song _musicPlayer;
  private Entity _playerEntity;
  private Entity _coinEntity;
  private Camera _camera;
  private TilemapManager _tilemapManager;
  private readonly SfxManager _sfxManager;

  public Playground(Game1 game) : base(game) {
    _tilemapManager = new TilemapManager(Game);
    _sfxManager = new SfxManager();

    MediaPlayer.IsRepeating = true;
  }

  public override void LoadContent() {
    _sfxManager.LoadSounds(Game);
    _tilemapManager.LoadMap(Map.Playground);
    _camera = new Camera(Game.GraphicsDevice);
    _musicPlayer = Game.Content.Load<Song>("music/one");
    _playerEntity = new PlayerEntity(Game.Content, _tilemapManager, _sfxManager);
    _coinEntity = new CoinEntity(Game.Content);

    MediaPlayer.Play(_musicPlayer);
  }

  public override void UnloadContent() {
    _playerEntity = null;
    _camera = null;
    _tilemapManager = null;
    _musicPlayer = null;
  }

  public override void Update(GameTime gameTime) {
    _tilemapManager.Update(gameTime);
    _playerEntity.Update(gameTime);
    _coinEntity.Update(gameTime);
    _camera.Update(_playerEntity.Position, _tilemapManager.TiledMap);
  }

  public override void Draw(SpriteBatch spriteBatch) {
    _tilemapManager.TiledMapRenderer.Draw(_camera.Transform);

    spriteBatch.Begin(transformMatrix: _camera.Transform);
    _playerEntity.Draw(spriteBatch);
    _coinEntity.Draw(spriteBatch);
    spriteBatch.End();
  }
}