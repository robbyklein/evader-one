using mgtest.Entities;
using mgtest.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace mgtest.Components;

public class Collider : IComponent {
  private readonly Entity _owner;
  private readonly float _width;
  private readonly float _height;
  private readonly float _offsetX;
  private readonly float _offsetY;
  public RectangleF Bounds { get; private set; }

  public Collider(Entity owner, float width = 8f, float height = 8f,
    float offsetX = 0f, float offsetY = 0f) {
    _owner = owner;
    _width = width;
    _height = height;
    _offsetX = offsetX;
    _offsetY = offsetY;
  }

  public void Update(GameTime gameTime) {
    Bounds = new RectangleF(_owner.Position.X + _offsetX,
      _owner.Position.Y + _offsetY,
      _width, _height);
  }

  public void Draw(SpriteBatch spriteBatch) {
  }

  public RectangleF GetProjectedBounds(Vector2 testPosition) {
    return new RectangleF(testPosition.X + _offsetX,
      testPosition.Y + _offsetY,
      _width, _height);
  }
}