using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public string turma;
    public int mission1Score = -1;
    public int maxCombo = 0;
    public List<bool> mission1Answers = new List<bool>();
    public List<int> mission1ChosenAnswers = new List<int>();
}

[System.Serializable]
public class PlayerDatabase
{
    public List<PlayerData> players = new List<PlayerData>();
    public List<string> turmas = new List<string>();
}

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private TMP_Dropdown turmaDropdown;

    private string savePath;
    private PlayerDatabase database;

    private void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "players.json");
        LoadDatabase();
        CarregarTurmasNoDropdown();
    }

    private void LoadDatabase()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            database = JsonUtility.FromJson<PlayerDatabase>(json);
        }
        else
        {
            database = new PlayerDatabase();
        }
    }

    private void CarregarTurmasNoDropdown()
    {
        turmaDropdown.ClearOptions();

        if (database.turmas == null || database.turmas.Count == 0)
        {
            turmaDropdown.AddOptions(new List<string> { "Nenhuma turma cadastrada" });
            turmaDropdown.interactable = false;
            return;
        }

        turmaDropdown.interactable = true;
        turmaDropdown.AddOptions(new List<string>(database.turmas));
    }

    public void SaveDatabase()
    {
        try
        {
            string json = JsonUtility.ToJson(database, true);
            File.WriteAllText(savePath, json);
            Debug.Log("Database salvo em: " + savePath);
            Debug.Log("Conteudo: " + json);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao salvar database: " + e.Message);
        }
    }

    public void OnLoginButtonClicked()
    {
        string input = emailInputField.text != null ? emailInputField.text.Trim() : "";

        if (string.IsNullOrEmpty(input))
        {
            feedbackText.text = "Por favor, digite seu nome ou email.";
            feedbackText.gameObject.SetActive(true);
            return;
        }

        if (database.turmas == null || database.turmas.Count == 0)
        {
            feedbackText.text = "Nenhuma turma cadastrada. Peça ao professor para criar uma turma.";
            feedbackText.gameObject.SetActive(true);
            return;
        }

        feedbackText.gameObject.SetActive(false);

        string turmaSelecionada = database.turmas[turmaDropdown.value];

        PlayerPrefs.SetString("CurrentPlayer", input);
        PlayerPrefs.SetString("CurrentTurma", turmaSelecionada);
        PlayerPrefs.Save();

        PlayerData player = database.players.Find(p => p.playerName == input && p.turma == turmaSelecionada);
        if (player == null)
        {
            player = new PlayerData { playerName = input, turma = turmaSelecionada };
            database.players.Add(player);
            SaveDatabase();
        }

        SceneManager.LoadScene(1);
    }

    public void OnProfessorButtonClicked()
    {
        SceneManager.LoadScene("ProfessorLoginScene");
    }
}