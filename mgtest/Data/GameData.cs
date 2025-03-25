using System.Collections.Generic;
using mgtest.Types;

namespace mgtest.Data;

public static class GameData {
  // Global Data
  public static List<string> Levels = ["1-1", "1-2"];
  private static int _currentLevel;

  // Global state
  public static bool GameStarted = true;
  public static bool GameFinished;
  public static int Points;
  public static int Lifes = 3;
  public static int Coins;
  public static int TotalCoins;
  public static int TotalPoints;
  public static GameDifficulty Difficulty = GameDifficulty.Normal;

  public static string CurrentLevel => Levels[_currentLevel];

  public static void NextLevel() {
    TotalCoins += Coins;
    TotalPoints += Points;
    Coins = 0;
    Points = 0;

    GameStarted = false;
    GameFinished = false;
    _currentLevel++;
  }

  public static void Fail() {
    Coins = 0;
    Points = 0;
    Lifes--;
    GameStarted = false;
    GameFinished = false;
  }

  public static void Reset(GameDifficulty difficulty) {
    Difficulty = difficulty;
    Points = 0;
    Lifes = 3;
    Coins = 0;
    GameStarted = false;
    GameFinished = false;
    _currentLevel = 0;
  }
}