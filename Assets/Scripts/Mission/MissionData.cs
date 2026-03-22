using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    Gerente,
    Cliente,
    Bot
}

public enum AnswerResult
{
    Correct,
    Wrong,
    Warning
}

[System.Serializable]
public class DialogueLine
{
    public CharacterType character;
    [TextArea(3, 6)]
    public string text;
}

[System.Serializable]
public class AnswerOption
{
    public string text;
    public AnswerResult result;
    [TextArea(2, 4)]
    public string consequenceText;
    [TextArea(2, 4)]
    public string explanationText;
}

[System.Serializable]
public class Question
{
    public string questionText;
    public string questionId; // M1, M2, M3, M4, M5
    public List<AnswerOption> answers = new List<AnswerOption>();
    public int correctAnswerIndex;
    [TextArea(2, 4)]
    public DialogueLine setupDialogue; // fala do personagem antes da pergunta
}

[System.Serializable]
public class MissionPhase
{
    public List<DialogueLine> dialogues = new List<DialogueLine>();
    public Question question; // null se for só diálogo sem pergunta
}