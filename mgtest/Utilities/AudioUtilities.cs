using System;

namespace mgtest.Utilities;

public class AudioUtilities {
  private const float Tolerance = 0.001f;

  public static float UpdateVolume(float currentVolume, float targetVolume, float fadeDuration, float elapsedSeconds) {
    if (Math.Abs(currentVolume - targetVolume) < Tolerance) {
      return currentVolume;
    }

    float deltaVolume = targetVolume - currentVolume;
    float volumeChange = deltaVolume / fadeDuration * elapsedSeconds;
    float newVolume = currentVolume + volumeChange;

    if ((deltaVolume > 0 && newVolume > targetVolume) ||
        (deltaVolume < 0 && newVolume < targetVolume)) {
      newVolume = targetVolume;
    }

    return newVolume;
  }
}