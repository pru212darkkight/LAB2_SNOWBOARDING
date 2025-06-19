using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalScoreManager : MonoBehaviour
{
    public static GlobalScoreManager Instance { get; private set; }

    private Dictionary<string, float> highScoresPerLevel = new();
    private Dictionary<string, bool> completedLevels = new();

    private string SavePath => Path.Combine(Application.persistentDataPath, "highscores.json");

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadScoresFromFile();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveScore(float newScore)
    {
        string scene = SceneManager.GetActiveScene().name;

        if (!highScoresPerLevel.ContainsKey(scene) || newScore > highScoresPerLevel[scene])
        {
            highScoresPerLevel[scene] = newScore;
            SaveScoresToFile();
            Debug.Log($"Đang thử lưu điểm {newScore} ở scene {scene}");
            Debug.Log("Saving to: " + SavePath);
        }
    }

    public float GetScore(string scene)
    {
        return highScoresPerLevel.TryGetValue(scene, out float score) ? score : 0f;
    }

    public void MarkLevelCompleted()
    {
        string scene = SceneManager.GetActiveScene().name;
        completedLevels[scene] = true;
        SaveScoresToFile();
    }

    public bool IsLevelCompleted(string scene)
    {
        return completedLevels.TryGetValue(scene, out bool completed) && completed;
    }

    private void LoadScoresFromFile()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            ScoreWrapper wrapper = JsonUtility.FromJson<ScoreWrapper>(json);
            highScoresPerLevel = wrapper?.ToDictionary(out completedLevels) ?? new();
        }
    }

    private void SaveScoresToFile()
    {
        ScoreWrapper wrapper = new ScoreWrapper(highScoresPerLevel, completedLevels);
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(SavePath, json);
    }

    [System.Serializable]
    private class ScoreWrapper
    {
        public List<LevelScore> scores = new();

        public ScoreWrapper(Dictionary<string, float> dict, Dictionary<string, bool> completionDict)
        {
            foreach (var kvp in dict)
            {
                scores.Add(new LevelScore
                {
                    levelName = kvp.Key,
                    score = kvp.Value,
                    isCompleted = completionDict.TryGetValue(kvp.Key, out bool val) && val
                });
            }
        }

        public Dictionary<string, float> ToDictionary(out Dictionary<string, bool> completionDict)
        {
            Dictionary<string, float> scoreDict = new();
            completionDict = new();
            foreach (var s in scores)
            {
                scoreDict[s.levelName] = s.score;
                completionDict[s.levelName] = s.isCompleted;
            }
            return scoreDict;
        }
    }

    [System.Serializable]
    private class LevelScore
    {
        public string levelName;
        public float score;
        public bool isCompleted;
    }
}
