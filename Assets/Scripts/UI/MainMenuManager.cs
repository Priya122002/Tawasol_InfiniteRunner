using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject loadingPanel;   // Parent object of progress bar
    public Image progressFill;        // Filled Image

    [Header("Settings")]
    public float fillSpeed = 1.2f;    // Speed of progress fill

    bool isLoading = false;

    void Start()
    {
        // Ensure clean state
        if (loadingPanel != null)
            loadingPanel.SetActive(false);

        if (progressFill != null)
            progressFill.fillAmount = 0f;
    }

    public void StartGame()
    {
        if (isLoading) return;
        SoundManager.Instance.Play("click");
        isLoading = true;
        Debug.Log("Start button clicked");

        loadingPanel.SetActive(true);
        progressFill.fillAmount = 0f;

        StartCoroutine(LoadGameRoutine());
    }

    IEnumerator LoadGameRoutine()
    {
        // Fake loading bar (smooth & controllable)
        while (progressFill.fillAmount < 1f)
        {
            progressFill.fillAmount += Time.deltaTime * fillSpeed;
            yield return null;
        }

        // Small delay for polish
        yield return new WaitForSeconds(0.2f);

        SceneManager.LoadScene("GameScene");
    }

    public void ExitGame()
    {
        SoundManager.Instance.Play("click");
        Application.Quit();
        Debug.Log("Game Quit");
    }
}
