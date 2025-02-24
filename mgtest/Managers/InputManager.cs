using System.Collections.Generic;
using mgtest.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class InputManager {
  // State
  private KeyboardState _currentKeyboardState;
  private KeyboardState _previousKeyboardState;
  private GamePadState _currentGamePadState;
  private GamePadState _previousGamePadState;

  // Action mappings
  private readonly Dictionary<InputAction, List<Keys>> _keyboardMappings = new();
  private readonly Dictionary<InputAction, List<Buttons>> _gamepadMappings = new();

  public InputManager() {
    // Setup keyboard mappings
    _keyboardMappings[InputAction.MoveLeft] = new List<Keys> { Keys.A, Keys.Left };
    _keyboardMappings[InputAction.MoveRight] = new List<Keys> { Keys.D, Keys.Right };
    _keyboardMappings[InputAction.Jump] = new List<Keys> { Keys.Space, Keys.W, Keys.Up };
    _keyboardMappings[InputAction.Start] = new List<Keys> { Keys.Enter };


    // Setup gamepad mappings
    _gamepadMappings[InputAction.MoveLeft] = new List<Buttons> { Buttons.DPadLeft, Buttons.LeftThumbstickLeft };
    _gamepadMappings[InputAction.MoveRight] = new List<Buttons> { Buttons.DPadRight, Buttons.LeftThumbstickRight };
    _gamepadMappings[InputAction.Jump] = new List<Buttons> { Buttons.A };
    _gamepadMappings[InputAction.Start] = new List<Buttons> { Buttons.Start };
  }

  // Lifecycle: Update input states each frame
  public void Update(GameTime gameTime) {
    _previousKeyboardState = _currentKeyboardState;
    _previousGamePadState = _currentGamePadState;

    _currentKeyboardState = Keyboard.GetState();
    _currentGamePadState = GamePad.GetState(PlayerIndex.One);
  }

  // Check if an action is pressed (i.e., just pressed this frame)
  public bool IsActionPressed(InputAction action) {
    bool keyboardPressed = _keyboardMappings.TryGetValue(action, out List<Keys> keys)
                           && keys.Exists(key =>
                             _currentKeyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key));

    bool gamepadPressed = _gamepadMappings.TryGetValue(action, out List<Buttons> buttons)
                          && buttons.Exists(button =>
                            _currentGamePadState.IsButtonDown(button) && !_previousGamePadState.IsButtonDown(button));

    return keyboardPressed || gamepadPressed;
  }

  // Check if an action is held down
  public bool IsActionDown(InputAction action) {
    bool keyboardDown = _keyboardMappings.TryGetValue(action, out List<Keys> keys)
                        && keys.Exists(key => _currentKeyboardState.IsKeyDown(key));

    bool gamepadDown = _gamepadMappings.TryGetValue(action, out List<Buttons> buttons)
                       && buttons.Exists(button => _currentGamePadState.IsButtonDown(button));

    return keyboardDown || gamepadDown;
  }
}