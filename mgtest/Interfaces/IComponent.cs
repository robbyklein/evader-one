using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Interfaces;

public interface IComponent {
  void Update(GameTime gameTime);
  void Draw(SpriteBatch spriteBatch);
}