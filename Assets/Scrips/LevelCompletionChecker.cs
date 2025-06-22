using System;
using System.Collections.Generic;
using UnityEngine;

public static class LevelCompletionChecker
{
    private static Dictionary<string, (float, float, float)> starThresholds = new()
    {
        { "Level1", (2000f, 3000f, 3600f) },
        { "Level2", (5400f, 6400f, 7200f) },
        { "Level3", (5400f, 6900f, 8500f) },
        { "Level4", (4000f, 5000f, 6000f) },
        { "Level5", (4500f, 5500f, 6500f) }
    };

    public static bool IsLevelCompleted(string levelName)
    {
        if (GlobalScoreManager.Instance == null) return false;
        return GlobalScoreManager.Instance.IsLevelCompleted(levelName);
    }

    public static int GetStarCount(string levelName)
    {
        if (GlobalScoreManager.Instance == null) return 0;

        float score = GlobalScoreManager.Instance.GetScore(levelName);
        bool isCompleted = GlobalScoreManager.Instance.IsLevelCompleted(levelName);

        if (!isCompleted) return 0;

        Debug.Log($"[Stars] Checking stars for {levelName} with score {score}");

        (float oneStar, float twoStar, float threeStar) thresholds =
            starThresholds.ContainsKey(levelName) ? starThresholds[levelName] : (2000f, 3000f, 4000f);

        Debug.Log($"[Stars] Thresholds for {levelName}: 1★={thresholds.oneStar}, 2★={thresholds.twoStar}, 3★={thresholds.threeStar}");

        if (score >= thresholds.threeStar)
        {
            Debug.Log($"[Stars] {levelName} earned 3 stars with score {score} >= {thresholds.threeStar}");
            return 3;
        }
        if (score >= thresholds.twoStar)
        {
            Debug.Log($"[Stars] {levelName} earned 2 stars with score {score} >= {thresholds.twoStar}");
            return 2;
        }
        if (score >= thresholds.oneStar)
        {
            Debug.Log($"[Stars] {levelName} earned 1 star with score {score} >= {thresholds.oneStar}");
            return 1;
        }

        Debug.Log($"[Stars] {levelName} earned 0 stars with score {score}");
        return 0;
    }
}
