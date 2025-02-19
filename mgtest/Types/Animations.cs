using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Types;

public class AnimationDefinition {
  public int Row { get; set; }
  public int StartFrame { get; set; }
  public int EndFrame { get; set; }
  public float FrameTime { get; set; }
}

public enum CharacterAnimationType {
  Idle,
  Walk,
  Jump,
  WallSlide
}

public class SpriteSheet {
  public Texture2D Texture { get; set; }
  public int Rows { get; set; }
  public int Columns { get; set; }

  public int FrameWidth { get; set; } = 8;

  public int FrameHeight { get; set; } = 8;
}