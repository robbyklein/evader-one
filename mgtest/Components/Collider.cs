using mgtest.Entities;
using mgtest.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace mgtest.Components;

public class Collider(
  Entity owner,
  float width = 8f,
  float height = 8f,
  float offsetX = 0f,
  float offsetY = 0f
) : IComponent {
  public RectangleF Bounds { get; private set; }

  public void Update(GameTime gameTime) {
    Bounds = new RectangleF(
      owner.Position.X + offsetX,
      owner.Position.Y + offsetY,
      width,
      height
    );
  }

  public void Draw(SpriteBatch spriteBatch) {
  }

  public RectangleF GetProjectedBounds(Vector2 position) {
    return new RectangleF(
      position.X + offsetX,
      position.Y + offsetY,
      width,
      height
    );
  }
}