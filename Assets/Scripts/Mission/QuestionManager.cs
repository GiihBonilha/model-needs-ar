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

        for (int i = 0; i < answerButtons.Count; i++)
        {
            if (i < question.answers.Count)
            {
                int index = i;
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

        // Passa o índice escolhido para o ScoreManager
        ScoreManager.Instance.RegisterAnswer(isCorrect, index);
        onAnswerSelected?.Invoke(selected);
    }
}