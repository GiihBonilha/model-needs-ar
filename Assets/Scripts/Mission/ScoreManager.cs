using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int totalScore = 0;
    private int totalQuestions = 5;
    private int maxCombo = 0;
    private List<bool> answers = new List<bool>();
    private List<int> chosenAnswers = new List<int>();
    private string savePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "players.json");
        totalScore = 0;
        maxCombo = 0;
        answers = new List<bool>();
        chosenAnswers = new List<int>();
    }

    public void RegisterAnswer(bool isCorrect, int chosenIndex)
    {
        if (isCorrect)
            totalScore++;
        answers.Add(isCorrect);
        chosenAnswers.Add(chosenIndex);
    }

    public void SetMaxCombo(int combo)
    {
        maxCombo = combo;
    }

    public int GetTotalScore()
    {
        return totalScore;
    }

    public int GetMaxCombo()
    {
        return maxCombo;
    }

    public void SaveScore()
    {
        string currentPlayer = PlayerPrefs.GetString("CurrentPlayer", "");
        if (string.IsNullOrEmpty(currentPlayer)) return;

        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        PlayerDatabase database = JsonUtility.FromJson<PlayerDatabase>(json);

        PlayerData player = database.players.Find(p => p.playerName == currentPlayer);
        if (player != null)
        {
            player.mission1Score = totalScore;
            player.maxCombo = maxCombo;
            player.mission1Answers = new List<bool>(answers);
            player.mission1ChosenAnswers = new List<int>(chosenAnswers);
            string updatedJson = JsonUtility.ToJson(database, true);
            File.WriteAllText(savePath, updatedJson);
        }
    }
}