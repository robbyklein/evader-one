using mgtest.Entities;
using mgtest.Interfaces;
using mgtest.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace mgtest.Components;

public class PlayerPossessed : IComponent {
  // Parent Entity
  private readonly Entity _owner;

  // Dependencies
  private readonly Physics _physics;
  private readonly SfxManager _sfxManager;

  // Track keyboard states
  private KeyboardState _previousKeyboardState;

  public PlayerPossessed(Entity owner, SfxManager sfxManager) {
    _owner = owner;
    _sfxManager = sfxManager;

    // Grab the Physics component from this entity (the player)
    _physics = _owner.GetComponent<Physics>();

    // Initialize the "previous" keyboard state
    _previousKeyboardState = Keyboard.GetState();
  }

  public void Update(GameTime gameTime) {
    // If there's no Physics, nothing to do
    if (_physics == null) {
      return;
    }

    // Gather current keyboard input
    KeyboardState currentKeyboardState = Keyboard.GetState();

    // Handle left/right movement
    _physics.MoveInputX = GetMoveX(currentKeyboardState);

    // Check for jump press
    if (IsJumpJustPressed(currentKeyboardState, _previousKeyboardState)) {
      _physics.RequestJump();
    }

    // Update previous keyboard state
    _previousKeyboardState = currentKeyboardState;
  }

  public void Draw(SpriteBatch spriteBatch) {
    // No visuals needed for possession/controls
  }

  // ---------------------------------------------------------
  // Private Helpers
  // ---------------------------------------------------------
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

  private bool IsJumpJustPressed(KeyboardState current, KeyboardState previous) {
    // "Just pressed" = currently down, previously up
    return current.IsKeyDown(Keys.Space) && previous.IsKeyUp(Keys.Space);
  }
}