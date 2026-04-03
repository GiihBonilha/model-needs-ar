using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ProfessorLoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField senhaInput;
    [SerializeField] private GameObject erroText;

    private void Start()
    {
        if (erroText != null)
            erroText.SetActive(false);
    }

    public void OnEntrarButtonClicked()
    {
        string senha = PlayerPrefs.GetString("ProfessorSenha", "1234");

        if (senhaInput.text == senha)
        {
            erroText.SetActive(false);
            SceneManager.LoadScene("AdminScene");
        }
        else
        {
            erroText.SetActive(true);
        }
    }

    public void OnVoltarButtonClicked()
    {
        SceneManager.LoadScene("LoginScene");
    }
}