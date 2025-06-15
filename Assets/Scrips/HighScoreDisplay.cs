using Assets.Scrips;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HighScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] scoreTexts;

    void Start()
    {
        string currentScene = PlayerPrefs.GetString("LastPlayedScene", SceneManager.GetActiveScene().name);

        List<HighScoreEntry> scores = GlobalScoreManager.Instance.GetTopScores(currentScene);

        for (int i = 0; i < scoreTexts.Length; i++)
        {
            if (i < scores.Count)
                scoreTexts[i].text = $"{i + 1}. {scores[i].playerName} - {Mathf.FloorToInt(scores[i].score)}";
            else
                scoreTexts[i].text = $"{i + 1}. ---";
        }
    }
}
