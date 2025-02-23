using mgtest.Entities;
using mgtest.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Components;

public class SpriteRenderComponent(Entity owner, Texture2D texture) : IComponent {
  public void Update(GameTime gameTime) {
  }

  public void Draw(SpriteBatch spriteBatch) {
    spriteBatch.Draw(texture, owner.Position, Color.White);
  }
}