using System.Collections.Generic;
using mgtest.Components;
using mgtest.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Entities;

public class CoinEntity : Entity {
  // Ensure this dictionary is the one used to create the SpriteAnimator.
  private readonly Dictionary<CharacterAnimationType, AnimationDefinition> _animations = new() {
    [CharacterAnimationType.Idle] = new AnimationDefinition {
      // For a coin, if your frames are laid out horizontally,
      // Row should remain constant and StartFrame should be set to 4.
      Row = 4,
      StartFrame = 4,
      EndFrame = 7,
      FrameTime = 0.1f,
      AnimateAcrossColumns = false
    }
  };

  public CoinEntity(ContentManager content) {
    Position = new Vector2(150f, 100f);

    var spriteSheet = new SpriteSheet {
      Rows = 8,
      Columns = 10,
      Texture = content.Load<Texture2D>("tilesets/items")
    };

    // Create the coin's animation component.
    var animationComponent = new SpriteAnimator(this, spriteSheet, _animations);
    AddComponent(animationComponent);
  }
}