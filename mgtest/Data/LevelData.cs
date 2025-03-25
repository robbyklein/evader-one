using System.Collections.Generic;

namespace mgtest.Data;

public class LevelRequirements {
  public int Coins { get; set; }
  public int Time { get; set; }
  public int ScoreThreshold { get; set; }
}

public class LevelDetails {
  public string SongAsset { get; set; }
  public string Tilemap { get; set; }
  public LevelRequirements Easy { get; set; }
  public LevelRequirements Normal { get; set; }
  public LevelRequirements Hard { get; set; }
}

public class LevelInfo {
  public string SongAsset { get; set; }
  public string Tilemap { get; set; }
  public int Coins { get; set; }
  public int Time { get; set; }
  public int ScoreThreshold { get; set; }
  public string Difficulty { get; set; }
}

public static class LevelData {
  public static readonly Dictionary<string, LevelDetails> Levels = new() {
    ["1-1"] = new LevelDetails {
      SongAsset = "music/one",
      Tilemap = "levels/playground",
      Easy = new LevelRequirements { Coins = 2, Time = 13, ScoreThreshold = 0 },
      Normal = new LevelRequirements { Coins = 2, Time = 10, ScoreThreshold = 0 },
      Hard = new LevelRequirements { Coins = 2, Time = 6, ScoreThreshold = 0 }
    },
    ["1-2"] = new LevelDetails {
      SongAsset = "music/one",
      Tilemap = "levels/playground",
      Easy = new LevelRequirements { Coins = 12, Time = 3, ScoreThreshold = 1100 },
      Normal = new LevelRequirements { Coins = 18, Time = 3, ScoreThreshold = 1600 },
      Hard = new LevelRequirements { Coins = 25, Time = 3, ScoreThreshold = 2100 }
    }
  };

  public static LevelInfo GetLevelInfo(string levelName, string difficulty) {
    if (!Levels.TryGetValue(levelName, out LevelDetails details)) {
      return null;
    }

    LevelRequirements reqs = difficulty.ToLower() switch {
      "easy" => details.Easy,
      "normal" => details.Normal,
      "hard" => details.Hard,
      _ => null
    };

    if (reqs == null) {
      return null;
    }

    return new LevelInfo {
      SongAsset = details.SongAsset,
      Tilemap = details.Tilemap,
      Coins = reqs.Coins,
      Time = reqs.Time,
      ScoreThreshold = reqs.ScoreThreshold,
      Difficulty = difficulty
    };
  }
}