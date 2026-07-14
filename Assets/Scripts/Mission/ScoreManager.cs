using UnityEngine;
using System.Collections.Generic;
using Firebase.Firestore;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int totalScore = 0;
    private int maxCombo = 0;
    private List<bool> answers = new List<bool>();
    private List<int> chosenAnswers = new List<int>();

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
        totalScore = 0;
        maxCombo = 0;
        answers = new List<bool>();
        chosenAnswers = new List<int>();
    }

    public void RegisterAnswer(bool isCorrect, int chosenIndex)
    {
        if (isCorrect) totalScore++;
        answers.Add(isCorrect);
        chosenAnswers.Add(chosenIndex);
    }

    public void SetMaxCombo(int combo) { maxCombo = combo; }
    public int GetTotalScore() { return totalScore; }
    public int GetMaxCombo() { return maxCombo; }

    public async void SaveScore()
    {
        string currentPlayer = PlayerPrefs.GetString("CurrentPlayer", "");
        if (string.IsNullOrEmpty(currentPlayer)) return;

        try
        {
            await FirebaseManager.Db
                .Collection("players")
                .Document(currentPlayer)
                .UpdateAsync(new Dictionary<string, object>
                {
                    { "mission1Score", totalScore },
                    { "maxCombo", maxCombo },
                    { "mission1Answers", answers },
                    { "mission1ChosenAnswers", chosenAnswers }
                });

            Debug.Log("Score salvo no Firestore!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao salvar score: " + e.Message);
        }
    }
}