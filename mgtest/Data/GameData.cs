using mgtest.Types;

namespace mgtest.Data;

public static class GameData {
  public static bool GameStarted = true;
  public static bool GameFinished = false;
  public static int Points;
  public static int Lifes = 3;
  public static int Coins;
  public static int World = 1;
  public static int Level = 1;
  public static GameDifficulty Difficulty = GameDifficulty.Normal;

  public static void Reset(GameDifficulty difficulty) {
    Difficulty = difficulty;
    Points = 0;
    Lifes = 3;
    Coins = 0;
    World = 1;
    Level = 1;
  }
}