using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Firestore;
using System.Threading.Tasks;

public class MissionsManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private GameObject mission1CompletedBadge;

    [Header("Popup de Confirmação")]
    [SerializeField] private GameObject popupConfirmacao;

    private async void Start()
    {
        while (!FirebaseManager.IsReady)
            await Task.Delay(100);

        string currentPlayer = PlayerPrefs.GetString("CurrentPlayer", "");
        if (playerNameText != null)
            playerNameText.text = "Olá, " + currentPlayer + "!";

        if (popupConfirmacao != null)
            popupConfirmacao.SetActive(false);

        await CheckMission1Status(currentPlayer);
    }

    private async Task CheckMission1Status(string playerName)
    {
        try
        {
            DocumentSnapshot snapshot = await FirebaseManager.Db
                .Collection("players")
                .Document(playerName)
                .GetSnapshotAsync();

            if (snapshot.Exists && snapshot.ContainsField("mission1Score"))
            {
                int score = snapshot.GetValue<int>("mission1Score");
                if (score >= 0 && mission1CompletedBadge != null)
                    mission1CompletedBadge.SetActive(true);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao checar missão: " + e.Message);
        }
    }

    public void OnMission1ButtonClicked() { SceneManager.LoadScene("BriefingScene"); }
    public void OnRankingButtonClicked() { SceneManager.LoadScene("RankingScene"); }
    public void OnSairButtonClicked() { if (popupConfirmacao != null) popupConfirmacao.SetActive(true); }
    public void OnConfirmarSairClicked() { PlayerPrefs.DeleteKey("CurrentPlayer"); SceneManager.LoadScene("LoginScene"); }
    public void OnCancelarSairClicked() { if (popupConfirmacao != null) popupConfirmacao.SetActive(false); }
}