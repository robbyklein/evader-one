using mgtest.Entities;
using mgtest.Interfaces;
using mgtest.Types;
using mgtest.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Components;

public class PlayerPossessed(Entity owner, SfxManager sfxManager, Physics physics, InputManager inputManager)
  : IComponent {
  // Dependencies
  private readonly Entity _owner = owner;
  private readonly SfxManager _sfxManager = sfxManager;

  public void Update(GameTime gameTime) {
    // Assume InputManager.Update(gameTime) is called in your main game loop.

    // Handle left/right movement using our input actions (or direct key mapping)
    physics.MoveInputX = GetMoveX();

    // Check for jump press using the InputManager's action or key check
    // Here, assuming you mapped Jump to Space (or corresponding gamepad button)
    if (inputManager.IsActionPressed(InputAction.Jump)) {
      physics.RequestJump();
    }
  }

  public void Draw(SpriteBatch spriteBatch) {
    // Drawing logic for the player, if needed.
  }

  // Helper method for movement input
  private float GetMoveX() {
    var moveX = 0f;

    // Check if the MoveLeft action is active (maps to Keys.A/Left, etc.)
    if (inputManager.IsActionDown(InputAction.MoveLeft)) {
      moveX = -1f;
    }
    else if (inputManager.IsActionDown(InputAction.MoveRight)) {
      moveX = 1f;
    }

    return moveX;
  }
}