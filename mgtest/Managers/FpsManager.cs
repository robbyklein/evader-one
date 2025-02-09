using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace mgtest.Managers;

public class FpsManager {
  public static float Fps;
  private readonly Queue<float> _frameTimes;
  private readonly int _maxSamples = 60;
  private double _elapsedTime;
  private int _frameCount;


  public FpsManager() {
    _frameTimes = new Queue<float>(_maxSamples);
  }

  public void Update(GameTime gameTime) {
    var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
    _elapsedTime += deltaTime;
    _frameCount++;

    _frameTimes.Enqueue(deltaTime);
    if (_frameTimes.Count > _maxSamples) {
      _frameTimes.Dequeue();
    }

    if (_elapsedTime >= 1.0) {
      Fps = _frameCount / (float)_elapsedTime;
      _elapsedTime = 0;
      _frameCount = 0;
    }
  }
}