using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using Firebase.Firestore;
using System.Threading.Tasks;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public string turma;
    public int mission1Score = -1;
    public int maxCombo = 0;
    public List<bool> mission1Answers = new List<bool>();
    public List<int> mission1ChosenAnswers = new List<int>();
}

[System.Serializable]
public class PlayerDatabase
{
    public List<PlayerData> players = new List<PlayerData>();
    public List<string> turmas = new List<string>();
}

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private TMP_Dropdown turmaDropdown;

    private List<string> turmas = new List<string>();

    private async void Start()
    {
        await WaitForFirebase();
        await CarregarTurmasDoFirestore();
    }

    private async Task WaitForFirebase()
    {
        while (!FirebaseManager.IsReady)
            await Task.Delay(100);
    }

    private async Task CarregarTurmasDoFirestore()
{
    turmaDropdown.ClearOptions();
    turmas.Clear();

    try
    {
        Debug.Log("Tentando carregar turmas...");
        
        QuerySnapshot snapshot = await FirebaseManager.Db
            .Collection("turmas")
            .GetSnapshotAsync();

        Debug.Log("Turmas encontradas: " + snapshot.Count);

        foreach (DocumentSnapshot doc in snapshot.Documents)
        {
            Debug.Log("Turma: " + doc.Id);
            turmas.Add(doc.Id);
        }

        if (turmas.Count == 0)
        {
            turmaDropdown.AddOptions(new List<string> { "Nenhuma turma cadastrada" });
            turmaDropdown.interactable = false;
        }
        else
        {
            turmaDropdown.interactable = true;
            turmaDropdown.AddOptions(new List<string>(turmas));
        }
    }
    catch (System.Exception e)
    {
        Debug.LogError("Erro ao carregar turmas: " + e.Message);
    }
}

    public async void OnLoginButtonClicked()
    {
        string input = emailInputField.text != null ? emailInputField.text.Trim() : "";

        if (string.IsNullOrEmpty(input))
        {
            feedbackText.text = "Por favor, digite seu nome.";
            feedbackText.gameObject.SetActive(true);
            return;
        }

        if (turmas == null || turmas.Count == 0)
        {
            feedbackText.text = "Nenhuma turma cadastrada. Peça ao professor para criar uma turma.";
            feedbackText.gameObject.SetActive(true);
            return;
        }

        feedbackText.gameObject.SetActive(false);

        string turmaSelecionada = turmas[turmaDropdown.value];

        PlayerPrefs.SetString("CurrentPlayer", input);
        PlayerPrefs.SetString("CurrentTurma", turmaSelecionada);
        PlayerPrefs.Save();

        try
        {
            DocumentReference docRef = FirebaseManager.Db
                .Collection("players")
                .Document(input);

            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                await docRef.SetAsync(new Dictionary<string, object>
                {
                    { "playerName", input },
                    { "turma", turmaSelecionada },
                    { "mission1Score", -1 },
                    { "maxCombo", 0 },
                    { "mission1Answers", new List<bool>() },
                    { "mission1ChosenAnswers", new List<int>() }
                });
            }

            SceneManager.LoadScene(1);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao fazer login: " + e.Message);
        }
    }

    public void OnProfessorButtonClicked()
    {
        SceneManager.LoadScene("ProfessorLoginScene");
    }
}