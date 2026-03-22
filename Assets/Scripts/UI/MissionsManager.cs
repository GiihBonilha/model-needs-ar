using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

public class MissionsManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private GameObject mission1CompletedBadge;

    private string savePath;

    private void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "players.json");

        // Mostra o nome do aluno logado
        string currentPlayer = PlayerPrefs.GetString("CurrentPlayer", "");
        if (playerNameText != null)
            playerNameText.text = "Olá, " + currentPlayer + "!";

        // Verifica se a Missão 1 já foi concluída por esse aluno
        CheckMission1Status(currentPlayer);
    }

    private void CheckMission1Status(string playerName)
    {
        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        PlayerDatabase database = JsonUtility.FromJson<PlayerDatabase>(json);

        PlayerData player = database.players.Find(p => p.playerName == playerName);
        if (player != null && player.mission1Score >= 0)
        {
            // Missão 1 já foi jogada — mostra o badge de concluída
            if (mission1CompletedBadge != null)
                mission1CompletedBadge.SetActive(true);
        }
    }

    public void OnMission1ButtonClicked()
    {
        SceneManager.LoadScene("BriefingScene");
    }
}