using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRestartManager : MonoBehaviour
{
 
    public void RestartGame()
    {
        // 1️⃣ Resume time
        Time.timeScale = 1f;
        ObstacleSpawner.ResetSpawner();
        ScoreManager.Instance?.ResetScore();
        LifeSystem.Instance?.ResetLife();
        PlayerStateSender.ResetBuffer();

        // 3️⃣ Reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        ObstacleSpawner.ResetSpawner();
        ScoreManager.Instance?.ResetScore();
        LifeSystem.Instance?.ResetLife();
        PlayerStateSender.ResetBuffer();
        SceneManager.LoadScene("MainMenu");
       
      
    }
}
