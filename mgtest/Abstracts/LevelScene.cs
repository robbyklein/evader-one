using mgtest.Data;
using mgtest.Entities;
using mgtest.Managers;
using mgtest.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Abstracts;

public class LevelScene : GameScene {
  // Dependencies
  private LevelHud _hud;

  public int Time = 300;
  private float _timeAccumulator;

  public LevelScene(Game1 game, int time) : base(game) {
    Time = time;
  }

  protected override void LoadContent(Map map, string songAsset) {
    base.LoadContent(map, songAsset);

    _hud = new LevelHud(Game, this);
  }

  public override void Update(GameTime gameTime) {
    base.Update(gameTime);

    _timeAccumulator += (float)gameTime.ElapsedGameTime.TotalSeconds;

    while (_timeAccumulator >= 1f) {
      _timeAccumulator -= 1f;
      if (Time > 1) {
        Time--;
      }
      else {
        Time--;
        GameData.Lifes--;

        if (GameData.Lifes > 0) {
          Game.SceneManager.ChangeScene(new Playground(Game));
        }
        else {
          Game.SceneManager.ChangeScene(new GameOver(Game));
        }
      }
    }
  }

  public override void Draw(SpriteBatch spriteBatch) {
    base.Draw(spriteBatch);
    _hud.Draw(spriteBatch);
  }
}