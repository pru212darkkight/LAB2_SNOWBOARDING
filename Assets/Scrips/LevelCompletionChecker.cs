using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LevelCompletionChecker
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "highscores.json");

    [System.Serializable]
    private class LevelScore
    {
        public string levelName;
        public float score;
        public bool isCompleted;
    }

    [System.Serializable]
    private class ScoreWrapper
    {
        public List<LevelScore> scores = new();
    }

    private static Dictionary<string, (float, float, float)> starThresholds = new()
    {
        { "Level1", (30f, 60f, 90f) },
        { "Level2", (20f, 50f, 80f) },
        { "Level3", (25f, 55f, 85f) }
    };

    public static bool IsLevelCompleted(string levelName)
    {
        var score = GetLevelScore(levelName);
        return score != null && score.isCompleted;
    }

    public static int GetStarCount(string levelName)
    {
        var levelScore = GetLevelScore(levelName);
        if (levelScore == null || !levelScore.isCompleted) return 0;

        (float oneStar, float twoStar, float threeStar) thresholds =
            starThresholds.ContainsKey(levelName) ? starThresholds[levelName] : (30f, 60f, 90f);

        if (levelScore.score >= thresholds.threeStar) return 3;
        if (levelScore.score >= thresholds.twoStar) return 2;
        if (levelScore.score >= thresholds.oneStar) return 1;
        return 0;
    }


    private static LevelScore GetLevelScore(string levelName)
    {
        if (!File.Exists(SavePath)) return null;

        try
        {
            string json = File.ReadAllText(SavePath);
            ScoreWrapper wrapper = JsonUtility.FromJson<ScoreWrapper>(json);
            return wrapper?.scores?.Find(s => s.levelName == levelName);
        }
        catch
        {
            return null;
        }
    }
}
