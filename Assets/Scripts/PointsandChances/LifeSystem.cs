using UnityEngine;
using UnityEngine.SceneManagement;

public class LifeSystem : MonoBehaviour
{
    public static LifeSystem Instance;

    public int maxLives = 3;
    public int currentLives;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentLives = maxLives;
        UIManager.Instance.UpdateLivesUI(currentLives);
    }

    public void PlayerHit()
    {
        currentLives--;

        if (currentLives > 0)
        {
            UIManager.Instance.ShowLifePopup(currentLives + " chances left");
        }

        if (currentLives <= 0)
        {
            UIManager.Instance.ShowLifePopup("No chances left!");
            GameOver();
        }
    }

    void GameOver()
    {
        // Freeze game completely
        Time.timeScale = 0f;

        // Stop player movement
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        if (pm != null) pm.canMove = false;

        // Stop scoring
        ScoreManager.Instance.StopScoring();

        // Show Game Over UI
        UIManager.Instance.ShowGameOver();
    }
    public void RestartGame()
    {
        // Freeze game completely
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }
    public void LoadMainMenuScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
