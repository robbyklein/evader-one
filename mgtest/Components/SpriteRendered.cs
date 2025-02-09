using mgtest.Entities;
using mgtest.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Components
{
  public class SpriteRenderComponent : IComponent
  {
    private readonly Entity _owner;
    private readonly Texture2D _texture;

    public SpriteRenderComponent(Entity owner, Texture2D texture) {
      _owner = owner;
      _texture = texture;
    }

    public void Update(GameTime gameTime) {
    }

    public void Draw(SpriteBatch spriteBatch) {
      spriteBatch.Draw(_texture, _owner.Position, Color.White);
    }
  }
}