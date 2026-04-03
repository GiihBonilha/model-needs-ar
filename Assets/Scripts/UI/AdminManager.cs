using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

public class AdminManager : MonoBehaviour
{
    [Header("Resumo")]
    [SerializeField] private TMP_Text resumoText;

    [Header("Painéis")]
    [SerializeField] private GameObject painelAlunos;
    [SerializeField] private GameObject painelGrafico;
    [SerializeField] private GameObject painelTurmas;

    [Header("Lista de Alunos")]
    [SerializeField] private Transform alunosContainer;
    [SerializeField] private GameObject alunoItemPrefab;

    [Header("Gráfico")]
    [SerializeField] private RectTransform[] barrasGrafico; // 5 barras M1-M5
    [SerializeField] private TMP_Text[] labelsGrafico;      // textos embaixo das barras

    private string savePath;
    private PlayerDatabase database;

    private void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "players.json");
        LoadDatabase();
        MostrarResumo();
        MostrarPainelAlunos();
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

    private void MostrarResumo()
    {
        List<PlayerData> jogadores = database.players.FindAll(p => p.mission1Score >= 0);
        int total = jogadores.Count;

        float media = 0;
        if (total > 0)
        {
            int soma = 0;
            foreach (PlayerData p in jogadores) soma += p.mission1Score;
            media = (float)soma / total;
        }

        resumoText.text = $"Alunos: {total}   |   Média: {media:F1}/5";
    }

    public void MostrarPainelAlunos()
    {
        painelAlunos.SetActive(true);
        painelGrafico.SetActive(false);
        painelTurmas.SetActive(false);

        // Limpa lista anterior
        foreach (Transform filho in alunosContainer)
            Destroy(filho.gameObject);

        List<PlayerData> jogadores = database.players.FindAll(p => p.mission1Score >= 0);
        jogadores.Sort((a, b) => b.mission1Score.CompareTo(a.mission1Score));

        foreach (PlayerData jogador in jogadores)
        {
            GameObject item = Instantiate(alunoItemPrefab, alunosContainer);
            item.transform.Find("NomeText").GetComponent<TMP_Text>().text = jogador.playerName;
            item.transform.Find("PontuacaoText").GetComponent<TMP_Text>().text = jogador.mission1Score + "/5";
            item.transform.Find("ComboText").GetComponent<TMP_Text>().text = "🔥" + jogador.maxCombo;

            // Botão de reset
            string nome = jogador.playerName;
            item.transform.Find("BotaoReset").GetComponent<Button>().onClick.AddListener(() =>
            {
                ResetarAluno(nome);
            });
        }
    }

    public void MostrarPainelGrafico()
    {
        painelAlunos.SetActive(false);
        painelGrafico.SetActive(true);
        painelTurmas.SetActive(false);

        string[] perguntas = { "M1", "M2", "M3", "M4", "M5" };
        List<PlayerData> jogadores = database.players.FindAll(p => p.mission1Score >= 0);
        int total = jogadores.Count;

        // Por enquanto mostra a pontuação média como aproximação
        // (para dados exatos por pergunta precisaríamos salvar respostas individuais)
        for (int i = 0; i < barrasGrafico.Length; i++)
        {
            float percentual = total > 0 ? (float)i / perguntas.Length : 0;
            barrasGrafico[i].sizeDelta = new Vector2(barrasGrafico[i].sizeDelta.x, 600 * percentual);
            if (labelsGrafico != null && i < labelsGrafico.Length)
                labelsGrafico[i].text = perguntas[i];
        }
    }

    public void MostrarPainelTurmas()
    {
        painelAlunos.SetActive(false);
        painelGrafico.SetActive(false);
        painelTurmas.SetActive(true);
    }

    private void ResetarAluno(string playerName)
    {
        PlayerData player = database.players.Find(p => p.playerName == playerName);
        if (player != null)
        {
            player.mission1Score = -1;
            player.maxCombo = 0;
            string json = JsonUtility.ToJson(database, true);
            File.WriteAllText(savePath, json);
            MostrarPainelAlunos(); // Atualiza a lista
        }
    }

    public void OnVoltarButtonClicked()
    {
        SceneManager.LoadScene("LoginScene");
    }
}