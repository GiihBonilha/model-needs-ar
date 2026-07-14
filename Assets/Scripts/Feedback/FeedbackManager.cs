using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Firestore;
using System.Threading.Tasks;

public class FeedbackManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private TMP_Text comboText;

    private async void Start()
    {
        while (!FirebaseManager.IsReady)
            await Task.Delay(100);

        string currentPlayer = PlayerPrefs.GetString("CurrentPlayer", "");
        await CarregarDados(currentPlayer);
    }

    private async Task CarregarDados(string playerName)
    {
        try
        {
            DocumentSnapshot snapshot = await FirebaseManager.Db
                .Collection("players")
                .Document(playerName)
                .GetSnapshotAsync();

            if (!snapshot.Exists) return;

            int score = snapshot.ContainsField("mission1Score") ? snapshot.GetValue<int>("mission1Score") : 0;
            int combo = snapshot.ContainsField("maxCombo") ? snapshot.GetValue<int>("maxCombo") : 0;

            scoreText.text = score + " / 5";
            feedbackText.text = GetFeedbackText(score);

            if (comboText != null)
            {
                if (combo >= 2)
                    comboText.text = "Maior combo: " + "<sprite name=\"fire\">" + combo;
                else
                    comboText.text = "";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao carregar feedback: " + e.Message);
        }
    }

    private string GetFeedbackText(int score)
    {
        if (score == 5) return "Você demonstrou ótima compreensão sobre os requisitos relacionados ao modelo de IA. Suas escolhas mostram que você entendeu bem como o sistema deve aprender, se adaptar ao usuário e utilizar feedbacks para melhorar suas recomendações. Você está preparado para identificar decisões importantes sobre Model Needs em sistemas com IA.";
        else if (score == 4) return "Você demonstrou um bom conhecimento sobre os requisitos do modelo de IA. Suas respostas indicam que você compreende a maior parte das decisões necessárias para que o sistema aprenda e evolua com os dados dos usuários. Vale apenas revisar alguns pontos para fortalecer ainda mais sua compreensão.";
        else if (score == 3) return "Você acertou parte importante da missão, mas ainda há aspectos dos requisitos do modelo de IA que precisam de mais atenção. Isso indica que você já tem uma base inicial, mas ainda precisa revisar como o modelo deve ser treinado, atualizado e adaptado com base no comportamento do usuário.";
        else if (score == 2) return "Suas respostas mostram que você ainda tem dificuldades para identificar decisões importantes sobre Model Needs. Em sistemas com IA, definir corretamente como o modelo aprende, usa feedback e se adapta ao contexto do usuário é essencial para garantir recomendações úteis e personalizadas.";
        else return "Suas escolhas indicam que os conceitos de Model Needs ainda não foram compreendidos adequadamente. Recomendamos revisar os pontos principais da missão. Entender esses elementos é fundamental para definir requisitos adequados para sistemas com IA.";
    }

    public void OnConcludeButtonClicked() { SceneManager.LoadScene(1); }
    public void OnRankingButtonClicked() { SceneManager.LoadScene("RankingScene"); }
    public void OnRevisaoButtonClicked() { SceneManager.LoadScene("RevisaoScene"); }
}