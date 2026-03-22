using UnityEngine;
using UnityEngine.SceneManagement;

public class BriefingManager : MonoBehaviour
{
    public void OnEnterARButtonClicked()
    {
        SceneManager.LoadScene("ARScene");
    }
}