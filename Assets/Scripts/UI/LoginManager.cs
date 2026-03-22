using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int mission1Score = -1; // -1 = missão não jogada ainda
}

[System.Serializable]
public class PlayerDatabase
{
    public List<PlayerData> players = new List<PlayerData>();
}

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_Text feedbackText;

    private string savePath;
    private PlayerDatabase database;

    private void Start()
    {
        // Define o caminho do arquivo JSON no dispositivo
        savePath = Path.Combine(Application.persistentDataPath, "players.json");
        LoadDatabase();
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

    public void SaveDatabase()
    {
        string json = JsonUtility.ToJson(database, true);
        File.WriteAllText(savePath, json);
    }

    public void OnLoginButtonClicked()
    {
        string input = emailInputField.text.Trim();

        if (string.IsNullOrEmpty(input))
        {
            feedbackText.text = "Por favor, digite seu nome ou email.";
            feedbackText.gameObject.SetActive(true);
            return;
        }

        // Verifica se o aluno já existe no banco, senão cria
        PlayerData player = database.players.Find(p => p.playerName == input);
        if (player == null)
        {
            player = new PlayerData { playerName = input };
            database.players.Add(player);
            SaveDatabase();
        }

        // Salva o nome do aluno atual no PlayerPrefs para uso durante a sessão
        PlayerPrefs.SetString("CurrentPlayer", input);
        PlayerPrefs.Save();

        SceneManager.LoadScene("MissionsScene");
    }
}