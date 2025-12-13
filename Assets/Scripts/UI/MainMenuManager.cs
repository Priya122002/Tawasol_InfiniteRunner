using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("game Scene open");
        SceneManager.LoadScene("GameScene"); 
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit"); 
    }
}
