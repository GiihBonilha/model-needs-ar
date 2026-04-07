using UnityEngine;
using TMPro;
using System.Collections;

public class ComboManager : MonoBehaviour
{
    [SerializeField] private TMP_Text comboText;

    private int currentCombo = 0;
    private int maxCombo = 0;

    public int MaxCombo => maxCombo;

    public void RegisterCorrect()
    {
        currentCombo++;
        if (currentCombo > maxCombo)
            maxCombo = currentCombo;

        if (currentCombo >= 2)
            ShowComboMessage();
    }

    public void RegisterWrong()
    {
        currentCombo = 0;
    }

    private void ShowComboMessage()
    {
        string message = GetComboMessage(currentCombo);
        StopAllCoroutines();
        StartCoroutine(ShowAndHide(message));
    }

    private string GetComboMessage(int combo)
    {
        switch (combo)
        {
            case 2: return "Boa! " + "<sprite name=\"clapping\">";
            case 3: return "Em chamas! "+"<sprite name=\"fire\">";
            case 4: return "Imparável! "+"<sprite name=\"rocket\">";
            case 5: return "Perfeito! " + "<sprite name=\"star\">";
            default: return "Incrível! " + "<sprite name=\"stars\">";
        }
    }

    private IEnumerator ShowAndHide(string message)
    {
        comboText.text = message;
        comboText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        comboText.gameObject.SetActive(false);
    }
}