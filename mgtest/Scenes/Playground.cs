using System.Collections.Generic;
using mgtest.Abstracts;
using mgtest.Components;
using mgtest.Entities;
using mgtest.Managers;
using mgtest.Types;
using mgtest.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace mgtest.Scenes;

public class Playground : GameScene {
  // Fade settings for music.
  private const float FadeDuration = 5.0f;
  private const float TargetVolume = 1.0f;

  public Playground(Game1 game) : base(game) {
    MediaPlayer.IsRepeating = true;
  }

  public override void LoadContent() {
    LoadCommonContent();
    LoadGameContent(Map.Playground, "music/one");
  }

  public override void UnloadContent() {
    Player = null;
    Entities.Clear();
    TilemapManager = null;
  }

  public override void Update(GameTime gameTime) {
    var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
    MediaPlayer.Volume = AudioUtilities.UpdateVolume(MediaPlayer.Volume, TargetVolume, FadeDuration, dt);

    InputManager.Update(gameTime);

    // --- Coin Collision Detection ---
    var playerCollider = Player?.GetComponent<Collider>();
    if (playerCollider != null) {
      // Iterate over a copy so we can modify the Entities list.
      foreach (Entity entity in new List<Entity>(Entities)) {
        if (entity == Player) {
          continue;
        }

        var coinCollider = entity.GetComponent<Collider>();
        if (coinCollider != null && playerCollider.Bounds.Intersects(coinCollider.Bounds)) {
          if (entity is CoinEntity) {
            Entities.Remove(entity);
            SfxManager.PlaySound(Sfx.Collect);
          }
        }
      }
    }


    base.Update(gameTime);
  }

  public override void Draw(SpriteBatch spriteBatch) {
    // Call base draw to render the tilemap and entities.
    base.Draw(spriteBatch);
    // Add any additional Playground-specific drawing here.
  }
}