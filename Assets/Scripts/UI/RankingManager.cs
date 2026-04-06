using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

public class RankingManager : MonoBehaviour
{
    [SerializeField] private Transform rankingContainer;
    [SerializeField] private GameObject rankingItemPrefab;

    private string savePath;

    private void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "players.json");
        LoadRanking();
    }

    public void OnVoltarButtonClicked()
    {
        SceneManager.LoadScene("MissionScene");
    }

    private void LoadRanking()
    {
        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        PlayerDatabase database = JsonUtility.FromJson<PlayerDatabase>(json);

        // Filtra só quem já jogou (score >= 0)
        List<PlayerData> jogadores = database.players.FindAll(p => p.mission1Score >= 0);

        // Ordena por pontuação (maior primeiro), combo como desempate
        jogadores.Sort((a, b) =>
        {
            if (b.mission1Score != a.mission1Score)
                return b.mission1Score.CompareTo(a.mission1Score);
            return b.maxCombo.CompareTo(a.maxCombo);
        });

        // Cria uma linha para cada jogador
        for (int i = 0; i < jogadores.Count; i++)
        {
            PlayerData jogador = jogadores[i];
            GameObject item = Instantiate(rankingItemPrefab, rankingContainer);

            item.transform.Find("PosicaoText").GetComponent<TMP_Text>().text = GetMedal(i + 1);
            item.transform.Find("NomeText").GetComponent<TMP_Text>().text = jogador.playerName;
            item.transform.Find("PontuacaoText").GetComponent<TMP_Text>().text = jogador.mission1Score + "/5";
            item.transform.Find("ComboText").GetComponent<TMP_Text>().text = "🔥" + jogador.maxCombo;
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rankingContainer.GetComponent<RectTransform>());
    }

    private string GetMedal(int posicao)
    {
        switch (posicao)
        {
            case 1: return "🥇";
            case 2: return "🥈";
            case 3: return "🥉";
            default: return posicao + "º";
        }
    }

}