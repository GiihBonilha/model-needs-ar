using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

public class MissionsManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private GameObject mission1CompletedBadge;

    [Header("Popup de Confirmação")]
    [SerializeField] private GameObject popupConfirmacao;

    private string savePath;

    private void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "players.json");

        // Mostra o nome do aluno logado
        string currentPlayer = PlayerPrefs.GetString("CurrentPlayer", "");
        if (playerNameText != null)
            playerNameText.text = "Olá, " + currentPlayer + "!";

        // Garante que o popup começa desativado
        if (popupConfirmacao != null)
            popupConfirmacao.SetActive(false);

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
    public void OnRankingButtonClicked()
    {
        SceneManager.LoadScene("RankingScene");
    }

    // Chamado pelo botão Sair — abre o popup
    public void OnSairButtonClicked()
    {
        if (popupConfirmacao != null)
            popupConfirmacao.SetActive(true);
    }

    // Chamado pelo botão Sim no popup — volta para a LoginScene
    public void OnConfirmarSairClicked()
    {
        PlayerPrefs.DeleteKey("CurrentPlayer");
        SceneManager.LoadScene("LoginScene");
    }

    // Chamado pelo botão Não no popup — fecha o popup
    public void OnCancelarSairClicked()
    {
        if (popupConfirmacao != null)
            popupConfirmacao.SetActive(false);
    }

}