using System.Collections.Generic;
using mgtest.Types;

namespace mgtest.Data;

public static class GameData {
  // Global Data
  public static List<string> Levels = ["1-1", "1-2"];
  private static int _currentLevel;

  // Global state
  public static bool GameStarted = true;
  public static bool GameFinished = false;
  public static int Points;
  public static int Lifes = 3;
  public static int Coins;
  public static GameDifficulty Difficulty = GameDifficulty.Normal;

  public static string CurrentLevel => Levels[_currentLevel];

  public static void NextLevel() {
    GameStarted = false;
    _currentLevel++;
  }

  public static void Reset(GameDifficulty difficulty) {
    Difficulty = difficulty;
    Points = 0;
    Lifes = 3;
    Coins = 0;
    _currentLevel = 0;
  }
}