using mgtest.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace mgtest.Utilities;

public static class SfxManager {
  private static SoundEffect jump;

  public static void LoadSounds(Game game) {
    jump = game.Content.Load<SoundEffect>("sfx/jump");
  }

  public static void PlaySound(Sfx sfx) {
    switch (sfx) {
      case Sfx.Jump:
        jump.Play();
        break;
    }
  }
}