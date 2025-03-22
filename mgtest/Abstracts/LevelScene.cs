using mgtest.Data;
using mgtest.Entities;
using mgtest.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Abstracts;

public class LevelScene(Game1 game, string levelString) : GameScene(game) {
  // Dependencies
  private LevelHud _hud;

  public LevelInfo LevelInfo = LevelData.GetLevelInfo(levelString, GameData.Difficulty.ToString());
  private float _timeAccumulator;
  private int _startTime;

  protected override void LoadContent(string mapAsset, string songAsset) {
    base.LoadContent(mapAsset, songAsset);
    _hud = new LevelHud(Game, this);
  }

  private void StartRoutine(GameTime gameTime) {
    if (GameData.GameStarted) {
      return;
    }

    if (_startTime > 3000) {
      GameData.GameStarted = true;
    }

    _startTime += gameTime.ElapsedGameTime.Milliseconds;
  }


  public override void Update(GameTime gameTime) {
    base.Update(gameTime);

    StartRoutine(gameTime);

    if (GameData.GameFinished || !GameData.GameStarted) {
      return;
    }

    _timeAccumulator += (float)gameTime.ElapsedGameTime.TotalSeconds;

    while (_timeAccumulator >= 1f) {
      _timeAccumulator -= 1f;
      if (LevelInfo.Time > 1) {
        LevelInfo.Time--;
      }
      else {
        LevelInfo.Time--;
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