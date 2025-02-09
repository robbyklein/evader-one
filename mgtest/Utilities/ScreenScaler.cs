using System;
using Microsoft.Xna.Framework;

namespace mgtest.Utilities;

public struct WindowScaleInfo {
  public int ScaledWidth;
  public int ScaledHeight;
  public int OffsetX;
  public int OffsetY;

  public WindowScaleInfo(int scaledWidth, int scaledHeight, int offsetX, int offsetY) {
    ScaledWidth = scaledWidth;
    ScaledHeight = scaledHeight;
    OffsetX = offsetX;
    OffsetY = offsetY;
  }
}

public class ScreenScaler {
  private readonly GraphicsDeviceManager _graphics;
  private readonly int _virtualHeight;
  private readonly int _virtualWidth;

  public ScreenScaler(GraphicsDeviceManager graphics, int virtualWidth, int virtualHeight) {
    _graphics = graphics;
    _virtualWidth = virtualWidth;
    _virtualHeight = virtualHeight;
  }

  public WindowScaleInfo CalculateScale() {
    // Get the current window size
    var screenWidth = _graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
    var screenHeight = _graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;

    // Compute scaling factors to maintain aspect ratio
    var scaleX = (float)screenWidth / _virtualWidth;
    var scaleY = (float)screenHeight / _virtualHeight;
    var scale = Math.Min(scaleX, scaleY); // Uniform scaling

    // Compute letterboxing (black bars)
    var scaledWidth = (int)(_virtualWidth * scale);
    var scaledHeight = (int)(_virtualHeight * scale);
    var offsetX = (screenWidth - scaledWidth) / 2;
    var offsetY = (screenHeight - scaledHeight) / 2;

    return new WindowScaleInfo(scaledWidth, scaledHeight, offsetX, offsetY);
  }
}