using mgtest.Entities;
using mgtest.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace mgtest.Components;

public class PlayerPossessed : IComponent {
  // Parent Entity
  private readonly Entity _owner;

  // Dependencies
  private readonly Physics _physics;

  // Track keyboard states
  private KeyboardState _previousKeyboardState;

  public PlayerPossessed(Entity owner) {
    _owner = owner;

    // Grab the Physics component from this entity (e.g. PlayerEntity)
    _physics = _owner.GetComponent<Physics>();

    // Initialize the "previous" keyboard state
    _previousKeyboardState = Keyboard.GetState();
  }

  public void Update(GameTime gameTime) {
    // If we don't have a Physics component, do nothing
    if (_physics == null) {
      return;
    }

    // Get the current keyboard state
    KeyboardState currentKeyboardState = Keyboard.GetState();

    // Handle movement input
    _physics.MoveInputX = GetMoveX(currentKeyboardState);

    // Detect jump press (space key)
    if (IsJumpJustPressed(currentKeyboardState, _previousKeyboardState)) {
      _physics.RequestJump();
    }

    // Save keyboard state for next frame
    _previousKeyboardState = currentKeyboardState;
  }

  public void Draw(SpriteBatch spriteBatch) {
    // No drawing logic needed here
  }

  // ---------------------------------------------------------
  // Private Helpers
  // ---------------------------------------------------------
  private bool IsJumpJustPressed(KeyboardState current, KeyboardState previous) {
    // "Just pressed" = currently down, previously up
    return current.IsKeyDown(Keys.Space) && previous.IsKeyUp(Keys.Space);
  }

  private float GetMoveX(KeyboardState ks) {
    // Simple left/right movement
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