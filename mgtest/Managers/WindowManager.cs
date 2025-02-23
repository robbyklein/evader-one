using System;
using Microsoft.Xna.Framework;

namespace mgtest.Managers;

public class WindowManager {
  private readonly GraphicsDeviceManager _graphics;
  private readonly GameWindow _window;

  public WindowManager(GameWindow window, GraphicsDeviceManager graphics) {
    _window = window;
    _graphics = graphics;

    ConfigureWindow();
    SubscribeToEvents();
  }

  private void ConfigureWindow() {
    _window.AllowUserResizing = true;
  }

  private void SubscribeToEvents() {
    _window.ClientSizeChanged += OnResize;
  }

  private void OnResize(object sender, EventArgs e) {
    _graphics.ApplyChanges();
  }
}