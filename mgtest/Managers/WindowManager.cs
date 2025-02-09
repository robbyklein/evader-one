using System;
using Microsoft.Xna.Framework;

namespace mgtest.Managers;

public class WindowManager {
  private readonly GraphicsDeviceManager _graphics;
  private readonly GameWindow _window;

  public WindowManager(GameWindow window, GraphicsDeviceManager graphics) {
    // Save reference to dependencies
    _window = window;
    _graphics = graphics;

    // Do work
    ConfigureWindow();
    SubscribeToEvents();
  }


  private void ConfigureWindow() {
    // Windows settings
    _window.AllowUserResizing = true;
  }

  private void SubscribeToEvents() {
    // Window events
    _window.ClientSizeChanged += OnResize;
  }

  private void OnResize(object sender, EventArgs e) {
    // Force graphics update
    _graphics.ApplyChanges();
  }
}