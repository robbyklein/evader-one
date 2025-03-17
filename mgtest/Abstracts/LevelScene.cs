using mgtest.Entities;
using mgtest.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Abstracts;

public class LevelScene : GameScene {
  // Dependencies
  private LevelHud _hud;

  public int Time = 300;
  private float _timeAccumulator;

  public LevelScene(Game1 game) : base(game) {
  }

  protected override void LoadContent(Map map, string songAsset) {
    base.LoadContent(map, songAsset);

    _hud = new LevelHud(Game, this);
  }

  public override void Update(GameTime gameTime) {
    base.Update(gameTime);
  }

  public override void Draw(SpriteBatch spriteBatch) {
    base.Draw(spriteBatch);
    _hud.Draw(spriteBatch);
  }
}