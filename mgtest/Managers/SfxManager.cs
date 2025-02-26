using System.Collections.Generic;
using mgtest.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace mgtest.Utilities;

public class SfxManager {
  private readonly Dictionary<Sfx, SoundEffect> _sounds = [];

  public void LoadSounds(Game game) {
    _sounds[Sfx.Jump] = game.Content.Load<SoundEffect>("sfx/jump");
    _sounds[Sfx.Collect] = game.Content.Load<SoundEffect>("sfx/collect");
    _sounds[Sfx.Collect2] = game.Content.Load<SoundEffect>("sfx/collect2");
  }

  public void PlaySound(Sfx sfx) {
    if (_sounds.TryGetValue(sfx, out SoundEffect sound)) {
      sound.Play();
    }
  }
}