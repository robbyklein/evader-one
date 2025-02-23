using mgtest.Abstracts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mgtest.Managers;

public class SceneManager(Scene initialScene) {
  private Scene _currentScene = initialScene;

  public void LoadContent() {
    _currentScene.LoadContent();
  }

  public void ChangeScene(Scene newScene) {
    _currentScene.UnloadContent();
    _currentScene = newScene;
    _currentScene.LoadContent();
  }

  public void Update(GameTime gameTime) {
    _currentScene.Update(gameTime);
  }

  public void Draw(SpriteBatch spriteBatch) {
    _currentScene.Draw(spriteBatch);
  }
}