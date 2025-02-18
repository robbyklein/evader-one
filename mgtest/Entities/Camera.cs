using System;
using mgtest.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;

namespace mgtest.Components;

public class Camera {
  private readonly GraphicsDevice _graphicsDevice;
  private readonly float _smoothSpeed;
  private Vector2 _position;

  public Camera(GraphicsDevice graphicsDevice, float smoothSpeed = 0.1f) {
    _graphicsDevice = graphicsDevice;
    _smoothSpeed = smoothSpeed;
  }

  public Matrix Transform { get; private set; }
  public Vector2 Position => _position;

  public void Update(Vector2 targetPosition, TiledMap tilemap) {
    // Use your virtual resolution so camera logic is in "low-res" space:
    var viewportSize = new Vector2(Settings.VirtualWidth, Settings.VirtualHeight);

    // 1) Find the desired position (center on target)
    Vector2 desiredPosition = targetPosition - viewportSize / 2f;

    // 2) Smoothly lerp
    _position = Vector2.Lerp(_position, desiredPosition, _smoothSpeed);

    // 3) Clamp to map bounds
    float maxX = tilemap.WidthInPixels - viewportSize.X;
    float maxY = tilemap.HeightInPixels - viewportSize.Y;
    if (maxX < 0) {
      maxX = 0;
    }

    if (maxY < 0) {
      maxY = 0;
    }

    _position.X = MathHelper.Clamp(_position.X, 0, maxX);
    _position.Y = MathHelper.Clamp(_position.Y, 0, maxY);

    // 4) Snap to integer coordinates (pixel-perfect)
    _position.X = (float)Math.Round(_position.X);
    _position.Y = (float)Math.Round(_position.Y);

    // 5) Build the transform
    Transform = Matrix.CreateTranslation(new Vector3(-_position, 0));
  }
}