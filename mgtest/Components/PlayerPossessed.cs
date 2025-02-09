using mgtest.Entities;
using mgtest.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace mgtest.Components;

public class PlayerPossessed : IComponent {
  // Parent
  private readonly Entity _owner;

  // Dependencies
  private readonly Physics _physics;

  // Lifecycle
  public PlayerPossessed(Entity owner) {
    _owner = owner;
    _physics = _owner.GetComponent<Physics>();
  }

  public void Update(GameTime gameTime) {
    // Make sure we have a physics component
    if (_physics == null) {
      return;
    }

    // Get the keyboard state
    KeyboardState ks = Keyboard.GetState();

    // Set X movement on physics
    _physics.MoveInputX = GetMoveX(ks);

    // Check jump
    if (IsJumpPressed(ks)) {
      _physics.RequestJump();
    }
  }

  public void Draw(SpriteBatch spriteBatch) {
  }

  // Methods
  private bool IsJumpPressed(KeyboardState ks) {
    return ks.IsKeyDown(Keys.Space);
  }

  private float GetMoveX(KeyboardState ks) {
    var moveX = 0f;

    if (ks.IsKeyDown(Keys.Left)) {
      moveX = -1f;
    }
    else if (ks.IsKeyDown(Keys.Right)) {
      moveX = 1f;
    }

    return moveX;
  }
}