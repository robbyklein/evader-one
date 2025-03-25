using mgtest.Data;
using mgtest.Entities;
using mgtest.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Abstracts;

public class LevelScene(Game1 game, string levelString) : GameScene(game) {
  // Dependencies
  private LevelHud _hud;
  private LevelNotice _levelNotice;

  public LevelInfo LevelInfo = LevelData.GetLevelInfo(levelString, GameData.Difficulty.ToString());
  private float _timeAccumulator;
  private int _startTime;
  private bool _started;
  private int _finishTime;


  protected override void LoadContent(string mapAsset, string songAsset) {
    base.LoadContent(mapAsset, songAsset);
    _hud = new LevelHud(Game, this);
    _levelNotice = new LevelNotice(Game, this);
  }

  private void StartRoutine(GameTime gameTime) {
    if (_started) {
      return;
    }

    if (_startTime < 1500) {
      _levelNotice.SetText("ready");
    }
    else if (_startTime < 2500) {
      _levelNotice.SetText("set");
    }
    else if (_startTime < 2700) {
      _levelNotice.SetText("go!");
      GameData.GameStarted = true;
    }
    else {
      _levelNotice.SetText("");
      _started = true;
    }

    _startTime += gameTime.ElapsedGameTime.Milliseconds;
  }

  public void EndRoutine() {
    GameData.GameFinished = true;

    bool coinPass = GameData.Coins >= LevelInfo.Coins;
    bool pointPass = GameData.Points >= LevelInfo.ScoreThreshold;
    bool timePass = LevelInfo.Time > 0;

    if (coinPass && pointPass && timePass) {
      _levelNotice.SetText("pass");

      if (_finishTime > 1000) {
        GameData.NextLevel();
        Game.SceneManager.ChangeScene(new LevelDisplay(Game, GameData.CurrentLevel));
      }
    }
    else {
      _levelNotice.SetText("fail");

      if (_finishTime > 1000) {
        GameData.Fail();

        if (GameData.Lifes == 0) {
          Game.SceneManager.ChangeScene(new GameOver(Game));
        }
        else {
          Game.SceneManager.ChangeScene(new Playground(Game, GameData.CurrentLevel));
        }
      }
    }
  }

  public override void Update(GameTime gameTime) {
    base.Update(gameTime);

    StartRoutine(gameTime);

    if (GameData.GameFinished) {
      _finishTime += gameTime.ElapsedGameTime.Milliseconds;
      EndRoutine();
    }

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
        GameData.GameFinished = true;
        EndRoutine();
      }
    }
  }

  public override void Draw(SpriteBatch spriteBatch) {
    base.Draw(spriteBatch);
    _hud.Draw(spriteBatch);
    _levelNotice.Draw(spriteBatch);
  }
}