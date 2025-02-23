using mgtest.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Components;

public class NullPhysics : IPhysics {
  public float MoveInputX { get; set; } = 0f;
  public bool IsTouchingWallRight => false;
  public bool IsFacingRight => true;
  public bool IsGrounded => true;
  public bool IsWallSliding => false;

  public void Update(GameTime gameTime) {
  }

  public void Draw(SpriteBatch spriteBatch) {
  }

  public void RequestJump() {
  }
}