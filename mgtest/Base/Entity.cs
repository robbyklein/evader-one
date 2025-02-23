using System.Collections.Generic;
using mgtest.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Entities;

public class Entity {
  public Vector2 Position;
  public float Rotation;
  public Vector2 Scale = Vector2.One;

  private readonly List<IComponent> _components = new();

  protected void AddComponent(IComponent component) {
    _components.Add(component);
  }

  public T GetComponent<T>() where T : class, IComponent {
    foreach (IComponent comp in _components) {
      if (comp is T casted) {
        return casted;
      }
    }

    return null;
  }

  public void Update(GameTime gameTime) {
    foreach (IComponent comp in _components) {
      comp.Update(gameTime);
    }
  }

  public void Draw(SpriteBatch spriteBatch) {
    foreach (IComponent comp in _components) {
      comp.Draw(spriteBatch);
    }
  }
}