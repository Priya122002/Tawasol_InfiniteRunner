using UnityEngine;
using System.Collections;

public class LifeSystem : MonoBehaviour
{
    public static LifeSystem Instance;

    public int maxLives = 3;
    private int currentLives;

    private PlayerMovement player;

    // must match ObstacleDissolve.dissolveDuration
    public float dissolveWaitTime = 1f;

    void Awake()
    {
        Instance = this;
        currentLives = maxLives;
    }

    public void RegisterPlayer(PlayerMovement p)
    {
        player = p;
    }

    public void PlayerHit()
    {
        Debug.Log("LifeSystem.PlayerHit() called");

        currentLives--;
        Debug.Log("Lives left: " + currentLives);

        UIManager.Instance.UpdateLivesUI(currentLives);

        if (player == null)
        {
            Debug.LogError("Player reference is NULL in LifeSystem!");
            return;
        }

        Debug.Log("Stopping player movement");
        player.StopMovement();

        if (currentLives <= 0)
        {
            Debug.Log("GameOver coroutine started");
            StartCoroutine(GameOverAfterDissolve());
        }
        else
        {
            Debug.Log("Resume coroutine started");
            StartCoroutine(ResumeAfterDissolve());
        }
    }

    IEnumerator ResumeAfterDissolve()
    {
        yield return new WaitForSeconds(dissolveWaitTime);
        player.ResumeMovement();
    }

    IEnumerator GameOverAfterDissolve()
    {
        yield return new WaitForSeconds(dissolveWaitTime);

        UIManager.Instance.ShowGameOver();
        Time.timeScale = 0f;
    }
}
