using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Interfaces;

public interface IComponent {
  // Called every frame
  void Update(GameTime gameTime);

  // Optional: if your component draws something
  void Draw(SpriteBatch spriteBatch);
}