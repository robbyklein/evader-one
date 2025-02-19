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

  // Track keyboard states
  private KeyboardState _previousKeyboardState;

  // Lifecycle
  public PlayerPossessed(Entity owner) {
    _owner = owner;
    _physics = _owner.GetComponent<Physics>();

    // Initialize previous keyboard state so we can detect changes on the first update
    _previousKeyboardState = Keyboard.GetState();
  }

  public void Update(GameTime gameTime) {
    // Make sure we have a physics component
    if (_physics == null) {
      return;
    }

    // Get the current keyboard state
    KeyboardState currentKeyboardState = Keyboard.GetState();

    // Set X movement on physics
    _physics.MoveInputX = GetMoveX(currentKeyboardState);

    // Check jump: we only jump if SPACE is down now and was up previously
    if (IsJumpJustPressed(currentKeyboardState, _previousKeyboardState)) {
      _physics.RequestJump();
    }

    // Store current state for next frame’s comparison
    _previousKeyboardState = currentKeyboardState;
  }

  public void Draw(SpriteBatch spriteBatch) {
    // No drawing needed here
  }

  // ---------------------------------------------------------
  // Private Helpers
  // ---------------------------------------------------------

  private bool IsJumpJustPressed(KeyboardState current, KeyboardState previous) {
    // "Just pressed" = currently down, previously up
    return current.IsKeyDown(Keys.Space) && previous.IsKeyUp(Keys.Space);
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