using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using Firebase.Firestore;
using System.Threading.Tasks;
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
    [SerializeField] private RectTransform[] barrasAcerto;
    [SerializeField] private RectTransform[] barrasErro;
    [SerializeField] private TMP_Text[] labelsPercentualAcerto;
    [SerializeField] private TMP_Text[] labelsPercentualErro;
    [SerializeField] private TMP_Text totalCadastradosText;
    [SerializeField] private TMP_Text totalCompletaramText;

    [Header("Turmas")]
    [SerializeField] private TMP_InputField novaTurmaInput;
    [SerializeField] private Transform turmasContainer;
    [SerializeField] private GameObject turmaItemPrefab;
    [SerializeField] private Transform rankingTurmaContainer;
    [SerializeField] private GameObject rankingTurmaItemPrefab;
    [SerializeField] private TMP_Text turmaAtualText;

    private List<PlayerData> todosJogadores = new List<PlayerData>();
    private List<string> todasTurmas = new List<string>();
    private string turmaSelecionada = "";

    private async void Start()
    {
        while (!FirebaseManager.IsReady)
            await Task.Delay(100);

        await CarregarDados();
        MostrarResumo();
        MostrarPainelAlunos();
    }

    private async Task CarregarDados()
    {
        todosJogadores.Clear();
        todasTurmas.Clear();

        try
        {
            QuerySnapshot playersSnapshot = await FirebaseManager.Db
                .Collection("players")
                .GetSnapshotAsync();

            foreach (DocumentSnapshot doc in playersSnapshot.Documents)
            {
                PlayerData p = new PlayerData
                {
                    playerName = doc.ContainsField("playerName") ? doc.GetValue<string>("playerName") : doc.Id,
                    turma = doc.ContainsField("turma") ? doc.GetValue<string>("turma") : "",
                    mission1Score = doc.ContainsField("mission1Score") ? doc.GetValue<int>("mission1Score") : -1,
                    maxCombo = doc.ContainsField("maxCombo") ? doc.GetValue<int>("maxCombo") : 0,
                    mission1Answers = doc.ContainsField("mission1Answers") ? new List<bool>(doc.GetValue<List<bool>>("mission1Answers")) : new List<bool>(),
                    mission1ChosenAnswers = doc.ContainsField("mission1ChosenAnswers") ? new List<int>(doc.GetValue<List<int>>("mission1ChosenAnswers")) : new List<int>()
                };
                todosJogadores.Add(p);
            }

            QuerySnapshot turmasSnapshot = await FirebaseManager.Db
                .Collection("turmas")
                .GetSnapshotAsync();

            foreach (DocumentSnapshot doc in turmasSnapshot.Documents)
                todasTurmas.Add(doc.Id);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao carregar dados: " + e.Message);
        }
    }

    private void MostrarResumo()
    {
        List<PlayerData> jogadores = todosJogadores.FindAll(p => p.mission1Score >= 0);
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

        foreach (Transform filho in alunosContainer)
            Destroy(filho.gameObject);

        List<PlayerData> jogadores = todosJogadores.FindAll(p => p.mission1Score >= 0);
        jogadores.Sort((a, b) => b.mission1Score.CompareTo(a.mission1Score));

        foreach (PlayerData jogador in jogadores)
        {
            GameObject item = Instantiate(alunoItemPrefab, alunosContainer);
            item.transform.Find("NomeText").GetComponent<TMP_Text>().text = jogador.playerName;
            item.transform.Find("PontuacaoText").GetComponent<TMP_Text>().text = jogador.mission1Score + "/5";
            item.transform.Find("ComboText").GetComponent<TMP_Text>().text = "<sprite name=\"fire\"> " + jogador.maxCombo;

            string nome = jogador.playerName;
            item.transform.Find("BotaoReset").GetComponent<Button>().onClick.AddListener(() =>
            {
                ResetarAluno(nome);
            });
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(alunosContainer.GetComponent<RectTransform>());
    }

    public void MostrarPainelGrafico()
    {
        painelAlunos.SetActive(false);
        painelGrafico.SetActive(true);
        painelTurmas.SetActive(false);

        List<PlayerData> jogadores = todosJogadores.FindAll(p => p.mission1Score >= 0);
        int totalCompletaram = jogadores.Count;
        int totalCadastrados = todosJogadores.Count;

        if (totalCadastradosText != null)
            totalCadastradosText.text = "Total cadastrados: " + totalCadastrados;

        if (totalCompletaramText != null)
            totalCompletaramText.text = "Completaram a missão: " + totalCompletaram;

        if (totalCompletaram == 0) return;

        int[] acertos = new int[5];

        foreach (PlayerData jogador in jogadores)
        {
            if (jogador.mission1Answers == null) continue;
            for (int i = 0; i < jogador.mission1Answers.Count && i < 5; i++)
            {
                if (jogador.mission1Answers[i])
                    acertos[i]++;
            }
        }

        RectTransform painelRect = painelGrafico.GetComponent<RectTransform>();
        float alturaMaxima = painelRect.rect.height * 0.55f;

        for (int i = 0; i < 5; i++)
        {
            float percentAcerto = (float)acertos[i] / totalCompletaram;
            float percentErro = 1f - percentAcerto;

            if (barrasAcerto != null && i < barrasAcerto.Length)
                barrasAcerto[i].sizeDelta = new Vector2(barrasAcerto[i].sizeDelta.x, alturaMaxima * percentAcerto);

            if (barrasErro != null && i < barrasErro.Length)
                barrasErro[i].sizeDelta = new Vector2(barrasErro[i].sizeDelta.x, alturaMaxima * percentErro);

            if (labelsPercentualAcerto != null && i < labelsPercentualAcerto.Length)
                labelsPercentualAcerto[i].text = Mathf.RoundToInt(percentAcerto * 100) + "%";

            if (labelsPercentualErro != null && i < labelsPercentualErro.Length)
                labelsPercentualErro[i].text = Mathf.RoundToInt(percentErro * 100) + "%";
        }
    }

    public void MostrarPainelTurmas()
    {
        painelAlunos.SetActive(false);
        painelGrafico.SetActive(false);
        painelTurmas.SetActive(true);

        AtualizarListaTurmas();
    }

    private void AtualizarListaTurmas()
    {
        foreach (Transform filho in turmasContainer)
            Destroy(filho.gameObject);

        foreach (string turma in todasTurmas)
        {
            GameObject item = Instantiate(turmaItemPrefab, turmasContainer);
            item.transform.Find("NomeTurmaText").GetComponent<TMP_Text>().text = turma;

            string nomeTurma = turma;
            item.transform.Find("BotaoVerRanking").GetComponent<Button>().onClick.AddListener(() =>
            {
                MostrarRankingTurma(nomeTurma);
            });

            item.transform.Find("BotaoDeletarTurma").GetComponent<Button>().onClick.AddListener(() =>
            {
                DeletarTurma(nomeTurma);
            });
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(turmasContainer.GetComponent<RectTransform>());
    }

    public async void OnCriarTurmaClicked()
    {
        string nomeTurma = novaTurmaInput.text.Trim();
        if (string.IsNullOrEmpty(nomeTurma)) return;

        if (todasTurmas.Contains(nomeTurma))
        {
            Debug.Log("Turma já existe!");
            return;
        }

        try
        {
            await FirebaseManager.Db
                .Collection("turmas")
                .Document(nomeTurma)
                .SetAsync(new Dictionary<string, object> { { "nome", nomeTurma } });

            todasTurmas.Add(nomeTurma);
            novaTurmaInput.text = "";
            AtualizarListaTurmas();
            Debug.Log("Turma criada no Firestore: " + nomeTurma);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao criar turma: " + e.Message);
        }
    }

    private async void DeletarTurma(string nomeTurma)
    {
        try
        {
            await FirebaseManager.Db
                .Collection("turmas")
                .Document(nomeTurma)
                .DeleteAsync();

            todasTurmas.Remove(nomeTurma);
            AtualizarListaTurmas();

            if (turmaSelecionada == nomeTurma)
            {
                turmaSelecionada = "";
                if (turmaAtualText != null) turmaAtualText.text = "";
                foreach (Transform filho in rankingTurmaContainer)
                    Destroy(filho.gameObject);
            }
            Debug.Log("Turma deletada: " + nomeTurma);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao deletar turma: " + e.Message);
        }
    }

    private void MostrarRankingTurma(string nomeTurma)
    {
        turmaSelecionada = nomeTurma;

        if (turmaAtualText != null)
            turmaAtualText.text = "Ranking: " + nomeTurma;

        foreach (Transform filho in rankingTurmaContainer)
            Destroy(filho.gameObject);

        List<PlayerData> jogadores = todosJogadores.FindAll(p => p.turma == nomeTurma && p.mission1Score >= 0);
        jogadores.Sort((a, b) =>
        {
            if (b.mission1Score != a.mission1Score)
                return b.mission1Score.CompareTo(a.mission1Score);
            return b.maxCombo.CompareTo(a.maxCombo);
        });

        for (int i = 0; i < jogadores.Count; i++)
        {
            PlayerData jogador = jogadores[i];
            GameObject item = Instantiate(rankingTurmaItemPrefab, rankingTurmaContainer);

            string posicao = i == 0 ? "1°" : i == 1 ? "2°" : i == 2 ? "3°" : (i + 1) + "°";
            item.transform.Find("PosicaoText").GetComponent<TMP_Text>().text = posicao;
            item.transform.Find("NomeText").GetComponent<TMP_Text>().text = jogador.playerName;
            item.transform.Find("PontuacaoText").GetComponent<TMP_Text>().text = jogador.mission1Score + "/5";
            item.transform.Find("ComboText").GetComponent<TMP_Text>().text = "<sprite name=\"fire\"> " + jogador.maxCombo;
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rankingTurmaContainer.GetComponent<RectTransform>());
    }

    private async void ResetarAluno(string playerName)
    {
        try
        {
            await FirebaseManager.Db
                .Collection("players")
                .Document(playerName)
                .UpdateAsync(new Dictionary<string, object>
                {
                    { "mission1Score", -1 },
                    { "maxCombo", 0 },
                    { "mission1Answers", new List<bool>() },
                    { "mission1ChosenAnswers", new List<int>() }
                });

            PlayerData player = todosJogadores.Find(p => p.playerName == playerName);
            if (player != null)
            {
                player.mission1Score = -1;
                player.maxCombo = 0;
                player.mission1Answers = new List<bool>();
                player.mission1ChosenAnswers = new List<int>();
            }

            MostrarPainelAlunos();
            Debug.Log("Aluno resetado: " + playerName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao resetar aluno: " + e.Message);
        }
    }

    public void OnVoltarButtonClicked()
    {
        SceneManager.LoadScene("LoginScene");
    }
}