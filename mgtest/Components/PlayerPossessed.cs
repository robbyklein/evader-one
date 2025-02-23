using mgtest.Entities;
using mgtest.Interfaces;
using mgtest.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace mgtest.Components;

public class PlayerPossessed(Entity owner, SfxManager sfxManager, Physics physics) : IComponent {
  // Dependencies
  private readonly Entity _owner = owner;
  private readonly SfxManager _sfxManager = sfxManager;

  // State
  private KeyboardState _previousKeyboardState = Keyboard.GetState();

  // Lifecycle
  public void Update(GameTime gameTime) {
    if (physics == null) {
      return;
    }

    // Gather current keyboard input
    KeyboardState currentKeyboardState = Keyboard.GetState();

    // Handle left/right movement
    physics.MoveInputX = GetMoveX(currentKeyboardState);

    // Check for jump press
    if (IsJumpJustPressed(currentKeyboardState, _previousKeyboardState)) {
      physics.RequestJump();
    }

    // Update previous keyboard state
    _previousKeyboardState = currentKeyboardState;
  }

  public void Draw(SpriteBatch spriteBatch) {
  }

  // Helpers
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

  private bool IsJumpJustPressed(KeyboardState current, KeyboardState previous) {
    return current.IsKeyDown(Keys.Space) && previous.IsKeyUp(Keys.Space);
  }
}