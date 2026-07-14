using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using Firebase.Firestore;
using System.Threading.Tasks;
using UnityEngine.UI;

public class RevisaoManager : MonoBehaviour
{
    [SerializeField] private Transform revisaoContainer;
    [SerializeField] private GameObject revisaoItemPrefab;

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

    private async void Start()
    {
        while (!FirebaseManager.IsReady)
            await Task.Delay(100);

        await CarregarRevisao();
    }

    private async Task CarregarRevisao()
    {
        string currentPlayer = PlayerPrefs.GetString("CurrentPlayer", "");
        if (string.IsNullOrEmpty(currentPlayer)) return;

        try
        {
            DocumentSnapshot snapshot = await FirebaseManager.Db
                .Collection("players")
                .Document(currentPlayer)
                .GetSnapshotAsync();

            if (!snapshot.Exists) return;

            List<bool> mission1Answers = snapshot.ContainsField("mission1Answers")
                ? new List<bool>(snapshot.GetValue<List<bool>>("mission1Answers"))
                : new List<bool>();

            List<int> mission1ChosenAnswers = snapshot.ContainsField("mission1ChosenAnswers")
                ? new List<int>(snapshot.GetValue<List<int>>("mission1ChosenAnswers"))
                : new List<int>();

            if (mission1Answers.Count == 0) return;

            for (int i = 0; i < perguntas.Length; i++)
            {
                bool acertou = i < mission1Answers.Count && mission1Answers[i];

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

                    if (i < mission1ChosenAnswers.Count)
                    {
                        int indiceEscolhido = mission1ChosenAnswers[i];
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

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(revisaoContainer.GetComponent<RectTransform>());
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao carregar revisão: " + e.Message);
        }
    }

    public void OnVoltarButtonClicked() { SceneManager.LoadScene("ResultScene"); }
}