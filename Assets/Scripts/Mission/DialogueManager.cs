using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [Header("Personagens")]
    [SerializeField] private Animator gerenteAnimator;
    [SerializeField] private Animator clienteAnimator;
    [SerializeField] private Animator botAnimator;

    [Header("Painel de Diálogo")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button continueButton;

    [Header("Painel de Feedback")]
    [SerializeField] private GameObject feedbackPanel;
    [SerializeField] private TMP_Text feedbackConsequenceText;
    [SerializeField] private TMP_Text feedbackExplanationText;
    [SerializeField] private Button nextStepButton;

    private List<DialogueLine> currentDialogues;
    private int currentDialogueIndex = 0;
    private System.Action onDialogueComplete;

    public void StartDialogue(List<DialogueLine> dialogues, System.Action onComplete)
    {
        currentDialogues = dialogues;
        currentDialogueIndex = 0;
        onDialogueComplete = onComplete;

        dialoguePanel.SetActive(true);
        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (currentDialogueIndex >= currentDialogues.Count)
        {
            dialoguePanel.SetActive(false);
            onDialogueComplete?.Invoke();
            return;
        }

        DialogueLine line = currentDialogues[currentDialogueIndex];

        // Atualiza o nome do personagem e o texto
        characterNameText.text = GetCharacterName(line.character);
        dialogueText.text = line.text;

        // Ativa a animação de fala do personagem correto
        PlayTalkingAnimation(line.character);
    }

    public void OnContinueButtonClicked()
    {
        currentDialogueIndex++;
        ShowCurrentLine();
    }

    public void ShowFeedback(string consequence, string explanation, System.Action onComplete)
    {
        feedbackPanel.SetActive(true);
        feedbackConsequenceText.text = consequence;
        feedbackExplanationText.text = explanation;

        nextStepButton.onClick.RemoveAllListeners();
        nextStepButton.onClick.AddListener(() =>
        {
            feedbackPanel.SetActive(false);
            onComplete?.Invoke();
        });
    }

    private void PlayTalkingAnimation(CharacterType character)
    {
        // Para todos primeiro
        gerenteAnimator.ResetTrigger("PlayTalking");
        clienteAnimator.ResetTrigger("PlayTalking");
        botAnimator.ResetTrigger("PlayTalking");

        // Ativa o personagem correto
        switch (character)
        {
            case CharacterType.Gerente:
                gerenteAnimator.SetTrigger("PlayTalking");
                break;
            case CharacterType.Cliente:
                clienteAnimator.SetTrigger("PlayTalking");
                break;
            case CharacterType.Bot:
                botAnimator.SetTrigger("PlayTalking");
                break;
        }
    }

    public void PlayBotReaction(bool isCorrect)
    {
    botAnimator.ResetTrigger("PlayPositive");
    botAnimator.ResetTrigger("PlayNegative");
    
    if (isCorrect)
        botAnimator.SetTrigger("PlayPositive");
    else
        botAnimator.SetTrigger("PlayNegative");
    }

    private string GetCharacterName(CharacterType character)
    {
        switch (character)
        {
            case CharacterType.Gerente: return "Gerente";
            case CharacterType.Cliente: return "Cliente";
            case CharacterType.Bot: return "Bot Educacional";
            default: return "";
        }
    }
}