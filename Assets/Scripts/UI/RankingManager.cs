using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using Firebase.Firestore;
using System.Threading.Tasks;
using UnityEngine.UI;

public class RankingManager : MonoBehaviour
{
    [SerializeField] private Transform rankingContainer;
    [SerializeField] private GameObject rankingItemPrefab;

    private async void Start()
    {
        while (!FirebaseManager.IsReady)
            await Task.Delay(100);

        await LoadRanking();
    }

    private async Task LoadRanking()
    {
        try
        {
            QuerySnapshot snapshot = await FirebaseManager.Db
                .Collection("players")
                .GetSnapshotAsync();

            List<PlayerData> jogadores = new List<PlayerData>();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                int score = doc.ContainsField("mission1Score") ? doc.GetValue<int>("mission1Score") : -1;
                if (score < 0) continue;

                PlayerData p = new PlayerData
                {
                    playerName = doc.ContainsField("playerName") ? doc.GetValue<string>("playerName") : doc.Id,
                    turma = doc.ContainsField("turma") ? doc.GetValue<string>("turma") : "",
                    mission1Score = score,
                    maxCombo = doc.ContainsField("maxCombo") ? doc.GetValue<int>("maxCombo") : 0
                };
                jogadores.Add(p);
            }

            jogadores.Sort((a, b) =>
            {
                if (b.mission1Score != a.mission1Score)
                    return b.mission1Score.CompareTo(a.mission1Score);
                return b.maxCombo.CompareTo(a.maxCombo);
            });

            for (int i = 0; i < jogadores.Count; i++)
            {
                PlayerData jogador = jogadores[i];
                GameObject item = Instantiate(rankingItemPrefab, rankingContainer);

                item.transform.Find("PosicaoText").GetComponent<TMP_Text>().text = GetMedal(i + 1);
                item.transform.Find("NomeText").GetComponent<TMP_Text>().text = jogador.playerName;
                item.transform.Find("PontuacaoText").GetComponent<TMP_Text>().text = jogador.mission1Score + "/5";
                item.transform.Find("ComboText").GetComponent<TMP_Text>().text = "<sprite name=\"fire\"> " + jogador.maxCombo;
            }

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rankingContainer.GetComponent<RectTransform>());
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao carregar ranking: " + e.Message);
        }
    }

    private string GetMedal(int posicao)
    {
        switch (posicao)
        {
            case 1: return "<sprite name=\"first\">";
            case 2: return "<sprite name=\"second\">";
            case 3: return "<sprite name=\"third\">";
            default: return posicao + "º";
        }
    }

    public void OnVoltarButtonClicked() { SceneManager.LoadScene("MissionScene"); }
}