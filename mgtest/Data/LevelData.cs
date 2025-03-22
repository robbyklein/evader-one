using System.Collections.Generic;

namespace mgtest.Data;

public class LevelDifficultyData {
  public int Coins { get; set; }
  public int Time { get; set; }
  public int ScoreThreshold { get; set; }
}

public class LevelDifficulty {
  public LevelDifficultyData Easy { get; set; }
  public LevelDifficultyData Normal { get; set; }
  public LevelDifficultyData Hard { get; set; }
}

public static class LevelData {
  public static readonly Dictionary<string, LevelDifficulty> Levels = new() {
    ["1-1"] = new LevelDifficulty {
      Easy = new LevelDifficultyData { Coins = 5, Time = 200, ScoreThreshold = 1000 },
      Normal = new LevelDifficultyData { Coins = 5, Time = 30, ScoreThreshold = 0 },
      Hard = new LevelDifficultyData { Coins = 20, Time = 150, ScoreThreshold = 2000 }
    },
    ["1-2"] = new LevelDifficulty {
      Easy = new LevelDifficultyData { Coins = 12, Time = 220, ScoreThreshold = 1100 },
      Normal = new LevelDifficultyData { Coins = 18, Time = 190, ScoreThreshold = 1600 },
      Hard = new LevelDifficultyData { Coins = 25, Time = 140, ScoreThreshold = 2100 }
    }
  };

  public static LevelDifficulty GetLevel(string levelName) {
    return Levels.TryGetValue(levelName, out LevelDifficulty data) ? data : null;
  }
}