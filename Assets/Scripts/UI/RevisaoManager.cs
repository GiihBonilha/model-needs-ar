using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using System.Collections.Generic;

public class RevisaoManager : MonoBehaviour
{
    [SerializeField] private Transform revisaoContainer;
    [SerializeField] private GameObject revisaoItemPrefab;

    private string savePath;

    private string[] perguntas = {
        "O treinamento do modelo será:",
        "Qual abordagem de aprendizado é mais adequada?",
        "Como o sistema deve utilizar o feedback do usuário?",
        "Como essa adaptação deve acontecer?",
        "Qual deve ser o impacto das novas entradas no sistema?"
    };

    private string[] respostasCorretas = {
        "B) Dinâmico — atualizado continuamente",
        "A) Aprendizado supervisionado",
        "B) Usar feedback para ajustar futuras recomendações",
        "B) O sistema reduz sugestões com ingredientes rejeitados",
        "C) Atualização imediata das recomendações"
    };

    private string[][] todasRespostas = {
        new string[] { "A) Estático — treinado apenas uma vez", "B) Dinâmico — atualizado continuamente" },
        new string[] { "A) Aprendizado supervisionado", "B) Aprendizado não supervisionado", "C) Aprendizado por reforço" },
        new string[] { "A) Ignorar feedback do usuário", "B) Usar feedback para ajustar futuras recomendações", "C) Apenas registrar feedback sem alterar o modelo" },
        new string[] { "A) O sistema continua sugerindo os mesmos pratos", "B) O sistema reduz sugestões com ingredientes rejeitados", "C) O sistema reinicia as preferências do usuário" },
        new string[] { "A) Nenhum impacto nas recomendações", "B) Impacto apenas após a próxima atualização", "C) Atualização imediata das recomendações" }
    };

    private void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "players.json");
        CarregarRevisao();
    }

    private void CarregarRevisao()
    {
        string currentPlayer = PlayerPrefs.GetString("CurrentPlayer", "");
        if (string.IsNullOrEmpty(currentPlayer)) return;

        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        PlayerDatabase database = JsonUtility.FromJson<PlayerDatabase>(json);
        PlayerData player = database.players.Find(p => p.playerName == currentPlayer);

        if (player == null || player.mission1Answers == null) return;

        for (int i = 0; i < perguntas.Length; i++)
        {
            bool acertou = i < player.mission1Answers.Count && player.mission1Answers[i];

            GameObject item = Instantiate(revisaoItemPrefab, revisaoContainer);

            TMP_Text statusText = item.transform.Find("StatusText").GetComponent<TMP_Text>();
            TMP_Text perguntaText = item.transform.Find("PerguntaText").GetComponent<TMP_Text>();
            TMP_Text respostaAlunoText = item.transform.Find("RespostaAlunoText").GetComponent<TMP_Text>();
            TMP_Text respostaCorretaText = item.transform.Find("RespostaCorretaText").GetComponent<TMP_Text>();

            perguntaText.text = "Pergunta " + (i + 1) + ": " + perguntas[i];

            if (acertou)
            {
                statusText.text = "[CERTO]";
                statusText.color = new Color(0.298f, 0.686f, 0.314f);
                respostaAlunoText.gameObject.SetActive(false);
                respostaCorretaText.text = respostasCorretas[i];
                respostaCorretaText.color = new Color(0.298f, 0.686f, 0.314f);
            }
            else
            {
                statusText.text = "[ERROU]";
                statusText.color = new Color(0.957f, 0.263f, 0.212f);

                // Mostra a resposta que o aluno escolheu
                if (player.mission1ChosenAnswers != null && i < player.mission1ChosenAnswers.Count)
                {
                    int indiceEscolhido = player.mission1ChosenAnswers[i];
                    if (indiceEscolhido < todasRespostas[i].Length)
                    {
                        respostaAlunoText.text = "Sua resposta: " + todasRespostas[i][indiceEscolhido];
                        respostaAlunoText.color = new Color(0.957f, 0.263f, 0.212f);
                    }
                }

                respostaCorretaText.text = "Resposta correta: " + respostasCorretas[i];
                respostaCorretaText.color = new Color(0.298f, 0.686f, 0.314f);
            }
        }
    }

    public void OnVoltarButtonClicked()
    {
        SceneManager.LoadScene("ResultScene");
    }
}