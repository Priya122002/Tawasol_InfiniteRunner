using UnityEngine;
using System.Collections;

public class LifeSystem : MonoBehaviour
{
    public static LifeSystem Instance;

    [Header("Lives")]
    public int maxLives = 3;
    private int currentLives;

    [Header("References")]
    private PlayerMovement player;

    // ✅ Explicit camera references
    public CameraShake playerCameraShake;
    public CameraShake ghostCameraShake;

    [Header("Timing")]
    public float dissolveWaitTime = 1f;

    private bool isProcessingHit = false;

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
        if (isProcessingHit) return;
        isProcessingHit = true;

        currentLives--;
        if (currentLives > 0)
            UIManager.Instance.ShowLifePopup(currentLives + " chance left");
        else
            UIManager.Instance.ShowLifePopup("No chances left!");
        UIManager.Instance.UpdateLivesUI(currentLives);

        if (player == null)
        {
            Debug.LogError("❌ Player not registered in LifeSystem");
            return;
        }

        // ⛔ Stop player movement
        player.StopMovement();

        // 🎥 One-time Y rotation hit wobble
        if (playerCameraShake != null)
            playerCameraShake.HitRotateY(2.5f, 0.25f);

        if (ghostCameraShake != null)
            ghostCameraShake.HitRotateY(2.5f, 0.25f);


        if (currentLives <= 0)
            StartCoroutine(GameOverAfterDissolve());
        else
            StartCoroutine(ResumeAfterDissolve());
    }

    IEnumerator ResumeAfterDissolve()
    {
        yield return new WaitForSeconds(dissolveWaitTime);
        player.ResumeMovement();
        isProcessingHit = false;
    }

    IEnumerator GameOverAfterDissolve()
    {
        yield return new WaitForSecondsRealtime(dissolveWaitTime);
        UIManager.Instance.ShowGameOver();
        Time.timeScale = 0f;
    }
    public void ResetLife()
    {
        currentLives = maxLives;
        isProcessingHit = false;
    }

}
