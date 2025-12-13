using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private PlayerMovement player;

    [Header("Gameplay UI")]
  
    public TextMeshProUGUI orbText;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalDistanceText;
    public TextMeshProUGUI finalOrbText;

    public GameObject ChancelPanel;
    public TextMeshProUGUI livesText;
    public RectTransform orbCounterUI;
    public RectTransform coinTargetUI;  
    public GameObject uiCoinFlyPrefab;
    public Canvas mainCanvas;

    [Header("Speed Effect UI")]
    public GameObject playerSpeedEffectUI;
    public GameObject ghostSpeedEffectUI;
    public float speedEffectDuration = 3f;

    private Coroutine speedRoutine;

    void Awake()
    {
        Instance = this;
    }

    public void SetPlayer(PlayerMovement p)
    {
        player = p;
    }
    public void ShowSpeedEffect()
    {
        if (speedRoutine != null)
            StopCoroutine(speedRoutine);

        speedRoutine = StartCoroutine(SpeedEffectRoutine());
    }
    IEnumerator SpeedEffectRoutine()
    {
        if (playerSpeedEffectUI != null)
            playerSpeedEffectUI.SetActive(true);

        if (ghostSpeedEffectUI != null)
            ghostSpeedEffectUI.SetActive(true);

        yield return new WaitForSeconds(speedEffectDuration);

        if (playerSpeedEffectUI != null)
            playerSpeedEffectUI.SetActive(false);

        if (ghostSpeedEffectUI != null)
            ghostSpeedEffectUI.SetActive(false);
    }

    public void JumpButton()
    {
        SoundManager.Instance.Play("click");
        if (player != null)
            player.Jump();
    }
    void Update()
    {
        if (ScoreManager.Instance != null)
        {
          
            orbText.text = ScoreManager.Instance.orbScore.ToString();
        }
    }

    public void ShowGameOver()
    {
        SoundManager.Instance.Play("lost");
        gameOverPanel.SetActive(true);
        HideLifePopup();
        finalScoreText.text = "Final Score: " + ScoreManager.Instance.TotalScore;
        finalDistanceText.text = "Distance: " + ScoreManager.Instance.DistanceInt;
        finalOrbText.text = "Orbs: " + ScoreManager.Instance.orbScore;
    }
    public void UpdateLivesUI(int lives)
    {
        if (lives == 3)
            livesText.text = "3 chances";
        else if (lives == 2)
            livesText.text = "2 chances left";
        else if (lives == 1)
            livesText.text = "1 chance left";
        else
            livesText.text = "No chances left!";
    }
    public void ShowLifePopup(string msg)
    {
        SoundManager.Instance.Play("popup");
        ChancelPanel.gameObject.SetActive(true);
        livesText.text = msg;
       

        CancelInvoke(nameof(HideLifePopup));
        Invoke(nameof(HideLifePopup), 2f);   // Hide after 2 sec
    }

    void HideLifePopup()
    {
        ChancelPanel.gameObject.SetActive(false);
    }
    public void FlyCoinToUI(Vector3 worldPos, int amount)
    {
        // Convert world position to screen position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // Spawn UI coin
        GameObject coin = Instantiate(uiCoinFlyPrefab, mainCanvas.transform);
        RectTransform rt = coin.GetComponent<RectTransform>();
        rt.position = screenPos;

        StartCoroutine(MoveCoin(rt, amount));
    }

    IEnumerator MoveCoin(RectTransform coin, int amount)
    {
        Vector3 start = coin.position;
        Vector3 end = coinTargetUI.position;

        float t = 0f;
        float duration = 0.5f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            // Smooth curve motion
            float height = Mathf.Sin(t * Mathf.PI) * 80f;
            coin.position = Vector3.Lerp(start, end, t) + Vector3.up * height;

            yield return null;
        }

        // Finalize
        ScoreManager.Instance.AddOrb(amount);
        Destroy(coin.gameObject);
    }
}
