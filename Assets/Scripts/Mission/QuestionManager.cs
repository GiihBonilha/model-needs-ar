using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QuestionManager : MonoBehaviour
{
    [Header("Painel de Perguntas")]
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private List<Button> answerButtons;
    [SerializeField] private List<TMP_Text> answerTexts;

    private Question currentQuestion;
    private System.Action<AnswerOption> onAnswerSelected;

    public void ShowQuestion(Question question, System.Action<AnswerOption> onAnswer)
    {
        currentQuestion = question;
        onAnswerSelected = onAnswer;

        questionText.text = question.questionText;

        // Configura cada botão de resposta
        for (int i = 0; i < answerButtons.Count; i++)
        {
            if (i < question.answers.Count)
            {
                int index = i; // captura o índice para o lambda
                answerTexts[i].text = question.answers[i].text;
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => OnAnswerClicked(index));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }

        questionPanel.SetActive(true);
    }

    private void OnAnswerClicked(int index)
    {
        questionPanel.SetActive(false);
        AnswerOption selected = currentQuestion.answers[index];
        bool isCorrect = (index == currentQuestion.correctAnswerIndex);

        ScoreManager.Instance.RegisterAnswer(isCorrect);
        onAnswerSelected?.Invoke(selected);
    }
}