using Assets.Scrips;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalScoreManager : MonoBehaviour
{
    public static GlobalScoreManager Instance { get; private set; }

    private Dictionary<string, List<HighScoreEntry>> allScores = new();

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

    public void SaveScore(string playerName, float score)
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (!allScores.ContainsKey(sceneName))
            allScores[sceneName] = new List<HighScoreEntry>();

        allScores[sceneName].Add(new HighScoreEntry(playerName, score));

        allScores[sceneName].Sort((a, b) => b.score.CompareTo(a.score));

        if (allScores[sceneName].Count > 5)
            allScores[sceneName].RemoveRange(5, allScores[sceneName].Count - 5);

        SaveScoresToFile();
    }

    public List<HighScoreEntry> GetTopScores(string sceneName)
    {
        if (allScores.ContainsKey(sceneName))
            return allScores[sceneName];

        return new List<HighScoreEntry>();
    }

    private void LoadScoresFromFile()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            allScores = JsonUtility.FromJson<ScoreDataWrapper>(json).ToDictionary();
        }
    }

    private void SaveScoresToFile()
    {
        ScoreDataWrapper wrapper = new(allScores);
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("Saved JSON to: " + SavePath);
    }

    [System.Serializable]
    private class ScoreDataWrapper
    {
        public List<SceneScoreData> scenes = new();

        public ScoreDataWrapper(Dictionary<string, List<HighScoreEntry>> dict)
        {
            foreach (var kvp in dict)
                scenes.Add(new SceneScoreData { sceneName = kvp.Key, scores = kvp.Value });
        }

        public Dictionary<string, List<HighScoreEntry>> ToDictionary()
        {
            Dictionary<string, List<HighScoreEntry>> dict = new();
            foreach (var item in scenes)
                dict[item.sceneName] = item.scores;
            return dict;
        }
    }

    [System.Serializable]
    private class SceneScoreData
    {
        public string sceneName;
        public List<HighScoreEntry> scores;
    }
}
