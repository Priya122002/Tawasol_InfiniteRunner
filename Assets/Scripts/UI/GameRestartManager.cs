using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRestartManager : MonoBehaviour
{
 
    public void RestartGame()
    {
        SoundManager.Instance.Play("click");
        Time.timeScale = 1f;
        ObstacleSpawner.ResetSpawner();
        ScoreManager.Instance?.ResetScore();
        LifeSystem.Instance?.ResetLife();
        PlayerStateSender.ResetBuffer();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        SoundManager.Instance.Play("click");
        Time.timeScale = 1f;
        ObstacleSpawner.ResetSpawner();
        ScoreManager.Instance?.ResetScore();
        LifeSystem.Instance?.ResetLife();
        PlayerStateSender.ResetBuffer();
        SceneManager.LoadScene("MainMenu");
       
      
    }
}
