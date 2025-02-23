namespace mgtest.Interfaces;

public interface IPhysics : IComponent {
  float MoveInputX { get; set; }
  bool IsTouchingWallRight { get; }
  bool IsFacingRight { get; }
  bool IsGrounded { get; }
  bool IsWallSliding { get; }
  void RequestJump();
}