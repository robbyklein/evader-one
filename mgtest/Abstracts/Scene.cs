using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Abstracts;

public class Scene {
  protected Game1 Game;

  public Scene(Game1 game) {
    Game = game;
  }

  public virtual void LoadContent() {
  }

  public virtual void UnloadContent() {
  }

  public virtual void Update(GameTime gameTime) {
  }

  public virtual void Draw(SpriteBatch spriteBatch) {
  }
}